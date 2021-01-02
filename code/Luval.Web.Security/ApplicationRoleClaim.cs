using Luval.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationRoleClaim : IStringKeyRecord
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimName { get; set; }
        public string ClaimValue { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserProfileId { get; set; }
        public string UpdatedByUserProfileId { get; set; }
    }
}
