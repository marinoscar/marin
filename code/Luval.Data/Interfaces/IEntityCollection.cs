using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface IEntityCollection<TEntity, TKey> : ICollection<TEntity>, IQuery<TEntity, TKey>
    {
        void Update(TEntity item);
        IEnumerable<TEntity> GetAdded();
        IEnumerable<TEntity> GetModified();
        IEnumerable<TEntity> GetRemoved();
        IEntityCollection ToEntityCollection();

    }

    public interface IEntityCollection
    {
        IEnumerable<object> Added { get; }
        IEnumerable<object> Modified { get; }
        IEnumerable<object> Removed { get; }
    }
}
