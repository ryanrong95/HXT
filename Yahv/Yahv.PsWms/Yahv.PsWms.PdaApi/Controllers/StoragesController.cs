using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.PdaApi.Services.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.PdaApi.Controllers
{
    /// <summary>
    /// 库存接口
    /// </summary>
    public class StoragesController : Controller
    {
        /// <summary>
        /// 库存信息
        /// </summary>
        /// <param name="inCode">入库标签条码号</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Detail(string inCode)
        {
            if (string.IsNullOrEmpty(inCode))
                throw new ArgumentNullException("入库标签不能为空");

            using (var view = new StoragesView())
            {
                var result = view.SearchByID(inCode.Trim()).Single();
                if (result == null)
                    throw new Exception($"未查询到【{inCode}】对应的库存记录");

                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 扫码上架
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Shelving(JPost jpost)
        {
            using (var view = new StoragesView())
            {
                view.Shelving(jpost);
                return Json(new { success = true, code = 200, data = "上架成功" }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 库位变更
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeShelve(JPost jpost)
        {
            using (var view = new StoragesView())
            {
                view.ChangeShelve(jpost);
                return Json(new { success = true, code = 200, data = "库位变更成功" }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}