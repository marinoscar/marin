using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data.Interfaces
{
    public interface IEntityAdapter<TEntity, TKey> where TEntity : class
    {
        int Insert(TEntity entity);
        Task<int> InsertAsync(TEntity entity);
        Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken);

        int Update(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        int Delete(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
        Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        int Delete(TKey key);
        Task<int> DeleteAsync(TKey key);
        Task<int> DeleteAsync(TKey key, CancellationToken cancellationToken);


        TEntity Read(TKey key);
        Task<TEntity> ReadAsync(TKey key);
        Task<TEntity> ReadAsync(TKey key, CancellationToken cancellationToken);

        TEntity Read(TKey key, EntityLoadMode mode);
        Task<TEntity> ReadAsync(TKey key, EntityLoadMode mode);
        Task<TEntity> ReadAsync(TKey key, EntityLoadMode mode, CancellationToken cancellationToken);

        IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> whereExpression);

        Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> whereExpression);
    }

    public abstract class EntityAdapter<TEntity, TKey> : IEntityAdapter<TEntity, TKey> where TEntity : class
    {
        #region Insert
        public abstract int Insert(TEntity entity);
        public Task<int> InsertAsync(TEntity entity)
        {
            return InsertAsync(entity, CancellationToken.None);
        }

        public Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Insert(entity); }, cancellationToken);
        }
        #endregion

        #region Update
        public abstract int Update(TEntity entity);
        public Task<int> UpdateAsync(TEntity entity)
        {
            return UpdateAsync(entity, CancellationToken.None);
        }

        public Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Update(entity); }, cancellationToken);
        }
        #endregion

        #region Delete
        public abstract int Delete(TEntity entity);
        public Task<int> DeleteAsync(TEntity entity)
        {
            return DeleteAsync(entity, CancellationToken.None);
        }

        public Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Delete(entity); }, cancellationToken);
        }

        public abstract int Delete(TKey key);
        public Task<int> DeleteAsync(TKey key)
        {
            return DeleteAsync(key, CancellationToken.None);
        }

        public Task<int> DeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Delete(key); }, cancellationToken);
        }
        #endregion

        #region Read
        public abstract TEntity Read(TKey key);

        public Task<TEntity> ReadAsync(TKey key)
        {
            return ReadAsync(key, CancellationToken.None);
        }
        public Task<TEntity> ReadAsync(TKey key, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(key); }, cancellationToken);
        }

        public abstract TEntity Read(TKey key, EntityLoadMode mode);

        public Task<TEntity> ReadAsync(TKey key, EntityLoadMode mode)
        {
            return ReadAsync(key, mode, CancellationToken.None);
        }
        public Task<TEntity> ReadAsync(TKey key, EntityLoadMode mode, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(key, mode); }, cancellationToken);
        }


        public abstract IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> whereExpression);
        
        public Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(whereExpression); }, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Task.Run(() => { return Read(whereExpression); }, CancellationToken.None);
        }



        #endregion
    }
}
