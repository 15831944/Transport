using GrymCore;
using Transport.DGis.Domain.Entities;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public class OrgRepository : Repository<Org>
    {
        public OrgRepository(ITable table, IBaseViewThread baseView)
            : base(table, baseView)
        {
        }

        protected override Org RowToEntity(IDataRow row)
        {
            var org = new Org
            {
                OrgId = row.Index,
                Name = (string)row.Value["name"]
            };

            var filCount = (int)row.Value["fil_count"];
            for (var i = 1; i <= filCount; i++)
            {
                var filRow = (IDataRow)row.Value["fil_"+i];
                if (filRow == null) continue;

                var fil = new OrgFil
                {
                    OrgFilId = filRow.Index,
                    OrgId = row.Index
                };

                var filRubsCount = (int)filRow.Value["rub_count"];
                for (var j = 1; j <= filRubsCount; j++)
                {
                    var rub = (IDataRow)filRow.Value["rub_" + j];
                    if (rub == null) continue;
                    fil.OrgRubs3Ids.Add(rub.Index);
                }

                var filAddrCount = (int)filRow.Value["addr_count"];
                for (var j = 1; j <= filAddrCount; j++)
                {
                    var addr = (IDataRow)filRow.Value["addr_" + j];
                    if (addr == null) continue;
                    fil.AddressesIds.Add(addr.Index);
                }

                org.OrgFils.Add(fil);
            }

            var rubCount = (int)row.Value["rub_count"];
            for (var i = 1; i <= rubCount; i++)
            {
                var rub = (IDataRow)row.Value["rub_" + i];
                if (rub == null) continue;
                org.OrgRubs3Ids.Add(rub.Index);
            }

            return org;
        }
    }
}
