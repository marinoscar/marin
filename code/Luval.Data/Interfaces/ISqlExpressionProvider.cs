using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface ISqlExpressionProvider
    {
        string ResolveWhere<TEntity>(Expression<Func<TEntity, bool>> expression);
        string ResolveOrderBy<TEntity>(Expression<Func<TEntity, object>> orderBy, bool @descending);
    }
}
