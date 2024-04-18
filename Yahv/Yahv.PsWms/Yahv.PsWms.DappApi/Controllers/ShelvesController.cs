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
    /// 库位接口
    /// </summary>
    public class ShelvesController : Controller
    {
        /// <summary>
        /// 库位列表
        /// </summary>
        /// <param name="jpost">前端过滤条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show(JPost jpost)
        {
            var args = new
            {
                OrderID = jpost["OrderID"]?.Value<string>(),
                Company = jpost["Company"]?.Value<string>(),
                ShelveCode = jpost["ShelveCode"]?.Value<string>(),
                Partnumber = jpost["Partnumber"]?.Value<string>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            using (var view = new ShelvesView())
            {
                var data = view;

                if (!string.IsNullOrEmpty(args.OrderID))
                {
                    data = data.SearchByOrderID(args.OrderID);
                }
                if (!string.IsNullOrEmpty(args.Company))
                {
                    data = data.SearchByCompany(args.Company);
                }
                if (!string.IsNullOrEmpty(args.ShelveCode))
                {
                    data = data.SearchByShelveCode(args.ShelveCode);
                }
                if (!string.IsNullOrEmpty(args.Partnumber))
                {
                    data = data.SearchByPartnumber(args.Partnumber);
                }

                var result = data.ToMyPage(args.PageIndex, args.PageSize);
                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 库位详情
        /// </summary>
        /// <param name="id">库位ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new JMessage { success = false, code = 500, data = "库位id不能为空" }, JsonRequestBehavior.AllowGet);
            }

            using (var view = new ShelvesView())
            {
                var result = view.SearchByID(id).Single();
                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="jpost">库位信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                using (var view = new ShelvesView())
                {
                    view.Enter(jpost);
                    return Json(new { success = true, code = 200, data = "新增库位成功" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="id">库位ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                using (var view = new ShelvesView())
                {
                    view.Delete(id);
                    return Json(new { success = true, code = 200, data = "删除库位成功" }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 打印库位标签
        /// </summary>
        /// <param name="id">库位ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrintLabel(string id)
        {
            throw new NotImplementedException();
        }
    }
}