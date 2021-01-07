using Luval.Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Luval.Data
{
    public class DbEntityCollection<TEntity, TKey> : EntityCollection<TEntity, TKey>
    {
        public DbEntityCollection(Database database, IDbDialectProvider sqlDialectProvider)
        {
            DbQuery = new DbQuery<TEntity, TKey>(database, sqlDialectProvider);
        }

        protected DbQuery<TEntity, TKey> DbQuery { get; private set; }

        public override TEntity Get(TKey key, EntityLoadMode mode)
        {
            return DbQuery.Get(key, mode);
        }

        public override IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> whereExpression)
        {
            return DbQuery.Get(whereExpression);
        }
    }
}
