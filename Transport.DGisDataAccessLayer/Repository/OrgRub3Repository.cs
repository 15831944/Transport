using GrymCore;
using Transport.DGis.Domain.Entities;


namespace Transport.DGis.DataAccessLayer.Repository
{
    public class OrgRub3Repository : Repository<OrgRub3>
    {
        public OrgRub3Repository(ITable table, IBaseViewThread baseView)
            : base(table, baseView)
        {
        }

        protected override OrgRub3 RowToEntity(IDataRow row)
        {
            return new OrgRub3
            {
                OrgRub3Id = row.Index,
                Name = (string)row.Value["name"],
                OrgRub2Id = ((IDataRow)row.Value["parent"]).Index
            };
        }
    }
}
