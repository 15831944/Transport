using System;
using System.Collections.Generic;
using System.Linq;

namespace Transport.Aca.Algorithm
{
    public class Colony
    {
        private readonly Network _network;
        private readonly AcaConfiguration _configuration;
        private readonly double[,] _map;
        private readonly double[,] _pheromone;
        private readonly int _size;

        public Colony(Network network, AcaConfiguration configuration)
        {
            _network = network;
            _configuration = configuration;
            
            _size = _network.DirectTravelersMatrix.GetLength(0);
            _pheromone = new double[_size, _size];
            _map = new double[_size, _size];

            Initialize();
        }

        private void Initialize()
        {
            var lengthSum = 0.0;
            var dtSum = 0.0;
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    lengthSum += _network.AdjacencyMatrix[i, j];
                    dtSum += _network.DirectTravelersMatrix[i, j];

                    if (Math.Abs(_network.AdjacencyMatrix[i, j]) < Constants.Tolerance) continue;
                    _map[i, j] = _network.DirectTravelersMatrix[i, j] / _network.AdjacencyMatrix[i, j];
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

        // Список муравьев
        private readonly List<Ant> _ants = new List<Ant>();

        public void FindPath(int origin, int destination)
        {
            // создаем требуемое количество муравьев и запускаем нахождение пути
            for (var i = 0; i < _configuration.AntsInSubcolonyCount; i++)
            {
                var ant = new Ant(_map, _pheromone, _configuration);
                _ants.Add(ant);
                var path = ant.FindPath(origin, destination);
                path.CountParamsForNetwork(_network);
                PheromoneUpdate(path);
            }
        }

        /// <summary>
        /// Обновление феромона в матрице
        /// </summary>
        /// <param name="path">Построенный путь O-D</param>
        private void PheromoneUpdate(Path path)
        {
            // расчет среднего значения феромона, а также минимальных и максимальных значений плотности??
            // испарение
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    _pheromone[i, j] += (1 - _configuration.EvoporationSpeed) * _pheromone[i, j];
                }
            }

            foreach (var link in path.Links)
            {
                var i = link.Start;
                var j = link.Finish;
                var pDtd = path.DirectTravelersDensty;

                var dPher = _configuration.Q / pDtd *
                            ((pDtd - _network.DirectTravelersMatrix[i, j]) / ((path.Nodes.Count() - 2) * pDtd));
                _pheromone[i, j] += dPher;
            }
        }
    }
}
