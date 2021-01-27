using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public class SqlServerDialectFactory : DbDialectProvider
    {
        public override IDbDialectProvider Create(DbTableSchema schema)
        {
            return new SqlServerDialectProvider(schema);
        }
    }
}
