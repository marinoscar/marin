using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public interface IAuditRecord
    {
        string Id { get; set; }
        DateTime UtcCreatedOn { get; set; }
        DateTime UtcUpdatedOn { get; set; }
        string CreatedByUserProfileId { get; set; }
        string UpdatedByUserProfileId { get; set; }
    }
}
