using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ApplicationRoleManager<TRole> : RoleManager<TRole> where TRole : ApplicationRole
    {
        public ApplicationRoleManager(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger)
            :base(store, roleValidators, keyNormalizer, errors, logger)
        {

        }
    }
}
