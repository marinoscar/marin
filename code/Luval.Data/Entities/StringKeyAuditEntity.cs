using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Entities
{
    public abstract class StringKeyAuditEntity : AuditEntity<string>
    {
        public StringKeyAuditEntity() : base()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
