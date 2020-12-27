using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    /// <summary>
    /// Provides an abstraction for the schema of a sql column
    /// </summary>
    public class SqlColumnSchema
    {
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }

        public static SqlColumnSchema Create(PropertyInfo property)
        {
            return new SqlColumnSchema()
            {
                ColumnName = GetColumnName(property),
                PropertyName = property.Name,
                IsPrimaryKey = property.GetCustomAttribute<PrimaryKeyAttribute>() != null,
                IsIdentity = property.GetCustomAttribute<IdentityColumnAttribute>() != null
            };
        }

        internal static string GetColumnName(PropertyInfo property)
        {
            var att = property.GetCustomAttribute<ColumnNameAttribute>();
            if (att == null) return property.Name;
            return ((ColumnNameAttribute)att).Name;
        }
    }
}
