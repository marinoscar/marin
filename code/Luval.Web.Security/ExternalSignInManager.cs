using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public class ExternalSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
    }
}
