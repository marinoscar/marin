using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TEntity, TKey> Create<TEntity, TKey>();
    }
}
