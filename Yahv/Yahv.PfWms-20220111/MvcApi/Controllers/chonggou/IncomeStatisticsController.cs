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
    public class IncomeStatisticsController : Controller
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
                payee = jpost["s_payee"]?.Value<string>(),
                pageindex = jpost["s_pageindex"]?.Value<int?>() ?? 1,
                pagesize = jpost["s_pagesize"]?.Value<int?>() ?? 20,
            };

            using (var view = new IncomeStatisticsView())
            {
                var data = view.GetData(arguments.begin, arguments.end, arguments.payee, arguments.clientid, arguments.code, arguments.currency, arguments.pageindex, arguments.pagesize);
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
                payee = jpost["payee"]?.Value<string>(),
                currency = jpost["currency"]?.Value<string>(),
            };

            using (var view = new IncomeStatisticsView())
            {
                var data = view.GetDetail(arguments.begin, arguments.end, arguments.clientid, arguments.payee, arguments.currency);
                return Json(data, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 获取客户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClients()
        {
            var data = new IncomeStatisticsView().GetClients();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取币种
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrencies()
        {
            var data = new IncomeStatisticsView().GetCurrencies();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取内部公司
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCompanies()
        {
            var data = new IncomeStatisticsView().GetCompanies();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}