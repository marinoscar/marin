using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data.Interfaces
{
    public interface IQuery<TEntity, TKey>
    {
        TEntity Get(TKey key);
        TEntity Get(TKey key, EntityLoadMode mode);
        Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken);
        Task<TEntity> GetAsync(TKey key, EntityLoadMode mode, CancellationToken cancellationToken);

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> whereExpression);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken);

        IEnumerable<TEntity> Get(IQueryCommand queryCommand);
        Task<IEnumerable<TEntity>> GetAsync(IQueryCommand queryCommand, CancellationToken cancellationToken);

        IEnumerable<IDictionary<string, object>> GetRaw(IQueryCommand queryCommand);
        Task<IEnumerable<IDictionary<string, object>>> GetRawAsync(IQueryCommand queryCommand, CancellationToken cancellationToken);

    }
}
