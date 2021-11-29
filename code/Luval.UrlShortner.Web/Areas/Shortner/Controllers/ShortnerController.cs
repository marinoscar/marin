using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.UrlShortner.Web.Areas.Shortner.Controllers
{
    [Area("r")]
    public class ShortnerController : Controller
    {
        public ShortnerController(IUrlShortnerRepository urlShortnerRepository)
        {
            Repository = urlShortnerRepository;
        }

        public IUrlShortnerRepository Repository { get; private set; }

        [AllowAnonymous, HttpGet, Route("r/{id}")]
        public async Task<IActionResult> Index(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            var item = await Repository.GetByIdAsync(id, cancelToken);
            if (item == null) return View("Error");
            return RedirectPermanent(item.OriginalUri);
        }
    }
}
