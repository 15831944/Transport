namespace Transport.DGis.Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public int? BuildingId { get; set; }
    }
}
