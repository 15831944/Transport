using System;
using System.Windows;
using System.Windows.Media;

namespace Transport.Aca.Model
{
    public class Edge : IEquatable<Edge>
    {
        public Node Origin { get; set; }
        public Node Destination { get; set; }
        public double Cost { get; set; }

        #region Visuals

        public Color Color { get; set; } = Colors.Black;
        public double Thickness { get; set; } = 1.0;

        #endregion // Visuals

        public bool Equals(Edge edge)
        {
            return ReferenceEquals(Origin, edge.Origin) && ReferenceEquals(Destination, edge.Destination);
        }

        public override bool Equals(object obj)
        {
            var edge = obj as Edge;
            return edge != null && Equals(edge);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Edge e1, Edge e2)
        {
            return e1 != null && e1.Equals(e2);
        }

        public static bool operator !=(Edge e1, Edge e2)
        {
            return e1 == null || !e1.Equals(e2);
        }
    }
}
