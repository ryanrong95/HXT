using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous.Views;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    public class cgLogOperatorsController : Controller
    {
        /// <summary>
        /// 日志展示
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                waybillID = jpost["MainID"]?.Value<string>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 10,
                key = jpost["Key"]?.Value<string>(),
            };

            using (var view = new CgLogOperatorsView())
            {
                var data = view;

                if (string.IsNullOrEmpty(arguments.waybillID))
                {
                    return Json(new
                    {
                        Code = 400,
                        Success = false,
                        Data = "请检查参数是否正确, MainID 不能为Null",
                    }, JsonRequestBehavior.DenyGet);
                }

                if (!string.IsNullOrEmpty(arguments.key))
                {
                    data = data.SearyByKey(arguments.key);
                }

                var result = data.SearchByMainID(arguments.waybillID).ToMyPage(arguments.pageIndex, arguments.pageSize);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                using (var view = new CgLogOperatorsView())
                {
                    view.Enter(jpost);

                    return Json(new
                    {
                        Code = 200,
                        Success = true,
                        Data = String.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 400,
                    Success = false,
                    Data = ex.Message,
                });
            }
        }
    }
}