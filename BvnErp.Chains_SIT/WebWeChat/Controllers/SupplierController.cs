using Needs.Wl.Web.WeChat;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WebWeChat.Controllers
{
    [UserAuthorize(UserAuthorize = true)]
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class SupplierController : UserController
    {
        /// <summary>
        /// 会员供应商列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MySuppliers()
        {
            var pickSupplier = new Needs.Ccs.Services.Views.OrderConsigneesView().Select(item => item.ClientSupplierID).ToArray();
            var paySupplier = new Needs.Ccs.Services.Views.OrderPayExchangeSuppliersView().Select(item => item.ClientSupplier.ID).ToArray();
            var ids = pickSupplier.Concat(paySupplier).Distinct();

            var supplerView = Needs.Wl.User.Plat.WeChatPlat.Current.MySuppliers;
            supplerView.AllowPaging = false;

            var list = supplerView.ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.ChineseName,
                isShowBtn = !ids.Contains(item.ID)
            });

            return View(list);
        }
    }
}