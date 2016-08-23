using System.Collections.Generic;

namespace Transport.DGis.Domain.Entities
{
    public class OrgRub1
    {
        public OrgRub1()
        {
            OrgRub2Ids = new List<int>();
        }

        public int OrgRub1Id { get; set; }
        public string Name { get; set; }
        public List<int> OrgRub2Ids { get; private set; }
    }
}
