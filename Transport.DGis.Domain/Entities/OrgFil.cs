using System.Collections.Generic;

namespace Transport.DGis.Domain.Entities
{
    public class OrgFil
    {
        public OrgFil()
        {
            OrgRubs3Ids = new List<int>();
            AddressesIds = new List<int>();
        }

        public int OrgFilId { get; set; }
        public int OrgId { get; set; }
        public List<int> OrgRubs3Ids { get; private set; }
        public List<int> AddressesIds { get; private set; }
    }
}
