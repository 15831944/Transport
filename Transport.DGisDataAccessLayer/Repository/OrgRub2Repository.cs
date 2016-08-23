using GrymCore;
using Transport.DGis.Domain.Entities;


namespace Transport.DGis.DataAccessLayer.Repository
{
    public class OrgRub2Repository : Repository<OrgRub2>
    {
        public OrgRub2Repository(ITable table, IBaseViewThread baseView)
            : base(table, baseView)
        {
        }

        protected override OrgRub2 RowToEntity(IDataRow row)
        {
            var orgRub2 = new OrgRub2
            {
                OrgRub2Id = row.Index,
                Name = (string)row.Value["name"],
                OrgRub1Id = ((IDataRow)row.Value["parent"]).Index
            };

            var orgRub3Count = (int)row.Value["rub3_count"];
            for (var i = 1; i <= orgRub3Count; i++)
            {
                var orgRub3 = (IDataRow)row.Value["rub3_" + i];
                if (orgRub3 == null) continue;
                orgRub2.OrgRub3Ids.Add(orgRub3.Index);
            }

            return orgRub2;
        }
    }
}
