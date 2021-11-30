using Luval.Data.Extensions;
using Luval.UrlShortner.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Luval.Common;

namespace Luval.UrlShortner.Web.Areas.Shortner.Controllers
{
    [Area("Shortner"), AllowAnonymous]
    public class ShortnerController : Controller
    {
        public ShortnerController(IUrlShortnerRepository urlShortnerRepository)
        {
            Repository = urlShortnerRepository;
        }

        public IUrlShortnerRepository Repository { get; private set; }

        [HttpGet, Route("Shortner")]
        public IActionResult Index(string id)
        {
            return View();
        }

        [HttpPost, Route("Shortner/Create")]
        public async Task<IActionResult> Create(ShortName item, CancellationToken cancellationToken)
        {
            if (item == null) return Json(new ShortnerViewModel() { Success = false, Message = "Invalid payload" });
            if (await Repository.GetByIdAsync(item.Id, cancellationToken) != null)
                return Json(new ShortnerViewModel() { Success = false, IsDuplicate = true, Message = "Short Code {0} already exists".Format(item.Id) });
            ShortName result = null;
            try
            {
                result = await Repository.AddUrlAsync(item.Id, item.OriginalUri, cancellationToken);
            }
            catch (Exception ex)
            {
                return Json(new ShortnerViewModel() { Success = false, IsDuplicate = false, Message = "Short Code {0} failed with error: {1}".Format(item.Id, ex.Message) });
            }
            return Json(new ShortnerViewModel()
            {
                Success = true,
                IsDuplicate = false,
                Message = "New short url created succesfully",
                Url = "{0}://{1}/r/{2}".Format(Request.Scheme, Request.Host, result.Id)
            }); ;
        }
    }
}
