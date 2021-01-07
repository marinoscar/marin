using Luval.Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Luval.Data
{
    public abstract class EntityCollection<TEntity, TKey> : EntityQuery<TEntity, TKey>, IEntityCollection<TEntity, TKey>
    {
        private readonly List<EntityItem> _internal = new List<EntityItem>();
        
        private class EntityItem { public TEntity Entity { get; set; } public EntityState State { get; set; } }

        public int Count { get { return _internal.Count; } }

        public bool IsReadOnly => false;

        public void Add(TEntity item)
        {
            _internal.Add(new EntityItem() { Entity = item, State = EntityState.New });
        }

        public void Clear()
        {
            _internal.Clear();
        }

        public bool Contains(TEntity item)
        {
            return _internal.Select(i => i.Entity).Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            _internal.Select(i => i.Entity).ToList().CopyTo(array, arrayIndex);
        }

        public IEnumerable<TEntity> GetAdded()
        {
            return _internal.Where(i => i.State == EntityState.New).Select(i => i.Entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _internal.Where(i => i.State != EntityState.Deleted).Select(i => i.Entity).GetEnumerator();
        }

        public IEnumerable<TEntity> GetModified()
        {
            return _internal.Where(i => i.State == EntityState.Modified).Select(i => i.Entity);
        }

        public IEnumerable<TEntity> GetRemoved()
        {
            return _internal.Where(i => i.State == EntityState.Deleted).Select(i => i.Entity);
        }

        public bool Remove(TEntity item)
        {
            _internal.Add(new EntityItem() { Entity = item, State = EntityState.Deleted });
            return true;
        }

        public void Update(TEntity item)
        {
            _internal.Add(new EntityItem() { Entity = item, State = EntityState.Modified });
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEntityCollection ToEntityCollection()
        {
            return new EntityCollection(GetAdded().Cast<object>(), GetModified().Cast<object>(), GetRemoved().Cast<object>());
        }
    }

    public class EntityCollection : IEntityCollection
    {
        internal EntityCollection(IEnumerable<object> added, IEnumerable<object> modified, IEnumerable<object> removed)
        {
            Added = added; Modified = modified; Removed = removed;
        }

        public IEnumerable<object> Added { get; private set; }

        public IEnumerable<object> Modified { get; private set; }

        public IEnumerable<object> Removed { get; private set; }
    }
}
