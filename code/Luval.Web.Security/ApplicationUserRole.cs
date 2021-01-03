﻿using Luval.Data;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationUserRole : IStringKeyRecord
    {
        public string Id { get; set; }
        public string ApplicationRoleId { get; set; }
        [TableReference]
        public ApplicationRole Role { get; set; }
        public string ApplicationUserId { get; set; }
        [TableReference]
        public ApplicationUser User { get; set; }
        public DateTime UtcCreatedOn { get; set; }
        public DateTime UtcUpdatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public string UpdatedByUserId { get; set; }
    }
}
