using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Transport.Aca.Model
{
    public class Node
    {
        #region Visuals

        public string Name { get; set; }
        public Point Center { get; set; }
        public Color Color { get; set; } = Colors.White;
        public double Size { get; set; } = 30.0;

        #endregion


        private readonly List<Edge> _edges = new List<Edge>();

        public IReadOnlyList<Node> AdjacentNodes => _edges.Select(e => e.Destination).ToList();
        public IReadOnlyList<double> Costs => _edges.Select(e => e.Cost).ToList();
        public IReadOnlyList<Edge> Edges => _edges;

        public void AddAdjacentNode(Node node, double cost)
        {
            AddEdge(new Edge
            {
                Origin = this,
                Destination = node,
                Cost = cost
            });
        }

        public void RemoveNode(Node node)
        {
            _edges.RemoveAll(e => e.Destination == node);
        }

        public void AddEdge(Edge edge)
        {
            if (edge.Origin != this) throw new ArgumentException(@"Ребро не начинается в текущем узле");
            if (_edges.Contains(edge)) throw new ArgumentException(@"Ребро уже добавлено");

            _edges.Add(edge);
        }

        public void RemoveEdge(Edge edge)
        {
            _edges.Remove(edge);
        }
    }
}
