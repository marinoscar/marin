using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public abstract class SqlDialectFactory
    {
        public abstract ISqlDialectProvider Create(SqlTableSchema schema);

        public virtual ISqlDialectProvider Create(Type entityType)
        {
            return Create(SqlTableSchema.Create(entityType));
        }
    }
}
