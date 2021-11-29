using Luval.Common;
using Luval.Data;
using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationUser : BaseAuditEntity, IApplicationUser
    {
        public ApplicationUser() : base()
        {
            CreatedByUserId = Id;
            UpdatedByUserId = Id;
            Roles = new List<ApplicationUserRole>();
        }

        public string ProviderKey { get; set; }
        public string ProviderName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }

        [TableReference]
        public List<ApplicationUserRole> Roles { get; set; }

    }
}
