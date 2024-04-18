using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.PdaApi.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Json(new { PsWms = "PdaApi is running" }, JsonRequestBehavior.AllowGet);
        }
    }
}