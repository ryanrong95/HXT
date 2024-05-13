using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;

namespace MvcApi.Controllers
{
    public class cgSzSortingsController : Controller
    {
        /// <summary>
        /// 深圳入库展示
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Show(string id, int pageIndex = 1, int pageSize = 20)
        {
            using (var view = new CgSzSortingsView())
            {
                var status = new CgSortingExcuteStatus[] { CgSortingExcuteStatus.Sorting, CgSortingExcuteStatus.PartStocked, CgSortingExcuteStatus.Completed };
                var data = view.SearchByWareHouseID(id).SearchByShelved(true).SearchByStatus(status).
                    ToMyPage(pageIndex, pageSize);

                return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据前端的过滤条件, 获取对应的运单
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult Show(JPost jpost)
        {
            var arguments = new
            {
                whid = jpost["WhID"]?.Value<string>(),
                code = jpost["Code"]?.Value<string>(),
                lotnumber = jpost["LotNumber"]?.Value<string>(),
                carriername = jpost["CarrierName"]?.Value<string>(),
                carNumber = jpost["CarNumber"]?.Value<string>(),
                status = jpost["WaybillExcuteStatus"]?.Value<string>(),
                waybillid = jpost["WaybillID"]?.Value<string>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
                startDate = jpost["StartDate"]?.Value<string>(),
                endDate = jpost["EndDate"]?.Value<string>(),
                tinyOrderID = jpost["TinyOrderID"]?.Value<string>(),
                isHandled = jpost["Status"]?.Value<bool>() ?? true,
            };

            var view = new CgSzSortingsView();

            if (!string.IsNullOrEmpty(arguments.whid))
            {
                view = view.SearchByWareHouseID(arguments.whid);
            }

            if (!string.IsNullOrEmpty(arguments.code))
            {
                view = view.SearchByCode(arguments.code);
            }

            if (!string.IsNullOrEmpty(arguments.lotnumber))
            {
                view = view.SearchByLotNumber(arguments.lotnumber);
            }

            if (!string.IsNullOrEmpty(arguments.tinyOrderID))
            {
                view = view.SearchByTinyOrderID(arguments.tinyOrderID);
            }

            if (!string.IsNullOrEmpty(arguments.carriername))
            {
                view = view.SearchByCarrierName(arguments.carriername);
            }

            if (!string.IsNullOrEmpty(arguments.carNumber))
            {
                view = view.SearchByCarNumber(arguments.carNumber);
            }

            if (!string.IsNullOrEmpty(arguments.status))
            {
                string[] status = arguments.status.Split(',');

                if (status.Contains(((int)(CgSortingExcuteStatus.All)).ToString()))
                {
                    view = view.SearchByStatus(new CgSortingExcuteStatus[] { CgSortingExcuteStatus.Sorting, CgSortingExcuteStatus.PartStocked, CgSortingExcuteStatus.Anomalous, CgSortingExcuteStatus.Completed });
                }
                else
                {
                    var statuslist = ExtendsEnum.ToArray<CgSortingExcuteStatus>().Where(item => status.Contains(((int)item).ToString())).ToArray();
                    view = view.SearchByStatus(statuslist);
                }
            }

            if (!string.IsNullOrEmpty(arguments.startDate) && !string.IsNullOrEmpty(arguments.endDate))
            {
                view = view.SearchByDate(DateTime.Parse(arguments.startDate), DateTime.Parse(arguments.endDate));
            }

            var result = view.SearchByShelved(arguments.isHandled).ToMyPage(arguments.pageIndex, arguments.pageSize);

            return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据id -- waybillID获取运单的详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            var data = new CgSzSortingsView().SearchByWaybillID(id).ToMyArray();
            return Json(data[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据id -- waybillID获取当前运单打印的详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DetailPrint(string id, string adminID)
        {
            var data = new CgSzSortingsView().SearchByWaybillID(id).ToPrint(id, adminID);
            var result = data as object[];
            return Json(result[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新上架信息
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult Update(JPost jpost)
        {
            using (var view = new CgSzSortingsView())
            {
                var updateInfo = jpost["Update"];

                view.UpdateStorageShelve(updateInfo);

                return Json(new
                {
                    Success = true,
                    Data = string.Empty
                });
            }

        }
    }
}