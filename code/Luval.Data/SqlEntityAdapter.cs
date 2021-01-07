using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data
{
    public class SqlEntityAdapter
    {
        public SqlEntityAdapter(Database database, DbDialectProvider dialectFactory, DbTableSchema sqlTableSchema)
        {
            Database = database;
            DialectFactory = dialectFactory;
            Schema = sqlTableSchema;
            PrimaryKey = Schema.Columns.Single(i => i.IsPrimaryKey);
        }

        public DbDialectProvider DialectFactory { get; private set; }
        public Database Database { get; private set; }
        public DbTableSchema Schema { get; private set; }
        public SqlColumnSchema PrimaryKey { get; private set; }

        protected IDbDialectProvider GetProvider()
        {
            return DialectFactory.Create(Schema);
        }

        private int DoAction(IDataRecord record, Func<IDataRecord, string> action)
        {
            return Database.ExecuteNonQuery(action(record));
        }

        public int Insert(IDataRecord record)
        {
            return DoAction(record, (r) => { return GetProvider().GetCreateCommand(r, false);  });
        }

        public Task<int> InsertAsync(IDataRecord record)
        {
            return Task.Run(() => { return Insert(record); });
        }


        public int Update(IDataRecord record)
        {
            return DoAction(record, GetProvider().GetUpdateCommand);
        }

        public Task<int> UpdateAsync(IDataRecord record)
        {
            return Task.Run(() => { return Update(record); });
        }

        public int Delete(IDataRecord record)
        {
            return DoAction(record, GetProvider().GetDeleteCommand);
        }

        public Task<int> DeleteAsync(IDataRecord record)
        {
            return Task.Run(() => { return Delete(record); });
        }

        public IDataRecord Read(IDataRecord record)
        {
            return Database.ExecuteToDictionaryList(GetProvider().GetReadCommand(record)).Select(i => new DictionaryDataRecord(i)).SingleOrDefault();
        }

        public Task<IDataRecord> ReadAsync(IDataRecord record)
        {
            return Task.Run(() => { return Read(record); });
        }


        protected virtual IDataRecord ReadByKey(object key)
        {
            return new DictionaryDataRecord(new Dictionary<string, object>() { { PrimaryKey.ColumnName, key } });
        }

        public int ExecuteInTransaction(IEnumerable<DataRecordAction> records)
        {
            var count = 0;
            Database.WithCommand((cmd) =>
            {
                cmd.CommandTimeout = cmd.Connection.ConnectionTimeout;
                foreach (var record in records)
                {
                    switch (record.Action)
                    {
                        case DataAction.Insert:
                            cmd.CommandText = GetProvider().GetCreateCommand(record.Record, false);
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

    }

    public class SqlEntityAdapter<TEntity, TKey> : SqlEntityAdapter, IEntityAdapter<TEntity, TKey> where TEntity : class
    {
        public SqlEntityAdapter(Database database, DbDialectProvider dialectFactory) : base(database, dialectFactory, DbTableSchema.Create(typeof(TEntity)))
        {

        }

        public int Insert(TEntity entity)
        {
            return Insert(DictionaryDataRecord.FromEntity(entity));
        }

        public Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Insert(entity); }, cancellationToken);
        }

        public Task<int> InsertAsync(TEntity entity)
        {
            return Task.Run(() => { return Insert(entity); });
        }

        public int Update(TEntity entity)
        {
            return Update(DictionaryDataRecord.FromEntity(entity));
        }

        public Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Update(entity); }, cancellationToken);
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            return Task.Run(() => { return Update(entity); });
        }

        public int Delete(TEntity entity)
        {
            return Delete(DictionaryDataRecord.FromEntity(entity));
        }

        public Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Delete(entity); }, cancellationToken);
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            return Task.Run(() => { return Delete(entity); });
        }

        public int Delete(TKey key)
        {
            return Delete(ReadByKey(key));
        }

        public Task<int> DeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Delete(key); }, cancellationToken);
        }

        public Task<int> DeleteAsync(TKey key)
        {
            return Task.Run(() => { return Delete(key); });
        }

        public TEntity Read(TKey key)
        {
            return Read(ReadByKey(key), EntityLoadMode.Lazy);
        }

        public Task<TEntity> ReadAsync(TKey key, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(key); }, cancellationToken);
        }

        public Task<TEntity> ReadAsync(TKey key)
        {
            return Task.Run(() => { return Read(key); });
        }

        public TEntity Read(TKey key, EntityLoadMode mode)
        {
            return Read(ReadByKey(key), mode);
        }


        public Task<TEntity> ReadAsync(TKey key, EntityLoadMode mode, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(key, mode); }, cancellationToken);
        }

        public Task<TEntity> ReadAsync(TKey key, EntityLoadMode mode)
        {
            return Task.Run(() => { return Read(key, mode); });
        }

        public TEntity Read(TEntity entity)
        {
            return Read(DictionaryDataRecord.FromEntity(entity), EntityLoadMode.Lazy);
        }

        public Task<TEntity> ReadAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(entity); }, cancellationToken);
        }

        public Task<TEntity> ReadAsync(TEntity entity)
        {
            return Task.Run(() => { return Read(entity); });
        }

        public TEntity Read(TEntity entity, EntityLoadMode mode)
        {
            return Read(DictionaryDataRecord.FromEntity(entity), mode);
        }

        public Task<TEntity> ReadAsync(TEntity entity, EntityLoadMode mode, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(entity, mode); }, cancellationToken);
        }

        public Task<TEntity> ReadAsync(TEntity entity, EntityLoadMode mode)
        {
            return Task.Run(() => { return Read(entity, mode); });
        }

        public TEntity Read(IDataRecord record, EntityLoadMode mode)
        {
            record = Database.ExecuteToDictionaryList(GetProvider().GetReadCommand(record)).Select(i => new DictionaryDataRecord(i)).SingleOrDefault();
            var entity = EntityMapper.FromDataRecord<TEntity>(record);
            if (mode == EntityLoadMode.Lazy) return entity;
            foreach (var tableRef in Schema.References)
            {
                var prop = typeof(TEntity).GetProperty(tableRef.SourceColumn.PropertyName);

                var propertyValue = tableRef.IsChild ?
                    ((IEnumerable)GetChildReference(tableRef, Schema, record)).ToList(tableRef.EntityType)
                    : GetParentReference(tableRef, Schema, record).FirstOrDefault();

                prop.SetValue(entity, propertyValue);
            }
            return entity;
        }

        public Task<TEntity> ReadAsync(IDataRecord record, EntityLoadMode mode, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(record, mode); }, cancellationToken);
        }

        public Task<TEntity> ReadAsync(IDataRecord record, EntityLoadMode mode)
        {
            return Task.Run(() => { return Read(record, mode); });
        }

        public IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> whereExpression)
        {
            var expressionProvider = new SqlExpressionProvider<TEntity>();
            var whereStatement = expressionProvider.ResolveWhere<TEntity>(whereExpression);
            var sql = string.Format("SELECT * FROM {0} WHERE {1}", Schema.TableName.GetFullTableName(), whereStatement);
            return Database.ExecuteToEntityList<TEntity>(sql);
        }

        public Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken)
        {
            return Task.Run(() => { return Read(whereExpression); }, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return ReadAsync(whereExpression, CancellationToken.None);
        }

        private List<object> GetChildReference(TableReference tableRef, DbTableSchema parentTable, IDataRecord record)
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}",
                tableRef.ReferenceTable.TableName.GetFullTableName(),
                tableRef.ReferenceTableKey,
                record[parentTable.Columns.Single(i => i.IsPrimaryKey).ColumnName].ToSql());
            return Database.ExecuteToEntityList(sql, CommandType.Text, null, tableRef.EntityType);
        }

        private List<object> GetParentReference(TableReference tableRef, DbTableSchema parentTable, IDataRecord record)
        {
            var parentPkName = parentTable.Columns.Single(i => i.IsPrimaryKey).ColumnName;
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}",
                tableRef.ReferenceTable.TableName.GetFullTableName(),
                parentPkName,
                record[tableRef.ReferenceTableKey].ToSql());
            return Database.ExecuteToEntityList(sql, CommandType.Text, null, tableRef.EntityType);
        }


    }

    public class SqlEntityAdapter<TEntity> : SqlEntityAdapter<TEntity, string>, IEntityAdapter<TEntity, string> where TEntity : class
    {
        public SqlEntityAdapter(Database database, DbDialectProvider dialectFactory) : base(database, dialectFactory)
        {

        }
    }
}
