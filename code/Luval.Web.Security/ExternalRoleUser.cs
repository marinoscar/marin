using Luval.Data;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    [TableName("UserSecurityRole")]
    public class ExternalRoleUser : IStringKeyRecord
    {
        public string Id { get; set; }
        public string SecurityRoleId { get; set; }
        [TableReference]
        public ExternalRole Role { get; set; }
        public string UserProfileId { get; set; }
        [TableReference]
        public ExternalUser User { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserProfileId { get; set; }
        public string UpdatedByUserProfileId { get; set; }
    }
}
