using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public static class EntityLoader
    {
        //public static T FromDataRecord<T>(IDataRecord record)
        //{
        //    var type = typeof(T);
        //    var entity = Activator.CreateInstance(type);
        //    for (int i = 0; i < record.FieldCount; i++)
        //    {
        //        //var p = GetEntityPropertyFromFieldName(record.GetName(i), type);
        //        //if (p == null) continue;
        //        //object val = record.GetValue(i);
        //        //if (record.IsDBNull(i)) val = GetDefaultValue(p.PropertyType);
        //        //var typeToConvert = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        //        //p.SetValue(entity, TryChangeType(val, typeToConvert));
        //        AssignFieldValueToEntity(record.GetName(i), ref entity, record.GetValue(i));
        //    }
        //    return ((T)entity);
        //}

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
            catch (InvalidCastException inv)
            {
                if (val != null && (typeof(Guid) == val.GetType()))
                    val = ((Guid)val).ToString();
            }
            catch (Exception ex)
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
            var property = type.GetProperty(name);
            if (property != null)
            {
                if (property.GetCustomAttribute<NotMappedAttribute>() != null) return null;
                return property;
            }
            foreach (var prop in type.GetProperties())
            {
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                var att = prop.GetCustomAttribute<ColumnNameAttribute>();
                if (att == null) continue;
                if (((ColumnNameAttribute)att).Name == name) return prop;
            }
            return null;
        }
    }
}
