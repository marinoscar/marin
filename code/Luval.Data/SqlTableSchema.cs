using Luval.Data.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public class SqlTableSchema
    {

        private static Dictionary<Type, SqlTableSchema> _cache = new Dictionary<Type, SqlTableSchema>();
        public TableName TableName { get; set; }
        public Type EntityType { get; set; }
        public List<SqlColumnSchema> Columns { get; set; }

        public List<TableReference> References { get; set; }

        public static SqlTableSchema Create(Type type)
        {
            if (_cache.ContainsKey(type)) return _cache[type];
            var columns = new List<SqlColumnSchema>();
            var refs = new List<TableReference>();
            var res = new SqlTableSchema() { TableName = GetTableName(type), Columns = columns, References = refs, EntityType = type };
            _cache[type] = res;

            foreach (var prop in type.GetProperties())
            {
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                if (prop.GetCustomAttribute<TableReferenceAttribute>() != null)
                {
                    var entityType = GetReferenceTableEntityType(prop);
                    var tableRef = new TableReference()
                    {
                        ReferenceTableKey = prop.GetCustomAttribute<TableReferenceAttribute>().ReferenceColumnKey,
                        EntityType = entityType,
                        IsChild = typeof(IEnumerable).IsAssignableFrom(prop.PropertyType),
                        ReferenceTable = SqlTableSchema.Create(entityType)
                    };
                    tableRef.SourceColumn = SqlColumnSchema.Create(prop);
                    ValidateTableRef(tableRef, res);
                    refs.Add(tableRef);
                    continue;
                }
                columns.Add(SqlColumnSchema.Create(prop));
            }
            if (!res.Columns.Any(i => i.IsPrimaryKey) && res.Columns.Any(i => i.ColumnName == "Id"))
                res.Columns.Single(i => i.ColumnName == "Id").IsPrimaryKey = true;
            
            return res;
        }

        private static void ValidateTableRef(TableReference tableReference, SqlTableSchema parent)
        {
            if (!string.IsNullOrWhiteSpace(tableReference.ReferenceTableKey)) return;

            tableReference.ReferenceTableKey = tableReference.IsChild ?
                string.Format("{0}Id", parent.TableName.Name) :
                string.Format("{0}Id", tableReference.ReferenceTable.TableName.Name);
        }

        private static Type GetReferenceTableEntityType(PropertyInfo property)
        {
            return property.PropertyType.IsGenericType ?
                property.PropertyType.GetGenericArguments()[0] :
                property.PropertyType;
        }

        public static TableName GetTableName(Type type)
        {
            var att = type.GetCustomAttribute<TableNameAttribute>();
            if (att == null) return new TableName(type.Name);
            return att.TableName;
        }

    }
}
