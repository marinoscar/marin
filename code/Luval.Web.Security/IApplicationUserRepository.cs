using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetUserByMailAsync(string email);
        Task<ApplicationUser> CreateOrUpdateExternalUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}
