using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Transport.DGis.Domain.Entities
{
    public class OrgRub2
    {
        public OrgRub2()
        {
            OrgRub3Ids = new List<int>();    
        }

        public int OrgRub2Id { get; set; }
        public string Name { get; set; }
        public int OrgRub1Id { get; set; }
        public List<int> OrgRub3Ids { get; private set; }
    }
}
