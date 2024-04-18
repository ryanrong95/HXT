using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.PdaApi.Services.Views;

namespace Yahv.PsWms.PdaApi.Controllers
{
    /// <summary>
    /// 预出库单接口
    /// </summary>
    public class PreExitBillsController : Controller
    {
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="code">预出库单条码号</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Detail(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("预出库单号不能为空");

            using (var view = new PreExitBillsView())
            {
                var result = view.SearchByID(code.Trim()).Single();
                if (result == null)
                    throw new Exception("未查询到预出库单信息");

                return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 扫码复核
        /// </summary>
        /// <param name="code">预出库单条码号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Review(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("预出库单号不能为空");

            using (var view = new PreExitBillsView())
            {
                view.Review(code.Trim());
                return Json(new { success = true, code = 200, data = "复核成功" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 扫码出库
        /// </summary>
        /// <param name="code">预出库单条码号</param>
        /// <param name="reviewerID">复核人</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Outbound(string code, string reviewerID)
        {
            if (string.IsNullOrEmpty(code.Trim()))
                throw new ArgumentNullException("预出库单号不能为空");

            using (var view = new PreExitBillsView())
            {
                view.Outbound(code.Trim(), reviewerID.Trim());
                return Json(new { success = true, code = 200, data = "出库成功" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取预出库单相关文件
        /// </summary>
        /// <param name="code">预出库单条码号</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Files(string code)
        {
            if (string.IsNullOrEmpty(code.Trim()))
                throw new ArgumentNullException("预出库单号不能为空");

            throw new NotImplementedException();
        }

        /// <summary>
        /// 接收单个文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterFile()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 接收多个文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterFiles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFile(string id)
        {
            throw new NotImplementedException();
        }
    }
}