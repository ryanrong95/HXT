using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Yahv.Erm.Fingerprints.Services;

namespace Yahv.Erm.WebApi.Controllers
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class TimesController : Controller
    {
        ///// <summary>
        ///// 返回淘宝时间
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Taobo()
        //{
        //    return Json(new
        //    {
        //        ticks = TimeServer.Current.TaoBo().Ticks
        //    });
        //}

        /// <summary>
        /// 返回淘宝时间
        /// </summary>
        /// <returns></returns>
        public ActionResult Enterprise()
        {
            return Json(new
            {
                ticks = DateTime.Now.AddSeconds(5).Ticks
            }, JsonRequestBehavior.AllowGet);
        }
    }
}