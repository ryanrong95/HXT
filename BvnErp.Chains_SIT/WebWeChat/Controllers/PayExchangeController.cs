using Needs.Utils.Descriptions;
using Needs.Wl.Web.WeChat;
using System;
using System.Linq;
using System.Web.Mvc;
using WebWeChat.Models;

namespace WebWeChat.Controllers
{
    [UserAuthorize(UserAuthorize = true)]
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class PayExchangeController : UserController
    {
        /// <summary>
        /// 付汇记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PayRecord(string id)
        {
            var model = new Needs.Ccs.Services.Views.OrderPayExchangeItemsView()
                .Where(item => item.OrderID == id).OrderByDescending(item => item.ApplyTime)
                .ToList().Select(item => new PayRecordModel
                {
                    SupplierName = item.SupplierName,
                    ApplyTime = item.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Applier = item.User == null ? "跟单员" : item.User.Name,
                    Amount = item.Amount.ToString("0.00"),
                    Status = item.Status.GetDescription()
                }).ToList();

            return View(model);
        }
    }
}