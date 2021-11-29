using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.UrlShortner.Web.Areas.Shortner.Controllers
{
    [Area("r"), Authorize]
    public class RedirectController : Controller
    {
        public RedirectController(IUrlShortnerRepository urlShortnerRepository)
        {
            Repository = urlShortnerRepository;
        }

        public IUrlShortnerRepository Repository { get; private set; }

        [AllowAnonymous, HttpGet, Route("r/{id}")]
        public async Task<IActionResult> Index(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            ShortName item = null;
            try
            {
                item = await Repository.GetByIdAsync(id, cancelToken);
            }
            finally
            {
            }
            if (item == null) return View("~/Views/Shortner/Error.cshtml");
            await Repository.UpdateAsync(item, cancelToken);
            return RedirectPermanent(item.OriginalUri);
        }

        [HttpGet, Route("r/create")]
        public IActionResult Create()
        {
            return View("~/Views/Shortner/Create.cshtml");
        }
    }
}
