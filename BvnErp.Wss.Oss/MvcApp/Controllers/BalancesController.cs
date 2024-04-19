using NtErp.Wss.Oss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class BalancesController : UserController
    {
        // GET: Balances
        public ActionResult Index()
        {
            Needs.Underly.Currency outer;
            Enum.TryParse(Request["Currency"], out outer);
            UserAccountType outputType;
            Enum.TryParse(Request["type"], out outputType);
            using (var view = new NtErp.Wss.Oss.Services.Views.UserOutputsView())
            {
                var json = Paging(view.Where(item => item.ClientID == this.Client.ID && item.Currency == outer && item.Type== outputType).OrderByDescending(item=>item.CreateDate));
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}