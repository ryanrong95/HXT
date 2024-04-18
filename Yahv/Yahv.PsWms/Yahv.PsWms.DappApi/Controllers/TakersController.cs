using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 拿货人接口
    /// </summary>
    public class TakersController : Controller
    {
        public ActionResult List()
        {
            using (var view = new Services.Views.TakersView())
            {
                var results = view.ToArray();
                return Json(new
                {
                    data = results
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存新增的Taker拿货人
        /// </summary>
        /// <returns></returns>
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                using (var view = new Services.Views.TakersView())
                {
                    view.Enter(jpost);

                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = string.Empty,
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 200;
                return Json(new
                {
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 修改Taker信息
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Modify(JPost jpost)
        {
            try
            {
                using (var view = new Services.Views.TakersView())
                {
                    view.Modify(jpost);

                    Response.StatusCode = 200;
                    return Json(new
                    {
                        success = true,
                        data = string.Empty,
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 200;
                return Json(new
                {
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}