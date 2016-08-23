using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Transport.Domain.Entities
{
    public class Busstop
    {
        [Key]
        public long BusstopId { get; set; }
        public string Name { get; set; }
        public DbGeography Location { get; set; }
        public long? AreaId { get; set; }

        public virtual Area Area { get; set; }
    }
}
