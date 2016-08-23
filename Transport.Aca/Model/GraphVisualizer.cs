using System.Collections.Generic;

namespace Transport.Aca.Model
{
    public class GraphVisualizer
    {
        private readonly Graph _graph;
        private readonly List<NodeVisual> _nodeVisuals = new List<NodeVisual>();
        private readonly List<EdgeVisual> _edgeVisuals = new List<EdgeVisual>();

        public GraphVisualizer(Graph graph)
        {
            _graph = graph;
        }

        private void MakeVisuals()
        {
            _nodeVisuals.Clear();
            _edgeVisuals.Clear();

            foreach (var node in _graph.Nodes)
            {
                _nodeVisuals.Add(new NodeVisual(node)
                {
                    
                });
            }

            foreach (var edge in _graph.Edges)
            {
                _edgeVisuals.Add(new EdgeVisual(edge)
                {
                    
                });
            }
        }

        public IVisual Shapes { get; }
    }
}
