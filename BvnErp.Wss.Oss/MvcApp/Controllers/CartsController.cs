using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class CartsController : Controller
    {
        [HttpPost]
        public ActionResult Selects(string carts)
        {
            string[] arry = carts.Split(',');
            Session[Buyer.Services.OldSso.session_cartid] = new List<string>(arry);

            return Json(new
            {
                status = 200,
            });
        }
    }
}