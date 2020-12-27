using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public interface IAuditRecord
    {
        [PrimaryKey]
        string Id { get; set; }
        DateTime UtcCreatedOn { get; set; }
        DateTime UtcUpdatedOn { get; set; }
        string CreatedByUserProfileId { get; set; }
        string UpdatedByUserProfileId { get; set; }
    }
}
