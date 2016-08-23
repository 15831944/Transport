using System;
using System.Collections.Generic;
using System.Linq;

namespace Transport.Aca.Algorithm
{
    public class Ant
    {
        private readonly double[,] _map;
        private readonly double[,] _pheromone;
        private readonly AcaConfiguration _configuration;
        private readonly int _mapSize;

        public Ant(double[,] map, double[,] pheromone, AcaConfiguration configuration)
        {
            _map = map;
            _pheromone = pheromone;
            _configuration = configuration;
            _mapSize = map.GetLength(1);
        }

        // Посещенные муравьем узлы
        private readonly HashSet<int> _visitedNodes = new HashSet<int>();

        // Путь
        public Path Path { get; } = new Path();

        public Path FindPath(int origin, int destination)
        {
            _visitedNodes.Add(origin);
            Path.Add(origin);
            var curNode = origin;
            // генератор псевдослучайных чисел
            var rand = new Random();

            while (curNode != destination)
            {
                var adjNodes = GetNotVisitedAdjacentNodes(curNode);
                // проверка на доступные пути
                if (adjNodes.Length == 0)
                {
                    // если их нет, то нужно возвращаться назад
                    curNode = Path.PopLast();
                    continue;
                }

                // вычисление вероятностей перехода в каждую из вершин
                // Pij = (Tij^a * NUij^b) / sum(Tih * NUih);
                var tn = adjNodes.Select(j => Math.Pow(_pheromone[curNode, j], _configuration.Alpha) * Math.Pow(_map[curNode, j], _configuration.Beta)).ToArray();
                var sum = tn.Sum();
                // выбираем следующий узел
                curNode = adjNodes[GetNextNodeIndex(tn.Select(p => p / sum).ToArray(), rand.NextDouble())];
                _visitedNodes.Add(curNode);
                Path.Add(curNode);
            }

            return Path;
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

        // получение массива смежных, непосещенных вершин
        private int[] GetNotVisitedAdjacentNodes(int current)
        {
            var nodes = new HashSet<int>();
            // смежные узлы
            for (var j = 0; j < _mapSize; j++)
            {
                if (Math.Abs(_map[current, j]) > Constants.Tolerance && !_visitedNodes.Contains(j))
                {
                    nodes.Add(j);
                }
            }

            return nodes.ToArray();
        }

    }
}
