using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Luval.Core
{
    public class Mapper
    {
        public static DataTable Copy(DataTable target, IDataRecord source)
        {
            target.TableName = (string)source["FieldName"];
            return target;
        }
    }
}
