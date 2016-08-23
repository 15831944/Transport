using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport.Domain.Entities
{
    public class AreaRoutes
    {
        [Key, ForeignKey("Origin"), Column(Order = 0)]
        public long OriginId { get; set; }
        [Key, ForeignKey("Destination"), Column(Order = 1)]
        public long DestinationId { get; set; }

        public int Distance { get; set; }

        public virtual Area Origin { get; set; }
        public virtual Area Destination { get; set; }

    }
}
