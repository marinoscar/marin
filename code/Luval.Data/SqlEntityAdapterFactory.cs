using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public class SqlEntityAdapterFactory : IEntityAdapterFactory
    {
        public SqlEntityAdapterFactory(Database database, SqlDialectFactory sqlDialectFactory)
        {
            SqlDialectFactory = sqlDialectFactory;
            Database = database;
        }

        protected Database Database { get; private set; }
        protected SqlDialectFactory SqlDialectFactory { get; private set; }

        public IEntityAdapter<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return new SqlEntityAdapter<TEntity, TKey>(Database, SqlDialectFactory);
        }
    }
}
