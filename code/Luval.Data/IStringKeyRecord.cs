using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public interface IStringKeyRecord
    {
        [PrimaryKey]
        string Id { get; set; }
        DateTime UtcCreatedOn { get; set; }
        DateTime UtcUpdatedOn { get; set; }
        string CreatedByUserId { get; set; }
        string UpdatedByUserId { get; set; }
    }
}
