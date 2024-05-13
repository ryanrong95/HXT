using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApi.Controllers
{
    public class GeckoHelperController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            //return JavaScript(WinApp.Services.GeckoHelper.Script());
            return View("");
        }
    }
}