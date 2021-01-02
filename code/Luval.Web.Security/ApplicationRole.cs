using Luval.Data;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    [TableName("SecurityRole")]
    public class ApplicationRole : IStringKeyRecord
    {
        public ApplicationRole()
        {
            RoleClaims = new List<ApplicationRoleClaim>();
            RoleUsers = new List<ApplicationUserRole>();
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
        public List<ApplicationRoleClaim> RoleClaims { get; set; }
        public List<ApplicationUserRole> RoleUsers { get; set; }
    }
}
