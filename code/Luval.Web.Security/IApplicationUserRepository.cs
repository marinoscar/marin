using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetUserByMailAsync(string email);
    }
}
