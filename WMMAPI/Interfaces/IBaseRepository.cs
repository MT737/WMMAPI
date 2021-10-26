using System;

namespace WMMAPI.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Delete(Guid id);
        void Update(TEntity entity);
    }
}
