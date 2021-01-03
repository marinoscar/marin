using Luval.Data.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public class TableReference
    {
        public SqlColumnSchema SourceColumn { get; set; }
        public SqlTableSchema ReferenceTable { get; set; }
        public string ReferenceTableKey { get; set; }

        public Type EntityType { get; set; }
        public bool IsChild { get; set; }

        public static TableReference Create(PropertyInfo prop)
        {
            var entityType = GetReferenceTableEntityType(prop);
            return new TableReference()
            {
                ReferenceTableKey = prop.GetCustomAttribute<TableReferenceAttribute>().ReferenceTableKey,
                EntityType = entityType,
                IsChild = typeof(IEnumerable).IsAssignableFrom(prop.PropertyType),
                ReferenceTable = SqlTableSchema.Create(entityType),
                SourceColumn = SqlColumnSchema.Create(prop)
            };
        }
        private static Type GetReferenceTableEntityType(PropertyInfo property)
        {
            return property.PropertyType.IsGenericType ?
                property.PropertyType.GetGenericArguments()[0] :
                property.PropertyType;
        }
    }
}
