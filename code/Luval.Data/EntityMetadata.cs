using Luval.Data.Attributes;
using Luval.Data.Sql;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public class EntityMetadata
    {
        public EntityMetadata(Type entityTpe) : this(entityTpe, new EntityFieldMetadata[] { })
        {

        }
        public EntityMetadata(Type entityType, IEnumerable<EntityFieldMetadata> entityFields)
        {
            EntityType = entityType;
            Fields = new List<EntityFieldMetadata>(entityFields);
        }
        public Type EntityType { get; private set; }
        public List<EntityFieldMetadata> Fields { get; private set; }
    }

    public class EntityFieldMetadata
    {
        public PropertyInfo Property { get; set; }
        public bool IsMapped { get; set; }
        public string DataFieldName { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsList { get; set; }

        public TableReference TableReference { get; set; }
    }

}
