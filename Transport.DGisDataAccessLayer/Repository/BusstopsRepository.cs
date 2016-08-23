using GrymCore;
using Transport.DGis.Domain.Entities;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public class BusstopsRepository : SpatialRepository<Busstop>
    {
        public BusstopsRepository(IDataRow cityDataRow, ITable table, IBaseViewThread baseView) 
            : base(cityDataRow, table, baseView)
        {
        }

        protected override Busstop RowToEntity(IDataRow row)
        {
            var fRow = row as IFeature;
            if (fRow == null) return null;

            var pointLocal = fRow.CenterPoint;
            var pointGeo = ((IMapCoordinateTransformationGeo)
                BaseView.Frame.Map.CoordinateTransformation).LocalToGeo(pointLocal);

            return new Busstop
            {
                BusstopId = row.Index,
                Name = (string)row.Value["name"],
                PosX = pointGeo.X,
                PosY = pointGeo.Y
            };
        }

        protected override string LayerName => "Grym_Map_UMLROSTP";
    }
}