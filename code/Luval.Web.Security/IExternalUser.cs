using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Web.Security
{
    public interface IExternalUser
    {
        string Id { get; set; }
        string ProviderKey { get; set; }
        string ProviderName { get; set; }
        string Email { get; set; }
        string DisplayName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string ProfilePicture { get; set; }
    }
}
