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
    /// 出入库报告
    /// </summary>
    public class ReportsController : Controller
    {
        /// <summary>
        /// 入库报告
        /// </summary>
        /// <param name="jPost">前端过滤条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Inbound(JPost jpost)
        {
            var args = new
            {
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var reports = new Services.Views.Reports_Show_View().SearchByType(Services.Enums.ReportType.Inbound);
            var query = reports.ToMyPage(args.PageIndex, args.PageSize);

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 出库报告
        /// </summary>
        /// <param name="jPost">前端过滤条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Outbound(JPost jpost)
        {
            var args = new
            {
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var reports = new Services.Views.Reports_Show_View().SearchByType(Services.Enums.ReportType.Outbound);
            var query = reports.ToMyPage(args.PageIndex, args.PageSize);

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 报告详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            var items = new Services.Views.ReportItemsView().Where(t => t.ReportID == id);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 入库报告（按产品展示）
        /// </summary>
        /// <param name="jPost">前端过滤条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Product_Inbound(JPost jpost)
        {
            var args = new
            {
                PartNumber = jpost["PartNumber"]?.Value<string>(),
                //Brand = jpost["Brand"]?.Value<string>(),
                FormID = jpost["FormID"]?.Value<string>(),
                Start = jpost["Start"]?.Value<string>(),
                End = jpost["End"]?.Value<string>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
                Status = jpost["Status"]?.Value<bool>(),
            };

            var reports = new Services.Views.ReportItems_In_View();
            if (!string.IsNullOrWhiteSpace(args.PartNumber))
            {
                reports = reports.SearchByPartnumber(args.PartNumber.Trim());
            }
            //if (!string.IsNullOrWhiteSpace(args.Brand))
            //{
            //    reports = reports.SearchByBrand(args.Brand.Trim());
            //}

            if (!string.IsNullOrEmpty(args.FormID))
            {
                reports = reports.SearchByFormID(args.FormID.Trim());
            }

            if (!string.IsNullOrWhiteSpace(args.Start) && !string.IsNullOrWhiteSpace(args.End))
            {
                reports = reports.SearchByDate(DateTime.Parse(args.Start), DateTime.Parse(args.End));
            }

            if (args.Status.HasValue)
            {
                reports = reports.SearchByNormalStatus(args.Status.Value);
            }

            var query = reports.ToMyPage(args.PageIndex, args.PageSize);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 出库报告（按产品展示）
        /// </summary>
        /// <param name="jPost">前端过滤条件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Product_Outbound(JPost jpost)
        {
            var args = new
            {
                PartNumber = jpost["PartNumber"]?.Value<string>(),
                Brand = jpost["Brand"]?.Value<string>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var reports = new Services.Views.ReportItems_Out_View();
            if (!string.IsNullOrWhiteSpace(args.PartNumber))
            {
                reports = reports.SearchByPartnumber(args.PartNumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Brand))
            {
                reports = reports.SearchByBrand(args.Brand.Trim());
            }

            var query = reports.ToMyPage(args.PageIndex,args.PageSize);

            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}