﻿using Luval.Data;
using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationUser : IApplicationUser, IStringKeyAuditEntity
    {
        public ApplicationUser()
        {
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public string UpdatedByUserId { get; set; }

        [TableReference]
        public List<ApplicationUserRole> Roles { get; set; }

    }
}
