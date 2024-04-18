using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class HomeController : BaseController
    {
        /// <summary>
        /// 主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() { return View(); }

        /// <summary>
        /// 获取主页信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetIndexData()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theSiteuserID = siteuser.SiteuserID;

            GetIndexDataOrderModel[] orders = new GetIndexDataOrderModel[0];

            using (var query = new AllStorageListView())
            {
                var view = query;
                view = view.SearchBySiteuserID(theSiteuserID);

                Func<AllStorageListViewModel, GetIndexDataOrderModel> convert = item => new GetIndexDataOrderModel
                {
                    CreateDateDes = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderID = item.OrderID,
                    OrderTypeDes = item.OrderType.GetDescription(),
                    OrderStatusDes = item.OrderStatus.GetDescription(),
                };

                orders = view.ToMyPage(convert, 1, 10).Item1;
            }

            return Json(new { type = "success", msg = string.Empty, data = new { Orders = orders, } }, JsonRequestBehavior.AllowGet);
        }
    }
}