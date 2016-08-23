using System;
using System.ComponentModel.DataAnnotations;
using Transport.Aca.Model;


namespace Transport.Aca.Algorithm
{
    public class Network : INetwork
    {

        private double[,] _adjacencyMatrix;
        private double[,] _directTravelersMatrix;
        private Node[] _nodes;

        public double[,] AdjacencyMatrix
        {
            get { return _adjacencyMatrix; }
            set
            {
                _adjacencyMatrix = value;
                CreateNodes();
                AdjacencyMatrixUpdated?.Invoke(this, null);
            }
        }

        private void CreateNodes()
        {
            var n = _adjacencyMatrix.GetLength(0);
            for (var i = 0; i < n; i++)
            {
                _nodes[i] = new Node();
            }

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (Math.Abs(_adjacencyMatrix[i,j]) < Constants.Tolerance) continue;

                    var edge = new Edge();
                    //edge.
                }
            }
        }

        public double[,] DirectTravelersMatrix
        {
            get { return _directTravelersMatrix; }
            set
            {
                _directTravelersMatrix = value;
                DirectTravelrsMatrixUpdated?.Invoke(this, null);
            }
        }

        public int Size => AdjacencyMatrix?.GetLength(0) ?? 0;

        public Node[] Nodes => _nodes;

        public event EventHandler AdjacencyMatrixUpdated;
        public event EventHandler DirectTravelrsMatrixUpdated;
        public event EventHandler NodesUpdated;
    }
}
