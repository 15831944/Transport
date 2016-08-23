using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Transport.Aca.Algorithm;

namespace Transport.Aca.Model
{
    public class Graph : IGraph
    {
        private readonly ObservableCollection<Node> _nodes = new ObservableCollection<Node>();

        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        public void RemoveNode(Node node)
        {
            // Удаление узла из внутренних списков
            foreach (var n in _nodes.Where(n => n.AdjacentNodes.Contains(node)))
            {
                n.RemoveNode(node);
            }

            var b = _nodes.Contains(node);

            _nodes.Remove(node);
        }

        public void AddEdge(Edge edge)
        {
            var origin = edge.Origin;
            var dest = edge.Destination;

            if (!_nodes.Contains(origin)) AddNode(origin);
            if (!_nodes.Contains(dest)) AddNode(dest);

            origin.AddEdge(edge);
        }

        public void RemoveEdge(Edge edge)
        {
            var origin = edge.Origin;
            origin.RemoveEdge(edge);
        }

        public void GenerateFromAdjacencyMatrix(double[,] adjacencyMatrix)
        {
            var n = adjacencyMatrix.GetLength(0);
            if (n != adjacencyMatrix.GetLength(1))
            {
                throw new ArgumentException(@"Некорректная размерность матрицы смежности", nameof(adjacencyMatrix));
            }

            // очистка графа
            Clear();
            
            // создаем узлы
            for (var i = 0; i < n; i++)
            {
                AddNode(new Node());
            }

            // создаем ребра
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (Math.Abs(adjacencyMatrix[i,j]) < Constants.Tolerance) continue;
                    Nodes[i].AddAdjacentNode(Nodes[j], adjacencyMatrix[i, j]);
                }
            }
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        private ReadOnlyObservableCollection<Node> _readonlyNodes;
        public ReadOnlyObservableCollection<Node> Nodes => _readonlyNodes ?? (_readonlyNodes = new ReadOnlyObservableCollection<Node>(_nodes));

        public IReadOnlyList<Edge> Edges => _nodes.SelectMany(n => n.Edges).ToList();

        public int Size => _nodes.Count;

        private double[,] _adjacencyMatrix;
        // восстановление матрицы корреспонденций по узлам
        // TODO: перенести в отображение в модели ей не место
        public double[,] AdjacencyMatrix
        {
            get
            {
                _adjacencyMatrix = new double[Size, Size];

                for (var i = 0; i < Size; i++)
                {
                    for (var k = 0; k < _nodes[i].AdjacentNodes.Count; k++)
                    {
                        var j = _nodes.IndexOf(_nodes[i].AdjacentNodes[k]);
                        _adjacencyMatrix[i, j] = _nodes[i].Costs[k];
                    }
                }

                return _adjacencyMatrix;
            }
        }
    }
}
