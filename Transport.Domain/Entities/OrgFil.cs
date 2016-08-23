using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Domain.Entities
{
    public class OrgFil
    {
        [Key]
        public long OrgFilId { get; set; }
        public long OrgId { get; set; }
        public long BuildingId { get; set; }

        public virtual Org Org { get; set; }
        public virtual Building Building { get; set; }
        
    }
}
