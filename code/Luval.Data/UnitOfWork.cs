using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data
{
    public abstract class UnitOfWork<TEntity, TKey> : IUnitOfWork<TEntity, TKey>
    {

        public abstract IEntityCollection<TEntity, TKey> Entities { get; }

        public abstract int SaveChanges();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => { return SaveChanges(); }, cancellationToken);
        }
    }
}
