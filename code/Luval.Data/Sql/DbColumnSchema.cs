using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Luval.Data.Sql
{
    /// <summary>
    /// Provides an abstraction for the schema of a sql column
    /// </summary>
    public class DbColumnSchema
    {
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }

        public static DbColumnSchema Create(PropertyInfo property)
        {
            return new DbColumnSchema()
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
