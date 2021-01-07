using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data
{
    public abstract class EntityQuery<TEntity, TKey> : IQuery<TEntity, TKey>
    {
        public TEntity Get(TKey key)
        {
            return Get(key, EntityLoadMode.Lazy);
        }

        public abstract TEntity Get(TKey key, EntityLoadMode mode);

        public abstract IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> whereExpression);

        public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Get(whereExpression); }, cancellationToken);
        }

        public Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken)
        {
            return GetAsync(key, EntityLoadMode.Lazy, cancellationToken);
        }

        public Task<TEntity> GetAsync(TKey key, EntityLoadMode mode, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Get(key, mode); }, cancellationToken);
        }
    }
}
