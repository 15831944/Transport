using GrymCore;
using Transport.DGis.Domain.Entities;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public class AddressRepository : Repository<Address>
    {
        public AddressRepository(ITable table, IBaseViewThread baseView) 
            : base(table, baseView)
        {
        }

        protected override Address RowToEntity(IDataRow row)
        {
            var street = (IDataRow) row.Value["street"];

            return new Address
            {
                AddressId = row.Index,
                Street = (string) street.Value["name"],
                Number = (string) row.Value["number"],
                BuildingId = ((IDataRow)row.Value["feature"])?.Index
            };
        }
    }
}
