using Luval.Data.Attributes;
using System;
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
    }
}
