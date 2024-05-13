using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous.Views;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers.chonggou
{
    public class StatisticStorageController : Controller
    {
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                whid = jpost["WhID"]?.Value<string>(),
                enterCode = jpost["EnterCode"]?.Value<string>(),
                starttime = jpost["StartTime"]?.Value<string>(),
                endtime = jpost["EndTime"]?.Value<string>(),
                clientName = jpost["ClientName"]?.Value<string>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var view = new StatisticStorageView();
            if (!string.IsNullOrEmpty(arguments.enterCode))
            {
                view = view.SearchByEnterCode(arguments.enterCode);
            }

            if (!string.IsNullOrEmpty(arguments.clientName))
            {
                view = view.SearchByClientName(arguments.clientName);
            }

            if (!string.IsNullOrEmpty(arguments.starttime))
            {
                if (!string.IsNullOrEmpty(arguments.endtime))
                {
                    view = view.SearchByCreateDate(DateTime.Parse(arguments.starttime), DateTime.Parse(arguments.endtime));
                }
                else
                {
                    var time = DateTime.Now;
                    view = view.SearchByCreateDate(DateTime.Parse(arguments.starttime), new DateTime(time.Year, time.Month, time.Day));
                }
            }
            var result = view.ToMyPage(arguments.pageIndex, arguments.pageSize);
            return Json( new
            {
                code = 200,
                success = true,
                data = result,
            }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 根据库房人员录入的大中小箱数据进行修改
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Modify(JPost jpost)
        {
            try
            {
                var view = new StatisticStorageView();
                var statisticStorages = jpost["StatisticStorages"];
                view.Modify(statisticStorages);
                return Json(new
                {
                    code = 200,
                    success = true,
                    data = "",
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }            
        }
    }
}