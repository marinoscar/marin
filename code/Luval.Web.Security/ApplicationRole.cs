using Luval.Data;
using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationRole : IStringKeyAuditEntity
    {
        public ApplicationRole()
        {
            RoleClaims = new List<ApplicationRoleClaim>();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string RoleName { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public string UpdatedByUserId { get; set; }
        [TableReference]
        public List<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}
