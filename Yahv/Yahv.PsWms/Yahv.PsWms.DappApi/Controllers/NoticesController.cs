using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 通知接口
    /// </summary>
    abstract public class NoticesController : ClientController
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <returns></returns>
        virtual public ActionResult Show(JPost jpost)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        virtual public ActionResult Detail(string id)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 复核
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        abstract public ActionResult Review(JPost jpost);

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <returns></returns>
        virtual public ActionResult Notice(JPost jpost)
        {

            return View();
        }
    }
}