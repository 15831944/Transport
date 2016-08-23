using System.Collections.Generic;
using GrymCore;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public abstract class Repository<TEntity>
        : IRepository<TEntity> where TEntity : class 
    {
        protected ITable Table { get; }
        protected IBaseViewThread BaseView { get; }

        protected Repository(ITable table, IBaseViewThread baseView)
        {
            Table = table;
            BaseView = baseView;
        }

        public int Count()
        {
            return GetAll().Count;
        }

        public TEntity GetById(int id)
        {
            var entity = RowToEntity(Table.GetRecord(id));
            return entity;
        }

        public virtual IList<TEntity> GetAll()
        {
            var entities = new List<TEntity>();

            for (var i = 1; i <= Table.RecordCount; i++)
            {
                entities.Add(RowToEntity(Table.GetRecord(i)));
            }

            return entities;
        }

        protected abstract TEntity RowToEntity(IDataRow row);
    }
}
