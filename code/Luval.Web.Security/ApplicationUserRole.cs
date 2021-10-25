using Luval.Data;
using Luval.Data.Attributes;
using Luval.Data.Entities;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationUserRole : StringKeyAuditEntity
    {
        public ApplicationUserRole()
        {
            Id = Guid.NewGuid().ToString();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
        }

        public string ApplicationRoleId { get; set; }
        [TableReference]
        public ApplicationRole Role { get; set; }
        public string ApplicationUserId { get; set; }
        [TableReference]
        public ApplicationUser User { get; set; }
    }
}
