using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Transport.Aca.Algorithm;
using Transport.Aca.Model;

namespace Transport.Aca
{
    public class GraphViewModel : ViewModelBase
    {
        private readonly IGraph _graph;

        public GraphViewModel(IGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<ShapeViewModel> Shapes
        {
            get
            {
                var shapes = new List<ShapeViewModel>();

                foreach (var node in _graph.Nodes)
                {
                    shapes.Add(new NodeViewModel(node));
                }

                foreach (var edge in _graph.Edges)
                {
                    shapes.Add(new EdgeViewModel(edge));
                }

                return shapes;
            }
        }
    }
}
