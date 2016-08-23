using System.Collections.Generic;

namespace Transport.DGis.Domain.Entities
{
    public class Org
    {
        public Org()
        {
            OrgRubs3Ids = new List<int>();
            OrgFils = new List<OrgFil>();
        }

        public int OrgId { get; set; }
        public string Name { get; set; }

        public List<int> OrgRubs3Ids { get; private set; }
        public List<OrgFil> OrgFils { get; private set; }
    }
}
