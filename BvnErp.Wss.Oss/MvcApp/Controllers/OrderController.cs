using NtErp.Wss.Oss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class OrderController : UserController
    {
        // GET: Order
        public ActionResult Index(string id)
        {
            using (var view = new NtErp.Wss.Oss.Services.Views.OrderAlls())
            {
                string clientid = Buyer.Services.OldSso.Current.ID;

                var rzlt = view.Single(item => /*item.Client.ID == clientid &&*/ item.ID == id);

                return Json(rzlt, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Pay(string id, int type)
        {
            using (var view = new NtErp.Wss.Oss.Services.Views.OrderAlls())
            {
                string clientid = Buyer.Services.OldSso.Current.ID;
                // 支付方式
                UserAccountType accountType = (UserAccountType)type;

                var order = view.Single(item => item.Client.ID == clientid && item.ID == id);
                order.NotEnough += Order_NotEnough;
                order.Pay(accountType == UserAccountType.Cash, accountType == UserAccountType.Credit);

                return this.CurrentResult ?? Json(new
                {
                    status = 200,
                    message = "10043"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private void Order_NotEnough(object sender, Needs.Linq.ErrorEventArgs e)
        {
            eJson(new
            {
                status = 400,
                message = "10042",
            });
        }
        [HttpPost]
        public ActionResult Close(string id)
        {
            using (var view = new NtErp.Wss.Oss.Services.Views.OrderAlls())
            {
                string clientid = Buyer.Services.OldSso.Current.ID;

                var order = view.Single(item => item.Client.ID == clientid && item.ID == id);
                order.Close();

                return Json(new
                {
                    status = 200,
                    message = "10047"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}