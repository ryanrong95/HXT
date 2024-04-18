using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Yahv.PsWms.SzApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Json(new { msg = "dfs" }, JsonRequestBehavior.AllowGet);
        }
    }
}