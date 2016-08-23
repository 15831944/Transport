using GrymCore;
using Transport.DGis.Domain.Entities;


namespace Transport.DGis.DataAccessLayer.Repository
{
    public class OrgRub1Repository : Repository<OrgRub1>
    {
        public OrgRub1Repository(ITable table, IBaseViewThread baseView)
            : base(table, baseView)
        {
        }

        protected override OrgRub1 RowToEntity(IDataRow row)
        {
            var orgRub1 = new OrgRub1
            {
                OrgRub1Id = row.Index,
                Name = (string)row.Value["name"]
            };

            var orgRub2Count = (int)row.Value["rub2_count"];
            for (var i = 1; i <= orgRub2Count; i++)
            {
                var orgRub2 = (IDataRow)row.Value["rub2_" + i];
                if (orgRub2 == null) continue;
                orgRub1.OrgRub2Ids.Add(orgRub2.Index);
            }

            return orgRub1;
        }
    }
}
