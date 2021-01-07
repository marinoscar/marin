using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Luval.Data
{
    public class DbQuery<TEntity, TKey> : EntityQuery<TEntity, TKey>
    {
        public DbQuery(Database database, IDbDialectProvider sqlDialectProvider)
        {
            Database = database;
            Schema = DbTableSchema.Create(typeof(TEntity));
            Provider = sqlDialectProvider;
            PrimaryKey = Schema.Columns.Single(i => i.IsPrimaryKey);
        }

        #region Property Implementation
        
        protected Database Database { get; private set; }
        protected DbTableSchema Schema { get; private set; }
        protected IDbDialectProvider Provider { get; private set; }
        protected DbColumnSchema PrimaryKey { get; private set; } 

        #endregion

        #region Implementation

        public override IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> whereExpression)
        {
            var expressionProvider = new SqlExpressionProvider<TEntity>();
            var whereStatement = expressionProvider.ResolveWhere<TEntity>(whereExpression);
            var sql = string.Format("SELECT * FROM {0} WHERE {1}", Schema.TableName.GetFullTableName(), whereStatement);
            return Database.ExecuteToEntityList<TEntity>(sql);
        }

        public override TEntity Get(TKey key, EntityLoadMode mode)
        {
            var record = EntityMapper.ToDataRecord(CreateByKey(key));
            var entity = Database.ExecuteToEntityList<TEntity>(Provider.GetReadCommand(record)).SingleOrDefault();
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

        #endregion

        #region Helper Methods

        private List<object> GetParentReference(TableReference tableRef, DbTableSchema parentTable, IDataRecord record)
        {
            var parentPkName = parentTable.Columns.Single(i => i.IsPrimaryKey).ColumnName;
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}",
                tableRef.ReferenceTable.TableName.GetFullTableName(),
                parentPkName,
                record[tableRef.ReferenceTableKey].ToSql());
            return Database.ExecuteToEntityList(sql, CommandType.Text, null, tableRef.EntityType);
        }

        private List<object> GetChildReference(TableReference tableRef, DbTableSchema parentTable, IDataRecord record)
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = {2}",
                tableRef.ReferenceTable.TableName.GetFullTableName(),
                tableRef.ReferenceTableKey,
                record[parentTable.Columns.Single(i => i.IsPrimaryKey).ColumnName].ToSql());
            return Database.ExecuteToEntityList(sql, CommandType.Text, null, tableRef.EntityType);
        }

        protected virtual IDataRecord CreateByKey(object key)
        {
            return new DictionaryDataRecord(new Dictionary<string, object>() { { PrimaryKey.ColumnName, key } });
        } 

        #endregion
    }
}
