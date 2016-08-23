using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Aca.Model
{
    public class Network
    {
        private Graph _graph;
        private double[,] _directTravelersMatrix;

        public Network(Graph graph)
        {
            _graph = graph;
        }

        public void LoadDirectTravelersMatrix(double[,] dtdMatrix)
        {
            _directTravelersMatrix = dtdMatrix;
        }
    }
}
