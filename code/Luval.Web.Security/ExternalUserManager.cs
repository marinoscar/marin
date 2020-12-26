using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public class ExternalUserManager<TUser> : UserManager<TUser> where TUser : class
    {
    }
}
