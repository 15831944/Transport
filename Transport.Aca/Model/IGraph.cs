using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Transport.Aca.Model
{
    public interface IGraph
    {
        double[,] AdjacencyMatrix { get; }
        IReadOnlyList<Edge> Edges { get; }
        ReadOnlyObservableCollection<Node> Nodes { get; }
        int Size { get; }

        void AddEdge(Edge edge);
        void AddNode(Node node);
        void Clear();
        void GenerateFromAdjacencyMatrix(double[,] adjacencyMatrix);
        void RemoveEdge(Edge edge);
        void RemoveNode(Node node);
    }
}