using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Luval.Data
{
    public class ListDataReader : IDataReader
    {
        private IDataRecord _current;

        public ListDataReader(Type entityType)
        {
            EntityType = entityType;
            Records = new List<IDataRecord>();
        }

        public ListDataReader(Type entityType, IEnumerable<IDataRecord> records) : this(entityType)
        {
            Records = new List<IDataRecord>(records);
        }

        public object this[int i] => _current[i];

        public object this[string name] => _current[name];

        public int Depth { get; internal set; }

        public bool IsClosed { get; internal set; }

        public int RecordsAffected => Records.Count;

        public int FieldCount => _current.FieldCount;

        public List<IDataRecord> Records { get; }

        public Type EntityType { get; }

        public void Close()
        {
            IsClosed = true;
        }

        public void Dispose()
        {
            Records.Clear();
            _current = null;
        }

        public bool GetBoolean(int i)
        {
            return _current.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _current.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _current.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _current.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _current.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return _current.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return _current.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _current.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return _current.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return _current.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return _current.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return _current.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return _current.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _current.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _current.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _current.GetInt64(i);
        }

        public string GetName(int i)
        {
            return _current.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _current.GetOrdinal(name);
        }

        public DataTable GetSchemaTable()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new[] {
                new DataColumn("AllowDBNull", typeof(bool)),
                new DataColumn("ColumnName", typeof(string)),
                new DataColumn("IsAutoIncrement", typeof(bool)),
                new DataColumn("IsIdentity", typeof(bool)),
                new DataColumn("IsKey", typeof(bool)),
                new DataColumn("BaseTableName", typeof(string)),
            });
            foreach (var prop in EntityType.GetProperties())
            {
                if (prop.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                var dr = dt.NewRow();
                var colAtt = prop.GetCustomAttribute<ColumnNameAttribute>();
                var tabAtt = EntityType.GetCustomAttribute<TableNameAttribute>();
                dr["ColumnName"] = colAtt != null ? colAtt.Name : prop.Name;
                dr["IsAutoIncrement"] = prop.GetCustomAttribute<IdentityColumnAttribute>() != null;
                dr["IsIdentity"] = dr["IsAutoIncrement"];
                dr["IsKey"] = prop.GetCustomAttribute<PrimaryKeyAttribute>() != null;
                dr["AllowDBNull"] = Nullable.GetUnderlyingType(prop.PropertyType) != null;
                dr["BaseTableName"] = tabAtt != null ? tabAtt.Name : EntityType.Name;
            }
            return dt;
        }

        public string GetString(int i)
        {
            return _current.GetString(i);
        }

        public object GetValue(int i)
        {
            return _current.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _current.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return _current.IsDBNull(i);
        }

        public bool NextResult()
        {
            return !((Records.IndexOf(_current) + 1) > (Records.Count - 1));
        }

        public bool Read()
        {
            if (_current == null)
            {
                _current = Records.FirstOrDefault();
                return true;
            }
            var res = NextResult();
            if (res) _current = Records[Records.IndexOf(_current) + 1];
            return res;
        }
    }
}
