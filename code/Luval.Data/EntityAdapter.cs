using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Luval.Data
{
    public class EntityAdapter
    {
        public EntityAdapter(Database database, SqlDialectFactory dialectFactory, SqlTableSchema sqlTableSchema)
        {
            Database = database;
            DialectFactory = dialectFactory;
            Schema = sqlTableSchema;
            PrimaryKey = Schema.Columns.Single(i => i.IsPrimaryKey);
        }

        public SqlDialectFactory DialectFactory { get; private set; }
        public Database Database { get; private set; }
        public SqlTableSchema Schema { get; private set; }
        public SqlColumnSchema PrimaryKey { get; private set; }

        protected ISqlDialectProvider GetProvider()
        {
            return DialectFactory.Create(Schema);
        }

        private int DoAction(IDataRecord record, Func<IDataRecord, string> action)
        {
            return Database.ExecuteNonQuery(action(record));
        }

        public int Insert(IDataRecord record)
        {
            return DoAction(record, GetProvider().GetCreateCommand);
        }


        public int Update(IDataRecord record)
        {
            return DoAction(record, GetProvider().GetUpdateCommand);
        }

        public int Delete(IDataRecord record)
        {
            return DoAction(record, GetProvider().GetDeleteCommand);
        }

        public IDataRecord Read(IDataRecord record)
        {
            return Database.ExecuteToDictionaryList(GetProvider().GetReadCommand(record)).Select(i => new DictionaryDataRecord(i)).SingleOrDefault();
        }


        public int ExecuteInTransaction(IEnumerable<DataRecordAction> records)
        {
            var count = 0;
            Database.WithCommand((cmd) => {
                cmd.CommandTimeout = cmd.Connection.ConnectionTimeout;
                foreach(var record in records)
                {
                    switch (record.Action)
                    {
                        case DataAction.Insert:
                            cmd.CommandText = GetProvider().GetCreateCommand(record.Record);
                            count += cmd.ExecuteNonQuery();
                            break;
                        case DataAction.Update:
                            cmd.CommandText = GetProvider().GetUpdateCommand(record.Record);
                            count += cmd.ExecuteNonQuery();
                            break;
                        case DataAction.Delete:
                            cmd.CommandText = GetProvider().GetDeleteCommand(record.Record);
                            count += cmd.ExecuteNonQuery();
                            break;
                    }
                }
            });
            return count;
        }

        protected virtual IDataRecord WithPrimaryKeyOnly(object key)
        {
            return new DictionaryDataRecord(new Dictionary<string, object>() { { PrimaryKey.ColumnName, key } });
        }

    }

    public class EntityAdapter<TEntity, TKey> : EntityAdapter
    {
        public EntityAdapter(Database database, SqlDialectFactory dialectFactory) : base(database, dialectFactory, SqlTableSchema.Create(typeof(TEntity)))
        {

        }

        public int Insert(TEntity entity)
        {
            return Insert(DictionaryDataRecord.FromEntity(entity));
        }

        public int Update(TEntity entity)
        {
            return Update(DictionaryDataRecord.FromEntity(entity));
        }

        public int Delete(TEntity entity)
        {
            return Delete(DictionaryDataRecord.FromEntity(entity));
        }

        public int Delete(TKey key)
        {
            return Delete(WithPrimaryKeyOnly(key));
        }

        public TEntity Read(TKey key)
        {
            return Read(WithPrimaryKeyOnly(key), EntityLoadMode.Lazy);
        }

        public TEntity Read(TKey key, EntityLoadMode mode)
        {
            return Read(WithPrimaryKeyOnly(key), mode);
        }

        public TEntity Read(TEntity entity)
        {
            return Read(DictionaryDataRecord.FromEntity(entity), EntityLoadMode.Lazy);
        }

        public TEntity Read(TEntity entity, EntityLoadMode mode)
        {
            return Read(DictionaryDataRecord.FromEntity(entity), mode);
        }

        public TEntity Read(IDataRecord record, EntityLoadMode mode)
        {
            record = Database.ExecuteToDictionaryList(GetProvider().GetReadCommand(record)).Select(i => new DictionaryDataRecord(i)).SingleOrDefault();
            var entity = EntityLoader.FromDataRecord<TEntity>(record);
            if (mode == EntityLoadMode.Lazy) return entity;
            foreach (var tableRef in Schema.References)
            {
                var prop = typeof(TEntity).GetProperty(tableRef.SourceColumn.PropertyName);

                var propertyValue = tableRef.IsList ?
                    ((IEnumerable)GetChildReference(tableRef, Schema, record)).ToList(tableRef.EntityType)
                    : GetParentReference(tableRef, Schema, record).FirstOrDefault();

                prop.SetValue(entity, propertyValue);
            }
            return entity;
        }

        private List<object> GetChildReference(TableReference tableRef, SqlTableSchema parentTable, IDataRecord record)
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}",
                tableRef.ReferenceTable.TableName.GetFullTableName(),
                tableRef.ReferenceTableKey,
                record[parentTable.Columns.Single(i => i.IsPrimaryKey).ColumnName].ToSql());
            return Database.ExecuteToEntityList(sql, CommandType.Text, null, tableRef.EntityType);
        }

        private List<object> GetParentReference(TableReference tableRef, SqlTableSchema parentTable, IDataRecord record)
        {
            var parentPkName = parentTable.Columns.Single(i => i.IsPrimaryKey).ColumnName;
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}",
                tableRef.ReferenceTable.TableName.GetFullTableName(),
                parentPkName,
                record[tableRef.ReferenceTableKey].ToSql());
            return Database.ExecuteToEntityList(sql, CommandType.Text, null, tableRef.EntityType);
        }


    }

    public class EntityAdapter<TEntity> : EntityAdapter<TEntity, string>
    {
        public EntityAdapter(Database database, SqlDialectFactory dialectFactory) : base(database, dialectFactory)
        {

        }
    }
}
