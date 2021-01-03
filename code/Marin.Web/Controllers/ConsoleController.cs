using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marin.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConsoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
