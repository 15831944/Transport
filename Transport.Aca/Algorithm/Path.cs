using System;
using System.Collections.Generic;
using System.Linq;

namespace Transport.Aca.Algorithm
{
    public class Path
    {
        public int Origin => Nodes.First();
        public int Destination => Nodes.Last();

        // Узлы
        private List<int> _nodes;
        public IEnumerable<int> Nodes => _nodes ?? (_nodes = new List<int>());

        // Ребра
        public IEnumerable<Link> Links
        {
            get
            {
                if (Nodes.Count() < 2)
                {
                    yield break;
                }

                for (var i = 0; i < _nodes.Count - 1; i++)
                {
                    yield return new Link(_nodes[i], _nodes[i + 1]);
                }
            }
        }

        public void Add(int value)
        {
            _nodes.Add(value);
        }

        public int PopLast()
        {
            var c = _nodes.Count - 1;
            if (c < 0) throw new InvalidOperationException(@"В пути нет элементов для удаления");
            var item = _nodes.ElementAt(c);
            _nodes.RemoveAt(c);

            return item;
        }

        private double _length;
        public double Length
        {
            get
            {
                if (_isParamsCounted) return _length;

                throw new InvalidOperationException(
                    @"Требуется предварительно выполнить обсчет характеристик маршрута");
            }
        }

        private double _travelersCount = 0.0;

        public double TravelersCount
        {
            get
            {
                if (_isParamsCounted) return _travelersCount;

                throw new InvalidOperationException(
                    @"Требуется предварительно выполнить обсчет характеристик маршрута");
            }
        }

        public double DirectTravelersDensty
        {
            get
            {
                if (!_isParamsCounted)
                {
                    throw new InvalidOperationException(
                    @"Требуется предварительно выполнить обсчет характеристик маршрута");
                }

                if (Math.Abs(TravelersCount) < Constants.Tolerance)
                {
                    return 0.0;
                }

                return TravelersCount / Length;
            }
        }

        private bool _isParamsCounted;

        /// <summary>
        /// Расчет характеристик маршрута
        /// </summary>
        /// <param name="network">Маршрутная сеть</param>
        public void CountParamsForNetwork(Network network)
        {
            for (var i = 0; i < _nodes.Count - 1; i++)
            {
                _length += network.AdjacencyMatrix[_nodes[i], _nodes[i + 1]];
                for (var j = i + 1; j < _nodes.Count; j++)
                {
                    _travelersCount += network.DirectTravelersMatrix[_nodes[i], _nodes[j]];
                }
            }

            _isParamsCounted = true;
        }
    }
}
