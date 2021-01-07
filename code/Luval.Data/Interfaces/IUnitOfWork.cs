using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data.Interfaces
{
    public interface IUnitOfWork<TEntity, TKey>
    {
        IEntityCollection<TEntity, TKey> Entities { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
