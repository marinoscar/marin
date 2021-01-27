using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public class DbUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public Database Database { get; private set; }
        protected DbDialectProvider ProviderFactory { get; private set; }

        public DbUnitOfWorkFactory(Database database, DbDialectProvider dialectProviderFactory)
        {
            Database = database; ProviderFactory = dialectProviderFactory;
        }

        public IUnitOfWork<TEntity, TKey> Create<TEntity, TKey>()
        {
            return new DbUnitOfWork<TEntity, TKey>(Database, ProviderFactory.Create(DbTableSchema.Create(typeof(TEntity))));
        }
    }
}
