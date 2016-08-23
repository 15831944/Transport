using Transport.Aca.Algorithm;

namespace Transport.Aca
{
    public class AdjacencyMatrixViewModel : MatrixViewModel
    {
        private readonly INetwork _network;
        public AdjacencyMatrixViewModel(INetwork network)
        {
            _network = network;
            _network.AdjacencyMatrixUpdated += (sender, args) => BuildMatrix();
        }

        protected override double[,] Matrix => _network.AdjacencyMatrix;
    }
}
