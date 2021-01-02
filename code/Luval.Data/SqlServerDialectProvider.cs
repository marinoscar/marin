using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public class SqlServerDialectProvider : ISqlDialectProvider
    {
        public SqlServerDialectProvider(SqlTableSchema schema)
        {
            Schema = schema;
        }

        public SqlTableSchema Schema { get; private set; }

        public string GetCreateCommand(IDataRecord record, bool includeChildren)
        {
            return GetCreateCommand(Schema, record, includeChildren);
        }

        private string GetCreateCommand(SqlTableSchema schema, IDataRecord record, bool includeChildren)
        {
            var sw = new StringWriter();
            sw.WriteLine("INSERT INTO {1} ({0}) VALUES ({2});",
                string.Join(", ", GetSqlFormattedColumnNames(schema, (i) => !i.IsIdentity)),
                GetSqlFormattedTableName(schema),
                string.Join(", ", GetSqlInserValues(schema, record)));
            if (includeChildren)
                foreach (var refTable in schema.References.Where(i => i.IsChild))
                {
                    GetCreateCommandForChildren(sw, refTable, record);
                }
            return sw.ToString();
        }

        public void GetCreateCommandForChildren(StringWriter sw, TableReference reference, IDataRecord record)
        {
            var value = record[reference.SourceColumn.PropertyName];
            if (value.IsPrimitiveType() ||
                !(typeof(IEnumerable<IDataRecord>).IsAssignableFrom(value.GetType()) || typeof(IDataRecord).IsAssignableFrom(value.GetType()))) return;
            if (reference.IsChild)
                foreach (var item in (IEnumerable)value)
                    sw.Write(GetCreateCommand(reference.ReferenceTable, (IDataRecord)item, true));
            else
                sw.Write(GetCreateCommand(reference.ReferenceTable, (IDataRecord)value, true));
        }

        public string GetDeleteCommand(IDataRecord record)
        {
            var sw = new StringWriter();
            sw.WriteLine("DELETE FROM {0} WHERE {1};", GetSqlFormattedTableName(),
                string.Join(" AND ", GetKeyWhereStatement(record)));
            return sw.ToString();
        }

        public string GetUpdateCommand(IDataRecord record)
        {
            var sw = new StringWriter();
            sw.WriteLine("UPDATE {0} SET {1} WHERE {2};", GetSqlFormattedTableName(),
                string.Join(", ", GetUpdateValueStatement(record)),
                string.Join(" AND ", GetKeyWhereStatement(record)));
            return sw.ToString();
        }

        public string GetReadCommand(IDataRecord record)
        {
            var sw = new StringWriter();
            sw.WriteLine("SELECT {0} FROM {1} WHERE {2};",
                string.Join(", ", GetSqlFormattedColumnNames((i) => true)),
                GetSqlFormattedTableName(),
                string.Join(" AND ", GetKeyWhereStatement(record)));
            return sw.ToString();
        }

        public string GetReadAllCommand()
        {
            var sw = new StringWriter();
            sw.WriteLine("SELECT {0} FROM {1};",
                string.Join(", ", GetSqlFormattedColumnNames((i) => true)),
                GetSqlFormattedTableName());
            return sw.ToString();
        }

        public string GetEntityQuery<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetUpdateValueStatement(IDataRecord record)
        {
            return GetColumnValuePair(record, i => !i.IsPrimaryKey && !i.IsIdentity).Select(i =>
            {
                if (i.Contains("IS NULL"))
                    i = i.Replace("IS NULL", "= NULL");
                return i;
            });
        }

        private IEnumerable<string> GetKeyWhereStatement(IDataRecord record)
        {
            return GetKeyWhereStatement(Schema, record);
        }


        private IEnumerable<string> GetKeyWhereStatement(SqlTableSchema schema, IDataRecord record)
        {
            if (!GetColumns(schema).Any(i => i.IsPrimaryKey))
                throw new InvalidDataException("At least one primary key column is required");
            return GetColumnValuePair(schema, record, i => i.IsPrimaryKey);
        }

        private IEnumerable<string> GetColumnValuePair(IDataRecord record, Func<SqlColumnSchema, bool> predicate)
        {
            return GetColumnValuePair(Schema, record, predicate);
        }

        private IEnumerable<string> GetColumnValuePair(SqlTableSchema schema, IDataRecord record, Func<SqlColumnSchema, bool> predicate)
        {
            return GetColumns(schema).Where(predicate)
                .Select(i =>
                {
                    var val = record[i.ColumnName];
                    var res = string.Format("{0} = {1}", GetSqlFormattedColumnName(i), val.ToSql());
                    if (val.IsNullOrDbNull()) res = string.Format("{0} IS NULL", GetSqlFormattedColumnName(i));
                    return res;
                }).ToList();
        }

        private IEnumerable<string> GetSqlInserValues(SqlTableSchema schema, IDataRecord record)
        {
            return GetEntityValues(schema, record, i => !i.IsIdentity).Select(i => i.ToSql());
        }

        private IEnumerable<object> GetEntityValues(SqlTableSchema schema, IDataRecord record, Func<SqlColumnSchema, bool> predicate)
        {
            var res = new List<object>();
            GetColumns(schema).Where(predicate).ToList().ForEach(i => res.Add(record[i.ColumnName]));
            return res;
        }

        private string GetSqlFormattedTableName() { return GetSqlFormattedTableName(Schema); }

        private string GetSqlFormattedTableName(SqlTableSchema schema)
        {
            return string.Format("{0}", schema.TableName.GetFullTableName());
        }

        private string GetSqlFormattedColumnName(SqlColumnSchema columnSchema)
        {
            return string.Format("[{0}]", columnSchema.ColumnName);
        }


        private IEnumerable<string> GetSqlFormattedColumnNames(Func<SqlColumnSchema, bool> predicate)
        { return GetSqlFormattedColumnNames(this.Schema, predicate); }

        private IEnumerable<string> GetSqlFormattedColumnNames(SqlTableSchema schema, Func<SqlColumnSchema, bool> predicate)
        {
            return GetColumns(schema).Where(predicate).Select(GetSqlFormattedColumnName);
        }

        private IEnumerable<SqlColumnSchema> GetColumns(SqlTableSchema schema)
        {
            var parentReferences = schema.References.Where(i => !i.IsChild).ToList();
            if (parentReferences.Count <= 0) return schema.Columns;
            foreach (var parentRef in parentReferences)
            {
                if (string.IsNullOrWhiteSpace(parentRef.ReferenceTableKey))
                    parentRef.ReferenceTableKey = parentRef.ReferenceTable.TableName.Name + "Id";
                if (!schema.Columns.Any(i => i.ColumnName == parentRef.ReferenceTableKey))
                    schema.Columns.Add(new SqlColumnSchema()
                    {
                        ColumnName = parentRef.ReferenceTableKey,
                        IsIdentity = false,
                        IsPrimaryKey = false
                    });
            }
            return schema.Columns;
        }
    }
}
