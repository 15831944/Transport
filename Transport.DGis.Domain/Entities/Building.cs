using System.Collections.Generic;

namespace Transport.DGis.Domain.Entities
{
    public class Building
    {
        public Building()
        {
            AddressesIds = new List<int>();
        }

        public int BuildingId { get; set; }
        public int Levels { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public string Purpose { get; set; }
        public string PostIndex { get; set; }

        public List<int> AddressesIds { get; private set; }
    }
}
