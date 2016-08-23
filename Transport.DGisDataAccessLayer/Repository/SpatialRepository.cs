using System.Collections.Generic;
using GrymCore;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public abstract class SpatialRepository<TEntity>: Repository<TEntity>
        where TEntity : class
    {
        private readonly IDataRow _cityDataRow;

        protected SpatialRepository(IDataRow cityDataRow, ITable table, IBaseViewThread baseView)
            : base(table, baseView)
        {
            _cityDataRow = cityDataRow;
        }

        public override IList<TEntity> GetAll()
        {
            var entities = new List<TEntity>();

            var layer = BaseView.Frame.Map.Layers.FindLayer(LayerName);
            var spatialQuery = BaseView.Database.CreateQuery("spatial");
            var fCity = (IFeature)_cityDataRow;

            spatialQuery.AddCriterion("layer", layer);
            spatialQuery.AddCriterion("filter", fCity);
            spatialQuery.Execute();

            IDataRow dRow;
            while ((dRow = spatialQuery.Fetch()) != null)
            {
                var entity = RowToEntity(dRow);
                entities.Add(entity);
            }

            return entities;
        }

        protected abstract string LayerName { get; }
    }
}
