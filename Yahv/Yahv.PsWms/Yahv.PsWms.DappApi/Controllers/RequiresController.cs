using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 特殊要求
    /// </summary>
    public class RequiresController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noticeID">通知ID</param>
        /// <returns>一个通知的全部的特殊要求</returns>
        public ActionResult Show(string id)
        {
            var requires = new Services.Views.RequiresView().Where(t => t.NoticeID == id);
            return Json(requires, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 标签特殊要求
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public ActionResult ShowLabel(string id)
        {
            var requires = new Services.Views.PcFilesView()
                .Where(t => t.MainID == id)
                .Where(t => t.Type == Services.Enums.FileType.Label);
            return Json(requires, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发货单特殊要求
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public ActionResult ShowOutDelivery(string id)
        {
            var requires = new Services.Views.PcFilesView()
                .Where(t => t.MainID == id)
                .Where(t => t.Type == Services.Enums.FileType.OutDelivery);
            return Json(requires, JsonRequestBehavior.AllowGet);
        }
    }
}