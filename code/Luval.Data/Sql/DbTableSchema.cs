using Luval.Data.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Luval.Data.Sql
{
    public class DbTableSchema
    {

        private static Dictionary<Type, DbTableSchema> _cache = new Dictionary<Type, DbTableSchema>();
        public TableName TableName { get; set; }
        public Type EntityType { get; set; }
        public List<DbColumnSchema> Columns { get; set; }

        public List<TableReference> References { get; set; }

        public static DbTableSchema Create(Type type)
        {
            if (_cache.ContainsKey(type)) return _cache[type];
            var columns = new List<DbColumnSchema>();
            var refs = new List<TableReference>();
            var res = new DbTableSchema() { TableName = GetTableName(type), Columns = columns, References = refs, EntityType = type };
            _cache[type] = res;

            foreach (var prop in type.GetProperties())
            {
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                if (prop.GetCustomAttribute<TableReferenceAttribute>() != null)
                {
                    var tableRef = TableReference.Create(prop);
                    ValidateTableRef(tableRef, res);
                    refs.Add(tableRef);
                    continue;
                }
                columns.Add(DbColumnSchema.Create(prop));
            }
            if (!res.Columns.Any(i => i.IsPrimaryKey) && res.Columns.Any(i => i.ColumnName == "Id"))
                res.Columns.Single(i => i.ColumnName == "Id").IsPrimaryKey = true;
            
            return res;
        }

        public static void ValidateTableRef(TableReference tableReference, DbTableSchema parent)
        {
            if (!string.IsNullOrWhiteSpace(tableReference.ReferenceTableKey)) return;

            tableReference.ReferenceTableKey = tableReference.IsChild ?
                string.Format("{0}Id", parent.TableName.Name) :
                string.Format("{0}Id", tableReference.ReferenceTable.TableName.Name);
        }


        

        public static TableName GetTableName(Type type)
        {
            var att = type.GetCustomAttribute<TableNameAttribute>();
            if (att == null) return new TableName(type.Name);
            return att.TableName;
        }

    }
}
