using Luval.Data;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    [TableName("SecurityRole")]
    public class ExternalRole : IAuditRecord
    {
        public ExternalRole()
        {
            RoleClaims = new List<ExternalRoleClaim>();
            RoleUsers = new List<ExternalRoleUser>();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string RoleName { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserProfileId { get; set; }
        public string UpdatedByUserProfileId { get; set; }
        public List<ExternalRoleClaim> RoleClaims { get; set; }
        public List<ExternalRoleUser> RoleUsers { get; set; }
    }
}
