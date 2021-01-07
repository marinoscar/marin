using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public class SqlEntityAdapterFactory : IEntityAdapterFactory
    {
        public SqlEntityAdapterFactory(Database database, DbDialectProvider sqlDialectFactory)
        {
            SqlDialectFactory = sqlDialectFactory;
            Database = database;
        }

        protected Database Database { get; private set; }
        protected DbDialectProvider SqlDialectFactory { get; private set; }

        public IEntityAdapter<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return new SqlEntityAdapter<TEntity, TKey>(Database, SqlDialectFactory);
        }
    }
}
