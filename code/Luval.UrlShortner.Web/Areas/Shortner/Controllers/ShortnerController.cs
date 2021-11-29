using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luval.UrlShortner.Web.Areas.Shortner.Controllers
{
    [Area("Shortner"), Authorize]
    public class ShortnerController : Controller
    {
        [HttpGet, Route("Shortner")]
        public IActionResult Index(string id)
        {
            return View();
        }
    }
}
