using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Transport.Aca3.ViewModels
{
    public class NodeViewModel : VisualItemViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        private List<EdgeViewModel> _edges;
        public List<EdgeViewModel> Edges => _edges ?? (_edges = new List<EdgeViewModel>());
        public List<NodeViewModel> AdjacenNodes => Edges.Select(e => e.Dest).ToList();

        public Point Center { get; set; }
        public double Left => Center.X - Size / 2;
        public double Top => Center.Y - Size / 2;

        public Color Color { get; set; } = Colors.White;
        public double Size { get; set; } = 25.0;
    }
}
