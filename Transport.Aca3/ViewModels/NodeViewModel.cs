using System.Windows;
using System.Windows.Media;

namespace Transport.Aca3.ViewModels
{
    public class NodeViewModel : VisualItemViewModel
    {
        public string Name { get; set; }
        public Point Center { get; set; }

        public double Left => Center.X - Size / 2;
        public double Top => Center.Y - Size / 2;

        public Color Color { get; set; } = Colors.White;
        public double Size { get; set; } = 25.0;
    }
}
