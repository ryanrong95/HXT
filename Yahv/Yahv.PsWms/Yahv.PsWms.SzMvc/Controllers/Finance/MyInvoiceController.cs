using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class FinanceController : BaseController
    {
        #region 页面

        /// <summary>
        /// 我的发票
        /// </summary>
        /// <returns></returns>
        public ActionResult MyInvoice() { return View(); }

        #endregion

        /// <summary>
        /// 获取我的发票列表数据
        /// </summary>
        /// <returns></returns>
        [Debug]
        public JsonResult GetMyInvoiceList()
        {
            return Json(new { type = "", msg = "", data = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}