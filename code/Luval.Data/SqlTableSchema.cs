using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public class SqlTableSchema
    {
        public string TableName { get; set; }
        public List<SqlColumnSchema> Columns { get; set; }

        public static SqlTableSchema Load(Type type)
        {
            var columns = new List<SqlColumnSchema>();
            foreach (var prop in type.GetProperties())
            {
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                columns.Add(SqlColumnSchema.Load(prop));
            }
            return new SqlTableSchema() { TableName = GetTableName(type), Columns = columns };
        }

        private static string GetTableName(Type type)
        {
            var att = type.GetCustomAttribute<TableNameAttribute>();
            if (att == null) return type.Name;
            return att.Name;
        }
    }
}
