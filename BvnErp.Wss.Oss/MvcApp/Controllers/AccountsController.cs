using Needs.Underly;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public interface IAccount
    {
        Currency Currency { get; }
        decimal Price { get; }
    }

    public class AccountsController : UserController
    {
        NtErp.Wss.Oss.Services.Models.ClientTop GetClient()
        {
            using (var view = new NtErp.Wss.Oss.Services.Views.ClientsTopView())
            {
                return view[this.Client.ID];
            }
        }

        public ActionResult Balances()
        {
            var arry = this.GetClient().GetBalances(NtErp.Wss.Oss.Services.UserAccountType.Cash);
            return Json(arry, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Credits()
        {
            var arry = this.GetClient().GetBalances(NtErp.Wss.Oss.Services.UserAccountType.Credit);
            return Json(arry, JsonRequestBehavior.AllowGet);
        }
    }
}