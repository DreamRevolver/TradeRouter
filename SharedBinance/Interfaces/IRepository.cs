using System;
using System.Collections.Generic;
namespace SharedBinance.interfaces
{
    public enum EntityAction
    {
        Added,
        Updated,
        Deleted,
    }

    public interface IRepo<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> GetMany(Func<TEntity, bool> predicate);

        TEntity FirstOrDefault(Func<TEntity, bool> predicate);

        void Add(TEntity entity);

        void Remove(TEntity entity);

        void Update(TEntity entity);

        event Action<TEntity, EntityAction> Action;
    }
}
