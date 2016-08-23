using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport.Domain.Entities
{
    public class OrgRub
    {
        public OrgRub()
        {
            ChildOrgRubs = new HashSet<OrgRub>();
            Orgs = new HashSet<Org>();
        }

        [Key]
        public long OrgRubId { get; set; }
        public string Name { get; set; }
        [ForeignKey("ParentOrgRub")]
        public long? ParentOrgRubId { get; set; }

        public virtual OrgRub ParentOrgRub { get; set; }
        public virtual ICollection<OrgRub> ChildOrgRubs { get; private set; }
        public virtual ICollection<Org> Orgs { get; private set; }
    }
}
