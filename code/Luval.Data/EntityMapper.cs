using Luval.Data.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Luval.Data
{
    public static class EntityMapper
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _mappedValues = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        private static readonly Dictionary<Type, EntityMetadata> _entityMetadata = new Dictionary<Type, EntityMetadata>();

        public static T FromDataRecord<T>(IDataRecord record)
        {
            return (T)Convert.ChangeType(FromDataRecord(record, typeof(T)), typeof(T));
        }

        public static object FromDataRecord(IDataRecord record, Type entityType)
        {
            var entity = Activator.CreateInstance(entityType);
            for (int i = 0; i < record.FieldCount; i++)
            {
                AssignFieldValueToEntity(record.GetName(i), ref entity, record.GetValue(i));
            }
            return entity;
        }

        public static void AssignFieldValueToEntity(string fieldName, ref object entity, object value)
        {
            var p = GetEntityPropertyFromFieldName(fieldName, entity.GetType());
            if (p == null) return;
            if (DBNull.Value == value || value == null) value = GetDefaultValue(p.PropertyType);
            var typeToConvert = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            p.SetValue(entity, TryChangeType(value, typeToConvert));
        }

        private static object TryChangeType(object val, Type type)
        {
            try
            {
                val = Convert.ChangeType(val, type);
            }
            catch (InvalidCastException)
            {
                if (val != null && (typeof(Guid) == val.GetType()))
                    val = ((Guid)val).ToString();
            }
            catch (Exception)
            {
            }
            return val;
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        internal static PropertyInfo GetEntityPropertyFromFieldName(string name, Type type)
        {
            if (_mappedValues.ContainsKey(type) && _mappedValues[type].ContainsKey(name))
                return _mappedValues[type][name];

            PropertyInfo property;
            property = type.GetProperty(name);
            if (property == null)
            {
                foreach (var prop in type.GetProperties())
                {
                    var columnName = prop.GetCustomAttribute<ColumnNameAttribute>();
                    if (columnName == null) continue;
                    if (((ColumnNameAttribute)columnName).Name == name)
                    {
                        property = prop;
                        break;
                    }
                }
                if (property != null && property.GetCustomAttribute<NotMappedAttribute>() != null)
                    property = null;
            }
            else
            {
                if (property.GetCustomAttribute<NotMappedAttribute>() != null)
                    property = null;
            }
            CreateOrUpdateMapEntry(type, name, property);
            return property;
        }

        private static void CreateOrUpdateMapEntry(Type type, string filedName, PropertyInfo property)
        {
            if(!_mappedValues.ContainsKey(type))
            {
                _mappedValues.Add(type, new Dictionary<string, PropertyInfo>());
                if (!_mappedValues[type].ContainsKey(filedName))
                    _mappedValues[type].Add(filedName, property);
            }
            _mappedValues[type][filedName] = property;
        }

        public static IDataRecord ToDataRecord(object entity)
        {
            return new DictionaryDataRecord(ToDictionary(entity));
        }

        private static EntityMetadata GetEntityMetadata(Type entityType)
        {
            if (_entityMetadata.ContainsKey(entityType)) return _entityMetadata[entityType];

            var metaData = new EntityMetadata(entityType);
            foreach (var property in entityType.GetProperties())
            {
                var field = new EntityFieldMetadata
                {
                    Property = property,
                    IsMapped = !(property.GetCustomAttribute<NotMappedAttribute>() != null),
                    IsPrimitive = ObjectExtensions.IsPrimitiveType(property.PropertyType)
                };
                var colAtt = property.GetCustomAttribute<ColumnNameAttribute>();
                field.DataFieldName = colAtt != null ? colAtt.Name : property.Name;
                if (!field.IsPrimitive)
                {
                    field.IsList = typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
                    //var refTable = property.GetCustomAttribute<TableReferenceAttribute>();
                    //if (refTable == null) refTable = new TableReferenceAttribute();
                    field.TableReference = TableReference.Create(property);
                    SqlTableSchema.ValidateTableRef(field.TableReference, SqlTableSchema.Create(entityType));
                }
                metaData.Fields.Add(field);
            }
            _entityMetadata[entityType] = metaData;
            return metaData;
        }

        public static Dictionary<string, object> ToDictionary(object entity)
        {
            var metaData = GetEntityMetadata(entity.GetType());
            var record = new Dictionary<string, object>();
            foreach (var field in metaData.Fields)
            {
                if (!field.IsMapped) continue;
                var value = field.Property.GetValue(entity);
                if (field.IsPrimitive)
                    record[field.DataFieldName] = value;
                else
                {
                    if (field.IsList)
                    {
                        var list = new List<IDataRecord>();
                        foreach (var item in (IEnumerable)value)
                            list.Add(ToDataRecord(item));
                        record[field.DataFieldName] = list;
                    }
                    else
                    {
                        if (field.TableReference != null && !record.ContainsKey(field.TableReference.ReferenceTableKey))
                        {
                            var parentMetaData = GetEntityMetadata(field.TableReference.EntityType);
                            var parentField = parentMetaData.Fields.FirstOrDefault(i => i.DataFieldName == field.TableReference.ReferenceTable.Columns.Where(c => c.IsPrimaryKey).First().ColumnName);
                            if (parentField != null)
                                record[field.TableReference.ReferenceTableKey] = parentField.Property.GetValue(value);
                        }
                    }
                }
            }
            return record;
        }

        private static void ValidateTableReference(TableReferenceAttribute tableReference, PropertyInfo property)
        {
            if (string.IsNullOrWhiteSpace(tableReference.ReferenceTableKey))
                tableReference.ReferenceTableKey = SqlTableSchema.GetTableName(property.PropertyType).Name + "Id";
            if (string.IsNullOrWhiteSpace(tableReference.ParentColumnKey))
                tableReference.ParentColumnKey = "Id";
        }

    }
}
