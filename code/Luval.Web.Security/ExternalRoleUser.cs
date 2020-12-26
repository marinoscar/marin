using Luval.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ExternalRoleUser : IAuditRecord
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserProfileId { get; set; }
        public string UpdatedByUserProfileId { get; set; }
    }
}
