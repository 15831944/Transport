using System.Windows;
using System.Windows.Media;
using Transport.Aca3.Models;

namespace Transport.Aca3.ViewModels
{
    public class EdgeViewModel : VisualItemViewModel
    {
        public NodeViewModel Source { get; set; }
        public NodeViewModel Dest { get; set; }
        public double Distance { get; set; }

        public Color Color { get; set; } = Colors.Black;
        public double Thickness { get; set; } = 1.0;
    }
}
