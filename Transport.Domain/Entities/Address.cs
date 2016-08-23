using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Domain.Entities
{
    public class Address
    {
        [Key]
        public long AddressId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }

        public long BuildingId { get; set; }

        public virtual Building Building { get; set; }
        
    }
}
