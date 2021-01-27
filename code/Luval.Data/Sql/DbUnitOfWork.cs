using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public class DbUnitOfWork<TEntity, TKey> : UnitOfWork<TEntity, TKey>
    {
        private IEntityCollection<TEntity, TKey> _entities;

        public DbUnitOfWork(Database database, IDbDialectProvider sqlDialectProvider)
        {
            _entities = new DbEntityCollection<TEntity, TKey>(database, sqlDialectProvider);
            Database = database;
            SqlDialectProvider = sqlDialectProvider;
        }


        public override IEntityCollection<TEntity, TKey> Entities { get { return _entities; } }
        protected IDbDialectProvider SqlDialectProvider { get; private set; }
        protected Database Database { get; private set; }

        public override int SaveChanges()
        {
            var commands = new List<string>();

            foreach (var item in Entities.GetAdded())
                commands.Add(SqlDialectProvider.GetCreateCommand(EntityMapper.ToDataRecord(item), true));

            foreach (var item in Entities.GetModified())
                commands.Add(SqlDialectProvider.GetUpdateCommand(EntityMapper.ToDataRecord(item)));

            foreach (var item in Entities.GetRemoved())
                commands.Add(SqlDialectProvider.GetDeleteCommand(EntityMapper.ToDataRecord(item)));

            Entities.Clear();
            return Database.ExecuteNonQuery(string.Join(Environment.NewLine, commands));
        }
    }
}
