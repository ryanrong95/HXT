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
    public class PayStatisticsController : Controller
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                begin = jpost["s_begin"]?.Value<string>(),
                end = jpost["s_end"]?.Value<string>(),
                clientid = jpost["s_client"]?.Value<string>(),
                code = jpost["s_code"]?.Value<string>(),
                currency = jpost["s_currency"]?.Value<string>(),
                payer = jpost["s_payer"]?.Value<string>(), //付款公司
                pageindex = jpost["s_pageindex"]?.Value<int?>() ?? 1,
                pagesize = jpost["s_pagesize"]?.Value<int?>() ?? 20
            };

            using (var view = new PayStatisticsView())
            {
                var data = view.GetData(arguments.begin, arguments.end, arguments.clientid, arguments.code, arguments.currency, arguments.payer, arguments.pageindex, arguments.pagesize);
                return Json(data, JsonRequestBehavior.DenyGet);
            }                
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult GetDetail(JPost jpost)
        {
            var arguments = new
            {
                begin = jpost["s_begin"]?.Value<string>(),
                end = jpost["s_end"]?.Value<string>(),
                clientid = jpost["clientid"]?.Value<string>(),
                payer = jpost["payer"]?.Value<string>(),
                currency = jpost["currency"]?.Value<string>(),
            };
            using (var view = new PayStatisticsView())
            {
                var data = view.GetDetail(arguments.begin, arguments.end, arguments.clientid, arguments.payer, arguments.currency);
                return Json(data, JsonRequestBehavior.DenyGet);
            }                
        }        
    }
}