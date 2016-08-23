using System.Windows;
using System.Windows.Media;
using Transport.Aca.Model;

namespace Transport.Aca
{
    public class EdgeViewModel : ShapeViewModel
    {
        private readonly Edge _edge;

        public EdgeViewModel(Edge edge)
        {
            _edge = edge;
        }

        public Point Point1 => _edge.Origin.Center;
        public Point Point2 => _edge.Destination.Center;

        public Color Color
        {
            get
            {
                return _edge.Color;
            }
            set
            {
                if (_edge.Color == value) return;
                _edge.Color = value;
                RaisePropertyChanged();
            }
        }

        public double Thickness
        {
            get
            {
                return _edge.Thickness;
            }
            set
            {
                _edge.Thickness = value;
                RaisePropertyChanged();
            }
        }
    }
}
