using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            var kkk = Session[Buyer.Services.OldSso.session_cartid] as Dictionary<string, int>;
            return View(kkk.Count);
        }
    }
}