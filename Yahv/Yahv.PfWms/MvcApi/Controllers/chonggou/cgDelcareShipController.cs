using Yahv.Utils.EventExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;
using Yahv.Services.Views;
using MvcApi.Models;
using Yahv.Utils.Kdn;
using Kdn.Library;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;
using Wms.Services.chonggous.Views;
using Newtonsoft.Json.Linq;
using Wms.Services.chonggous.Models;

namespace MvcApp.Controllers
{
    /// <summary>
    /// 报关运输
    /// </summary>
    public class cgDelcareShipController : Controller
    {

        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="jpost">
        /// json 参数
        /// 只需要传递数组TinyOrderID数组
        /// </param>
        /// <returns>成功或是报错！</returns>
        [HttpPayload]
        public ActionResult GetList(JPost jpost)
        {
            var arguments = new
            {
                LotNumber = jpost["LotNumber"]?.Value<string>(),
                Carrier = jpost["Carrier"]?.Value<string>(),

                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),

                Status = jpost["Status"]?.Value<int>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            using (CgDelcareShipView view = new CgDelcareShipView())
            {
                CgDelcareShipView data = view;
                if (!string.IsNullOrWhiteSpace(arguments.LotNumber))
                {
                    data = data.SearchByLotNumber(arguments.LotNumber);
                }

                if (!string.IsNullOrWhiteSpace(arguments.Carrier))
                {
                    data = data.SearchByCarrier(arguments.Carrier);
                }

                if (arguments.EndDate.HasValue && arguments.StartDate.HasValue
                    && arguments.EndDate == arguments.StartDate)
                {
                    data = data.SearchByShipStartDate(arguments.StartDate.Value);
                    data = data.SearchByShipEndDate(arguments.StartDate.Value.AddDays(1));
                }
                else
                {
                    if (arguments.StartDate.HasValue)
                    {
                        data = data.SearchByShipStartDate(arguments.StartDate.Value);
                    }

                    if (arguments.EndDate.HasValue)
                    {
                        data = data.SearchByShipEndDate(arguments.EndDate.Value);
                    }
                }
                if (arguments.Status.HasValue)
                {
                    data = data.SearchByStatus((CgCuttingOrderStatus)arguments.Status);
                }

                var results = data.ToMyPage(arguments.pageIndex, arguments.pageSize);

                return Json(new
                {
                    obj = results
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取具体小订单信息
        /// </summary>
        /// <param name="id">小订单ID</param>
        /// <returns>小订单信息</returns>
        public ActionResult Detail(string id)
        {
            using (CgDelcareShipView data = new CgDelcareShipView())
            {
                var one = data.SearchByID(id);
                return Json(one, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 截单状态枚举
        /// </summary>
        /// <returns></returns>
        public ActionResult EnumCgCuttingOrderStatus()
        {
            return Json(ExtendsEnum.ForFrontEnd<CgCuttingOrderStatus>(), JsonRequestBehavior.AllowGet);
        }


   
    }
}
