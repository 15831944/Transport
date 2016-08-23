using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Transport.Aca3.Helpers;

namespace Transport.Aca3.Models
{
    public class AcaAlgorithm
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly AcaConfiguration _acaConfiguration;
        private readonly DataSource _dataSource;
        // Матрица корреспонденций.
        // Отражает потребность в перевозке между всеми узлами сети
        // В ходе расчета уменьшается на количество условно перевезенных пассажиров
        private double[,] _demandMatrix;
        // матрица феромонов, откладываемых на ребрах
        private double[,] _pheromone;
        // матрица видимости (чем больше значение, тем более приоритетным является ребро)
        private double[,] _visibility;
        private int _size;
        private const double Tolerance = 1e-5;


        public AcaAlgorithm(AcaConfiguration acaConfiguration, DataSource dataSource)
        {
            _acaConfiguration = acaConfiguration;
            _dataSource = dataSource;
        }

        public bool CanStart()
        {
            return _acaConfiguration.IsValid &&
                   _dataSource.AdjacencyMatrix != null &&
                   _dataSource.DemandMatrix != null;
        }

        // выполняем поиск пути по заданному графу между O и D
        public void Start()
        {
            Logger.Info("Алгоритм поиска запущен");
            Initialization();

            var origin = 0;
            var destination = 15;

            // запускаем в поиск муравьев
            var paths = new List<Path>();
            for (var i = 0; i < _acaConfiguration.AntsInSubcolonyCount; i++)
            {
                var path = FindPath(origin, destination);
                // првоерить на ограниения
                paths.Add(path);
                // обновление матрицы феромонов
                PheromoneUpdate(path);
                //Logger.Info("{0}", string.Join("-", path.Nodes));
            }



            Logger.Info("Алгоритм поиска завершен");
        }

        // инициализация алгоритма
        private void Initialization()
        {
            // задаем размер сети
            _size = _dataSource.AdjacencyMatrix.GetLength(0);

            InitDemandMatrix();
            InitPheromoneMatrix();
            InitVisibilityMatrix();
        }

        // инициализация матрицы корреспонденций
        private void InitDemandMatrix()
        {
            _demandMatrix = new double[_size, _size];

            // копируем загруженную матрицу корееспонденций
            // для последующего изменения в процессе нахождения маршрутов
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    _demandMatrix[i, j] = _dataSource.DemandMatrix[i, j];
                }
            }
        }

        // инициализация матрицы феромонов
        private void InitPheromoneMatrix()
        {
            _pheromone = new double[_size, _size];

            var lengthSum = 0.0; // сумма длин
            var dtSum = 0.0; // сумма SP по матрице корреспонденций
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    lengthSum += _dataSource.AdjacencyMatrix[i, j];
                    dtSum += _demandMatrix[i, j];
                }
            }

            // начальное значение феромона
            var defPher = dtSum / lengthSum;

            // задание начального значния феромона
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    _pheromone[i, j] = defPher;
                }
            }
        }

        // расчет матрицы видимости
        private void InitVisibilityMatrix()
        {
            _visibility = new double[_size, _size];

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    if (Math.Abs(_dataSource.AdjacencyMatrix[i, j]) < Constants.Tolerance) continue;

                    _visibility[i, j] = _demandMatrix[i, j] / _dataSource.AdjacencyMatrix[i, j];
                }
            }
        }

        readonly Random _rand = new Random();

        // поиск пути муравьем из Origin в Destination
        private Path FindPath(int origin, int destination)
        {
            var visitedNodes = new List<int>();
            var path = new Path();
            visitedNodes.Add(origin);
            path.Add(origin);

            var curNode = origin;
            // генератор псевдослучайных чисел
           

            // предусмотреть условие выхода, в случае недостижимости destination из origin
            while (curNode != destination || !path.Nodes.Any())
            {
                var adjNodes = GetNotVisitedAdjacentNodes(curNode, visitedNodes);
                // проверка на доступные пути
                if (adjNodes.Length == 0)
                {
                    // если их нет, то нужно возвращаться назад
                    path.RemoveLast();
                    curNode = path.Destination;
                    continue;
                }

                // вычисление вероятностей перехода в каждую из вершин
                // Pij = (Tij^a * NUij^b) / sum(Tih * NUih);
                var tn = adjNodes.Select(j => Math.Pow(_pheromone[curNode, j], _acaConfiguration.Alpha) * Math.Pow(_visibility[curNode, j], _acaConfiguration.Beta)).ToArray();
                var sum = tn.Sum();
                // выбираем следующий узел
                curNode = adjNodes[GetNextNodeIndex(tn.Select(p => p / sum).ToArray(), _rand.NextDouble())];
                visitedNodes.Add(curNode);
                path.Add(curNode);
            }

            return path;
        }

        // получение массива смежных, непосещенных вершин
        private int[] GetNotVisitedAdjacentNodes(int current, ICollection<int> visitedNodes)
        {
            var nodes = new HashSet<int>();
            // смежные узлы
            for (var j = 0; j < _size; j++)
            {
                if (Math.Abs(_visibility[current, j]) > Tolerance && !visitedNodes.Contains(j))
                {
                    nodes.Add(j);
                }
            }

            return nodes.ToArray();
        }

        /// <summary>
        /// Получение индекса диапазона, в который будет производиться переход
        /// </summary>
        /// <param name="transProb">веротяности перехода</param>
        /// <param name="value">случайное значение</param>
        /// <returns>Индекс </returns>
        private int GetNextNodeIndex(double[] transProb, double value)
        {
            if (!transProb.Any())
                throw new ArgumentException(@"В массиве вероятности переходов отсутствуют элементы", nameof(transProb));

            if (value < 0.0 || value > 1.0)
                throw new ArgumentException(@"Случайное значение должно находится в границах от 0 до 1", nameof(value));

            if (transProb.Length > 1)
            {
                var left = 0.0;
                for (var i = 0; i < transProb.Length; i++)
                {
                    var right = left + transProb[i];
                    if (left < value && value < right)
                    {
                        return i;
                    }

                    left = right;
                }
            }

            return 0;
        }

        /// <summary>
        /// Обновление феромона в матрице
        /// </summary>
        /// <param name = "path" > Построенный путь O-D</param>
        private void PheromoneUpdate(Path path)
        {
            var p = path.Nodes.ToArray();
            var quantity = CountTravalersQuantityOnPath(path);
            var len = CountPathLength(path);
            var pathDtd = CountDirectTravalersDensityOnPath(path);
            var stopsCount = path.Nodes.Count();

            // нужно еще применить min/max

            // испарение
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    _pheromone[i, j] = (1 - _acaConfiguration.EvoporationSpeed) * _pheromone[i, j];
                }
            }

            for (var i = 0; i < stopsCount - 1; i++)
            {
                var s = p[i];
                var f = p[i + 1];
                var dPher = _acaConfiguration.Q / pathDtd *
                                             ((pathDtd - _visibility[s, f]) / ((stopsCount - 2) * pathDtd));
                _pheromone[s, f] += dPher;
            }

            Logger.Info($"{string.Join("-", p)}. Количество: {quantity} Длина: {len} Плотность: {pathDtd}");
        }

        /// <summary>
        /// Вычисление плотности пассажиров прямого сообщения на маршруте
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private double CountDirectTravalersDensityOnPath(Path path)
        {
            var pathLength = CountPathLength(path);
            var travalersQuantity = CountTravalersQuantityOnPath(path);

            return travalersQuantity / pathLength;
        }

        /// <summary>
        /// Вычисление количества пассажиров прямого сообщения на маршруте
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private double CountTravalersQuantityOnPath(Path path)
        {
            var travalersQuantity = 0.0;

            var p = path.Nodes.ToArray();
            for (var i = 0; i < p.Length - 1; i++)
            {
                for (var j = i + 1; j < p.Length; j++)
                {
                    travalersQuantity += _demandMatrix[i, j];
                }
            }

            return travalersQuantity;
        }


        /// <summary>
        /// Вычисление длины маршрута
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private double CountPathLength(Path path)
        {
            var pathLength = 0.0;

            var p = path.Nodes.ToArray();
            for (var i = 0; i < p.Length - 1; i++)
            {
                pathLength += _dataSource.AdjacencyMatrix[p[i], p[i + 1]];
            }

            return pathLength;
        }
    }
}
