using Luval.Data;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Marin.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController(IEntityAdapterFactory entityAdapterFactory, IApplicationUserRepository userRepository)
        {
            AdapterFactory = entityAdapterFactory;
            UserRepository = userRepository;
        }

        protected IEntityAdapterFactory AdapterFactory { get; private set; }
        protected IApplicationUserRepository UserRepository { get; private set; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            var currentUser = await GetUserAsync();
            var userAdapter = AdapterFactory.Create<ApplicationUser, string>();
            
            user.CreatedByUserId = currentUser.Id;
            user.UpdatedByUserId = currentUser.Id;
            user.UtcCreatedOn = DateTime.UtcNow;
            user.UtcUpdatedOn = user.UtcCreatedOn;

            await userAdapter.InsertAsync(user);
            return Ok();
        }

        private Task<ApplicationUser> GetUserAsync()
        {
            return UserRepository.GetUserByMailAsync(User.GetEmail());
        } 
    }
}
