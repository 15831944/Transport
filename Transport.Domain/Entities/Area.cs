using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Transport.Domain.Entities
{
    public class Area
    {
        public Area()
        {
            Busstops = new HashSet<Busstop>();
            Buildings = new HashSet<Building>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AreaId { get; set; }
        public string Name { get; set; }
        public DbGeography Location { get; set; }
        public int People { get; set; }
        /// <summary>
        /// Процент людей, отправляющихся из зоны
        /// </summary>
        public double DeparturePct { get; set; }
        /// <summary>
        /// Процент людей, прибывающих в зону
        /// </summary>
        public double ArrivalPct { get; set; }
        public virtual ICollection<Busstop> Busstops { get; private set; }
        public virtual ICollection<Building> Buildings { get; private set; }
    }
}
