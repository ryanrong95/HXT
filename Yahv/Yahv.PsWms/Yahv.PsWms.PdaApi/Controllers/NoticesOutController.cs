using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.PdaApi.Services.Enums;
using Yahv.PsWms.PdaApi.Services.Views;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.PdaApi.Controllers
{
    /// <summary>
    /// 出库通知接口
    /// </summary>
    public class NoticesOutController : Controller
    {
        /// <summary>
        /// 待出库列表
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show(JPost jpost)
        {
            var args = new
            {
                OrderID = jpost["OrderID"]?.Value<string>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            using (var view = new NoticesOutView())
            {
                //待出库通知状态
                NoticeStatus[] statuses = { NoticeStatus.Processing, NoticeStatus.Reviewing, NoticeStatus.Packing };
                var data = view.SearchByStatus(statuses);

                if (!string.IsNullOrEmpty(args.OrderID))
                {
                    data = data.SearchByOrderID(args.OrderID);
                }

                var result = data.ToMyPage(args.PageIndex, args.PageSize);
                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 待出库详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("通知ID不能为空");

            using (var view = new NoticesOutView())
            {
                var result = view.SearchByID(id).Single();
                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}