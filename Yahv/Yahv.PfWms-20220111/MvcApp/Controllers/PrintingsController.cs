using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class PrintingsController : Controller
    {
        // GET: Printings
        public ActionResult Index()
        {
            var o= new Wms.Services.Views.PrintingsView();
            return Json(new { obj = new Wms.Services.Views.PrintingsView() }, JsonRequestBehavior.AllowGet);
        }
    }
}