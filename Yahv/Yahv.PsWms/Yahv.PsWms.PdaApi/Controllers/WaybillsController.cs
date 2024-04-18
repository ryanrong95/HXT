using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.PdaApi.Services.Models;
using Yahv.PsWms.PdaApi.Services.Views;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.PdaApi.Controllers
{
    /// <summary>
    /// 运单接口
    /// </summary>
    public class WaybillsController : Controller
    {
        /// <summary>
        /// 运单信息
        /// </summary>
        /// <param name="waybillCode">运单号</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Info(string waybillCode)
        {
            if (string.IsNullOrEmpty(waybillCode))
                throw new ArgumentNullException("运单号不能为空");

            using (var view = new WaybillInfosView())
            {
                var result = view.SearchByWaybillCode(waybillCode).Single();
                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 运单详情(含产品信息，收货人信息等)
        /// </summary>
        /// <param name="waybillCode"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Detail(string waybillCode)
        {
            if (string.IsNullOrEmpty(waybillCode))
                throw new ArgumentNullException("运单号不能为空");

            using (var view = new WaybillDetailsView())
            {
                var result = view.SearchByWaybillCode(waybillCode).Single();
                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 运费录入
        /// </summary>
        /// <param name="jpost">运费信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enter(JPost jpost)
        {
            using (var view = new WaybillsView())
            {
                view.EnterFee(jpost);
                return Json(new { success = true, code = 200, data = "运单费用录入成功" }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}