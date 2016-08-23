using GrymCore;
using Transport.DGis.Domain.Entities;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public class BuildingsRepository : SpatialRepository<Building>
    {
        public BuildingsRepository(IDataRow cityDataRow, ITable table, IBaseViewThread baseView)
            : base(cityDataRow, table, baseView)
        { }

        protected override Building RowToEntity(IDataRow row)
        {
            var fRow = row as IFeature;
            if (fRow == null) return null;

            var pointLocal = fRow.CenterPoint;
            var pointGeo = ((IMapCoordinateTransformationGeo)
                BaseView.Frame.Map.CoordinateTransformation).LocalToGeo(pointLocal);

            var addrCount = (int)row.Value["addr_count"];

            var building = new Building();
            building.BuildingId = row.Index;
            building.Levels = (int)row.Value["levels"];
            building.PosX = pointGeo.X;
            building.PosY = pointGeo.Y;
            building.Purpose = (string)row.Value["purpose"];
            building.PostIndex = (string)row.Value["post_index"];

            for (var i = 1; i <= addrCount; i++)
            {
                var addr = (IDataRow)row.Value["addr_" + i];
                if (addr == null)
                    continue;
                building.AddressesIds.Add(addr.Index);
            }

            return building;
        }

        public Building GetBuilingByCoords(double lat, double lon)
        {
            var building = new Building();

            var layer = BaseView.Frame.Map.Layers.FindLayer(LayerName);
            var spatialQuery = BaseView.Database.CreateQuery("spatial");

            const double delta = 0.0005;

            var geoPoints = new[]
            {
                BaseView.Factory.CreateMapPoint(lon - delta, lat - delta),
                BaseView.Factory.CreateMapPoint(lon - delta, lat + delta),
                BaseView.Factory.CreateMapPoint(lon + delta, lat + delta),
                BaseView.Factory.CreateMapPoint(lon + delta, lat - delta)
            };
            
            var shape = ((IGrymObjectFactory2)BaseView.Factory).CreateShape();
            shape.AddComponent(ComponentDimension.ComponentDimensionPolygon);

            foreach (var geoPoint in geoPoints)
            {
                var localPoint = ((IMapCoordinateTransformationGeo)
                BaseView.Frame.Map.CoordinateTransformation).GeoToLocal(geoPoint);
                shape.AddPoint(localPoint.X, localPoint.Y);
            }
            
            spatialQuery.AddCriterion("layer", layer);
            spatialQuery.AddCriterion("filter", shape);
            spatialQuery.Execute();

            var dRow = spatialQuery.Fetch();
            return dRow != null ? RowToEntity(dRow) : null;
        }

        protected override string LayerName => "Grym_Map_UMLRHOUS";
    }
}
