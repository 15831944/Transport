using System.Collections.Generic;

namespace Transport.DGis.DataAccessLayer.Repository
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        #region Retrieval Operations

        int Count();
        TEntity GetById(int id);
        IList<TEntity> GetAll();

        #endregion // Retrieval Operations
    }
}
