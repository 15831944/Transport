using System.Windows;
using System.Windows.Media;
using Transport.Aca.Model;

namespace Transport.Aca
{
    public class NodeViewModel : ShapeViewModel
    {
        private readonly Node _node;

        public NodeViewModel(Node node)
        {
            _node = node;
        }

        public double Left => _node.Center.X - _node.Size / 2;
        public double Top => _node.Center.Y - _node.Size / 2;

        public string Name
        {
            get { return _node.Name; }
            set
            {
                if (_node.Name == value) return;
                _node.Name = value;
                RaisePropertyChanged();
            }
        }

        public Point Center
        {
            get { return _node.Center; }
            set
            {
                if (_node.Center == value) return;
                _node.Center = value;
                RaisePropertyChanged();
            }
        }

        public Color Color
        {
            get { return _node.Color; }
            set
            {
                if (_node.Color == value) return;
                _node.Color = value;
                RaisePropertyChanged();
            }
        }

        public double Size
        {
            get
            {
                return _node.Size;
            }
            set
            {
                _node.Size = value;
                RaisePropertyChanged();
            }
        }
    }
}
