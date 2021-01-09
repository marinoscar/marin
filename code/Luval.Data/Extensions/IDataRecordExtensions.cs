using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Luval.Data.Extensions
{
    public static class IDataRecordExtensions
    {
        public static IDictionary<string, object> ToDictionary(this IDataRecord record)
        {
            var dic = new Dictionary<string, object>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                dic[record.GetName(i)] = record.GetValue(i);
            }
            return dic;
        }
    }
}
