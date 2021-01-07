using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface IAuditableEntity<TKey> : IIdBasedEntity<TKey>, ICreatedEntity, IUpdatedEntity
    {
    }
}
