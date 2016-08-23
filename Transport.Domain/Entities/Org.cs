using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport.Domain.Entities
{
    public class Org
    {
        public Org()
        {
            OrgRubs = new HashSet<OrgRub>();
            OrgFils = new HashSet<OrgFil>();
        }

        [Key]
        public long OrgId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OrgRub> OrgRubs { get; private set; }
        public virtual ICollection<OrgFil> OrgFils { get; private set; }
    }
}
