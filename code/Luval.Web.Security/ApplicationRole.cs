using Luval.Common;
using Luval.Data;
using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationRole : BaseAuditEntity
    {
        public ApplicationRole() : base()
        {
            RoleClaims = new List<ApplicationRoleClaim>();
        }
        public string RoleName { get; set; }
        [TableReference]
        public List<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}
