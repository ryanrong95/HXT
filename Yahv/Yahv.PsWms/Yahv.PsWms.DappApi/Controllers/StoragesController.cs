using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 库存接口
    /// </summary>
    public class StoragesController : Controller
    {
        /// <summary>
        /// 库存记录
        /// </summary>
        /// <param name="jPost">前端过滤条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show(JPost jpost)
        {
            var args = new
            {
                OrderID = jpost["OrderID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                ShelveCode = jpost["ShelveCode"]?.Value<string>(),
                Partnumber = jpost["Partnumber"]?.Value<string>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            using (var view = new StoragesView())
            {
                var data = view.SearchByCondition(item => item.PackageNumber > 0);
                data = data.OrderByDateTimeDesc(item => item.ModifyDate);

                if (!string.IsNullOrWhiteSpace(args.OrderID))
                {
                    data = data.SearchByOrderID(args.OrderID);
                }
                if (!string.IsNullOrWhiteSpace(args.ClientName))
                {
                    data = data.SearchByClientName(args.ClientName);
                }
                if (!string.IsNullOrWhiteSpace(args.ShelveCode))
                {
                    data = data.SearchByShelveCode(args.ShelveCode);
                }
                if (!string.IsNullOrWhiteSpace(args.Partnumber))
                {
                    data = data.SearchByPartnumber(args.Partnumber);
                }

                var result = data.ToMyPage(args.PageIndex, args.PageSize);

                return Json(new
                {
                    success = true,
                    code = 200,
                    data = result
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 拆分库存
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Inventory(JPost jpost)
        {
            var arguments = new
            {
                storageID = jpost["ID"].Value<string>(),       // 要拆分的或移库的库存ID 
                quantity = jpost["Quantity"]?.Value<int?>(),   // 要拆分或移库的数量
                summary = jpost["Summary"]?.Value<string>(),   // 拆分库存备注
            };

            if (string.IsNullOrEmpty(arguments.storageID) || arguments.quantity.HasValue == false)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    data = "参数不正确, ID(库存ID) 不能为null 或空字符串, 拆分库存的数量必须有值"
                });
            }

            using (var view = new StoragesView())
            {
                view.Inventory(arguments.storageID, arguments.quantity.Value, arguments.summary);

                return Json(new
                {
                    success = true,
                    data = string.Empty
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 打印剩余标签
        /// </summary>
        /// <param name="id">库存ID</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PrintLabel(string id)
        {
            throw new NotImplementedException();
        }
    }
}