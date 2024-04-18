using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 入库提货通知
    /// </summary>
    public class InPlanNoticesController : NoticesController
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        sealed public override ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                whid = jpost["WarehouseID"]?.Value<string>(),
                status = jpost["Status"]?.Value<int?>(), // 1 自提待安排, 2 自提已安排
                formID = jpost["FormID"]?.Value<string>(),
                clientName = jpost["ClientName"]?.Value<string>(),
                start = jpost["Start"]?.Value<string>(),
                end = jpost["End"]?.Value<string>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 50,
            };

            using (var view = new NoticesInPlanView())
            {
                var data = view;

                if (!string.IsNullOrWhiteSpace(arguments.whid))
                {
                    data = data.SearchByWarehouseID(arguments.whid.ToLower());
                }

                if (arguments.status.HasValue)
                {
                    if (arguments.status.Value == 1)
                    {
                        data = data.SearchByStatus(true);
                    }
                    else
                    {
                        data = data.SearchByStatus(false);
                    }                    
                }

                if(!string.IsNullOrEmpty(arguments.formID))
                {
                    data = data.SearchByFormID(arguments.formID.Trim());
                }

                if (!string.IsNullOrWhiteSpace(arguments.clientName))
                {
                    data = data.SearchByClientName(arguments.clientName);
                }

                if (!string.IsNullOrWhiteSpace(arguments.start) && !string.IsNullOrWhiteSpace(arguments.end))
                {
                    data = data.SearchByDate(DateTime.Parse(arguments.start), DateTime.Parse(arguments.end));
                }

                var result = data.ToMyPage(arguments.pageIndex, arguments.pageSize);
                return Json(new
                {
                    success = true,
                    code = 200,
                    data = result
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">ID值为NoticeID</param>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Detail(string id)
        {            
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = "查询的参数不能为null或空字符串"
                }, JsonRequestBehavior.DenyGet);
            }
            using (var view = new NoticesInPlanView())
            {
                var data = view;

                var result = data.SearchByNoticeID(id).Single();
                return Json(new
                {
                    success = true,
                    code = 200,
                    data = result,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 复核
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public override ActionResult Review(JPost jpost)
        {
            // 入库提货安排不需要复核，不需要实现
            throw new NotImplementedException();
        }

        /// <summary>
        /// 入库安排
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Arrange(JPost jpost)
        {
            try
            {
                using (NoticesInPlanView view = new NoticesInPlanView())
                {
                    view.Arrange(jpost);

                    return Json(new JMessage
                    {
                        success = true,
                        code = 200,
                        data = string.Empty
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    code = 400,
                    data = ex.Message
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}