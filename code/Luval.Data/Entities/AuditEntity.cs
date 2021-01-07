using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Entities
{
    public abstract class AuditEntity<TKey> : IAuditableEntity<TKey>
    {

        public AuditEntity()
        {
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = DateTime.UtcNow;
        }
        [PrimaryKey]
        public TKey Id { get ; set; }
        public DateTime UtcCreatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string UpdatedByUserId { get; set; }
    }
}
