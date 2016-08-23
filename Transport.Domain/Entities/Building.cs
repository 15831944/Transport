using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

namespace Transport.Domain.Entities
{
    public class Building
    {
        public Building()
        {
            Addresses = new HashSet<Address>();
            OrgFils = new HashSet<OrgFil>();
        }

        [Key]
        public long BuildingId { get; set; }
        public int Levels { get; set; } 
        public DbGeography Location { get; set; }
        public string Purpose { get; set; }
        public string PostIndex { get; set; }
        public double Square { get; set; }
        public DbGeography Polygon { get; set; }

        public long? AreaId { get; set; }

        public virtual ICollection<Address> Addresses { get; private set; }
        public virtual ICollection<OrgFil> OrgFils { get; private set; }
        public virtual Area Area { get; set; }
    }
}
