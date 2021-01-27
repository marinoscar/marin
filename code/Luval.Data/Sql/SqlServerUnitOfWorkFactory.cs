using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public class SqlServerUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public SqlServerUnitOfWorkFactory(string sqlServerConnectionString) : this(sqlServerConnectionString, new ReflectionDataRecordMapper())
        {

        }
        public SqlServerUnitOfWorkFactory(string sqlServerConnectionString, IDataRecordMapper dataRecordMapper)
        {
            Database = new SqlServerDatabase(sqlServerConnectionString, dataRecordMapper);
            ProviderFactory = new SqlServerDialectFactory();
        }

        public Database Database { get; private set; }
        protected DbDialectProvider ProviderFactory { get; private set; }

        public IUnitOfWork<TEntity, TKey> Create<TEntity, TKey>()
        {
            return new DbUnitOfWork<TEntity, TKey>(Database, ProviderFactory.Create(DbTableSchema.Create(typeof(TEntity))));
        }
    }
}
