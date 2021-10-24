using Luval.Data;
using Luval.Data.Interfaces;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marin.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController(IUnitOfWorkFactory unitOfWorkFactory, IApplicationUserRepository userRepository)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            UserRepository = userRepository;
        }

        protected IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }
        protected IApplicationUserRepository UserRepository { get; private set; }

        

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            var currentUser = await GetUserAsync();
            var userUoW = UnitOfWorkFactory.Create<ApplicationUser, string>();
            currentUser.PrepareForInsert(currentUser);
            userUoW.Entities.Add(user);
            await userUoW.SaveChangesAsync(CancellationToken.None);
            return Ok();
        }

        private Task<ApplicationUser> GetUserAsync()
        {
            return UserRepository.GetUserByMailAsync(User.GetEmail());
        } 
    }
}
