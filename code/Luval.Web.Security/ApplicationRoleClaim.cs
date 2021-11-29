using Luval.Common;
using Luval.Data;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationRoleClaim : BaseAuditEntity
    {
        public string RoleId { get; set; }
        public string ClaimName { get; set; }
        public string ClaimValue { get; set; }
    }
}
