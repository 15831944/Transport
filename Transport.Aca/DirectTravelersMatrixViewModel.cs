using Transport.Aca.Algorithm;

namespace Transport.Aca
{
    public class DirectTravelersMatrixViewModel : MatrixViewModel
    {
        private readonly INetwork _network;
        public DirectTravelersMatrixViewModel(INetwork network) 
        {
            _network = network;
            _network.DirectTravelrsMatrixUpdated += (sender, args) => BuildMatrix();
        }

        protected override double[,] Matrix => _network.DirectTravelersMatrix;
    }
}
