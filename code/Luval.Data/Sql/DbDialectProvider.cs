using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public abstract class DbDialectProvider
    {
        public abstract IDbDialectProvider Create(DbTableSchema schema);

        public virtual IDbDialectProvider Create(Type entityType)
        {
            return Create(DbTableSchema.Create(entityType));
        }
    }
}
