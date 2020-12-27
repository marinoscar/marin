using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public class SqlServerDialectFactory : SqlDialectFactory
    {
        public override ISqlDialectProvider Create(SqlTableSchema schema)
        {
            return new SqlServerDialectProvider(schema);
        }
    }
}
