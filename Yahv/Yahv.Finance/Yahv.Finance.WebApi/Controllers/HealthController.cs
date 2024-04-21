using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    public class HealthController : ClientController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
    }
}