using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;
using Wms.Services.chonggous.Views;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;

namespace MvcApi.Controllers
{
    public class cgInputReportController : Controller
    {
        /// <summary>
        /// 入库报告
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult InputReport(JPost jpost)
        {
            var WarehouseID = jpost["WarehouseID"]?.Value<string>();
            var OrderID = jpost["OrderID"]?.Value<string>();
            var StartDate = jpost["StartDate"]?.Value<DateTime?>();
            var EndDate = jpost["EndDate"]?.Value<DateTime?>();
            var Source = jpost["Source"]?.Value<int?>();
            var PartNumber = jpost["PartNumber"]?.Value<string>();
            var Manufacturer = jpost["Manufacturer"]?.Value<string>();
            var Supplier = jpost["Supplier"]?.Value<string>();
            var ClientID = jpost["ClientID"]?.Value<string>();
            var ClientName = jpost["ClientName"]?.Value<string>();
            var EnterCode = jpost["EnterCode"]?.Value<string>();

            using (CgInputReportsView view = new CgInputReportsView())
            {
                var data = view.SearchByWareHouseID(WarehouseID);

                if (!string.IsNullOrEmpty(OrderID))
                {
                    data = data.SearchByOrderID(OrderID);
                }

                if (StartDate.HasValue || EndDate.HasValue)
                {
                    data = data.SearchByEnterDate(StartDate, EndDate);
                }
                if (Source.HasValue)
                {
                    data = data.SearchBySource((CgNoticeSource)Source);
                }
                if (!string.IsNullOrEmpty(PartNumber))
                {
                    data = data.SearchByPartNumber(PartNumber);
                }
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    data = data.SearchByManufacturer(Manufacturer);
                }
                if (!string.IsNullOrEmpty(ClientID))
                {
                    data = data.SearchByClientID(ClientID);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    data = data.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(EnterCode))
                {
                    data = data.SearchByEnterCode(EnterCode);
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 入库报告分组
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult InputReportGroup(JPost jpost)
        {

            var WarehouseID = jpost["WarehouseID"]?.Value<string>();
            var OrderID = jpost["OrderID"]?.Value<string>();
            var StartDate = jpost["StartDate"]?.Value<DateTime?>();
            var EndDate = jpost["EndDate"]?.Value<DateTime?>();
            var Source = jpost["Source"]?.Value<int?>();
            var PartNumber = jpost["PartNumber"]?.Value<string>();
            var Manufacturer = jpost["Manufacturer"]?.Value<string>();
            var Supplier = jpost["Supplier"]?.Value<string>();
            var ClientID = jpost["ClientID"]?.Value<string>();
            var ClientName = jpost["ClientName"]?.Value<string>();
            var EnterCode = jpost["EnterCode"]?.Value<string>();
            var PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1;
            var PageSize = jpost["PageSize"]?.Value<int?>() ?? 20;
            var Status = jpost["Status"]?.Value<bool?>() ?? true;
            var AdminName = jpost["AdminName"]?.Value<string>();

            using (CgInputReportsView view = new CgInputReportsView())
            {
                var data = view.SearchByWareHouseID(WarehouseID);
                if (!string.IsNullOrEmpty(OrderID))
                {
                    data = data.SearchByOrderID(OrderID);
                }
                if (StartDate.HasValue || EndDate.HasValue)
                {
                    data = data.SearchByEnterDate(StartDate, EndDate);
                }
                if (Source.HasValue)
                {
                    if (Source.Value == 1)
                    {
                        if (WarehouseID.StartsWith("HK"))
                        {
                            data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentEnter, CgNoticeSource.Transfer });
                        }
                        else
                        {
                            data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentBreakCustoms, CgNoticeSource.AgentBreakCustomsForIns, CgNoticeSource.AgentCustomsFromStorage });
                        }
                    }
                    else
                    {
                        data = data.SearchBySource((CgNoticeSource)Source);
                    }

                }
                if (!string.IsNullOrEmpty(PartNumber))
                {
                    data = data.SearchByPartNumber(PartNumber);
                }
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    data = data.SearchByManufacturer(Manufacturer);
                }
                if (!string.IsNullOrEmpty(ClientID))
                {
                    data = data.SearchByClientID(ClientID);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    data = data.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(EnterCode))
                {
                    data = data.SearchByEnterCode(EnterCode);
                }
                if (!string.IsNullOrEmpty(AdminName))
                {
                    data = data.SearchByAdminName(AdminName);
                }

                return Json(data.ToMyPage(Status, PageIndex, PageSize), JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 订单的入库报告
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult OrderInputReport(string warehouseId, string orderId)
        {
            using (CgInputReportsView view = new CgInputReportsView())
            {
                var data = view.SearchByWareHouseID(warehouseId).SearchByOrderID(orderId).ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 订单的入库报告分组
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult OrderInputReportGroup(string warehouseId, string orderId, bool status = true)
        {
            using (CgInputReportsView view = new CgInputReportsView())
            {
                var data = view.ToGroupByInput(view.SearchByWareHouseID(warehouseId).SearchByOrderID(orderId), status);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TinyOrderIDReportGroup(string warehouseId, string orderId, bool status = true)
        {
            using (CgInputReportsView view = new CgInputReportsView())
            {
                var data = view.ToGroupByTinyOrderID(view.SearchByWareHouseID(warehouseId).SearchByOrderID(orderId));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class cgCustomsStorageReportController : Controller
    {
        /// <summary>
        /// 拣货报告分组
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult CustomsStorageReportGroup(JPost jpost)
        {
            var OrderID = jpost["OrderID"]?.Value<string>();
            var StartDate = jpost["StartDate"]?.Value<DateTime?>();
            var EndDate = jpost["EndDate"]?.Value<DateTime?>();
            var Source = jpost["Source"]?.Value<int?>();
            var PartNumber = jpost["PartNumber"]?.Value<string>();
            var Manufacturer = jpost["Manufacturer"]?.Value<string>();
            var Supplier = jpost["Supplier"]?.Value<string>();
            var ClientID = jpost["ClientID"]?.Value<string>();
            var ClientName = jpost["ClientName"]?.Value<string>();
            var EnterCode = jpost["EnterCode"]?.Value<string>();
            var PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1;
            var PageSize = jpost["PageSize"]?.Value<int?>() ?? 20;
            var Status = jpost["Status"]?.Value<bool?>() ?? true;
            var AdminName = jpost["AdminName"]?.Value<string>();

            using (CgCustomsStorageReportsView view = new CgCustomsStorageReportsView())
            {
                var data = view;
                if (!string.IsNullOrEmpty(OrderID))
                {
                    data = data.SearchByOrderID(OrderID);
                }
                if (StartDate.HasValue || EndDate.HasValue)
                {
                    data = data.SearchByEnterDate(StartDate, EndDate);
                }
                if (Source.HasValue)
                {
                    if (Source.Value == 1)
                    {   
                        data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentBreakCustoms });                        
                    }
                    else
                    {
                        data = data.SearchBySource((CgNoticeSource)Source);
                    }

                }
                if (!string.IsNullOrEmpty(PartNumber))
                {
                    data = data.SearchByPartNumber(PartNumber);
                }
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    data = data.SearchByManufacturer(Manufacturer);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    data = data.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(EnterCode))
                {
                    data = data.SearchByEnterCode(EnterCode);
                }
                if (!string.IsNullOrEmpty(AdminName))
                {
                    data = data.SearchByAdminName(AdminName);
                }

                return Json(data.ToMyPage(Status, PageIndex, PageSize), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult TinyOrderIDReportGroup(string orderId, bool status = true)
        {
            using (CgCustomsStorageReportsView view = new CgCustomsStorageReportsView())
            {
                var data = view.ToGroupByTinyOrderID(view.SearchByOrderID(orderId));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class cgOutputReportController : Controller
    {
        /// <summary>
        /// 出库报告
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult OutputReport(JPost jpost)
        {

            var WarehouseID = jpost["WarehouseID"]?.Value<string>();
            var OrderID = jpost["OrderID"]?.Value<string>();
            var StartDate = jpost["StartDate"]?.Value<DateTime?>();
            var EndDate = jpost["EndDate"]?.Value<DateTime?>();
            var Source = jpost["Source"]?.Value<int?>();
            var PartNumber = jpost["PartNumber"]?.Value<string>();
            var Manufacturer = jpost["Manufacturer"]?.Value<string>();
            var Supplier = jpost["Supplier"]?.Value<string>();
            var ClientID = jpost["ClientID"]?.Value<string>();
            var ClientName = jpost["ClientName"]?.Value<string>();
            var EnterCode = jpost["EnterCode"]?.Value<string>();
            var PageIndex = jpost["PageIndex"]?.Value<int>() ?? 1;
            var PageSize = jpost["PageSize"]?.Value<int>() ?? 20;
            var AdminName = jpost["AdminName"]?.Value<string>();

            using (CgOutputReportsView view = new CgOutputReportsView())
            {
                if (string.IsNullOrEmpty(WarehouseID))
                {
                    throw new Exception("库房ID不能为空");
                }
                var data = view.SearchByWareHouseID(WarehouseID);
                if (!string.IsNullOrEmpty(OrderID))
                {
                    data = data.SearchByOrderID(OrderID);
                }
                if (StartDate.HasValue || EndDate.HasValue)
                {
                    data = data.SearchByExitDate(StartDate, EndDate);
                }
                if (Source.HasValue)
                {
                    if (Source.Value == 1)
                    {
                        if (WarehouseID.StartsWith("HK"))
                        {
                            data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentSend, CgNoticeSource.Transfer });
                        }
                        else
                        {
                            data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentBreakCustoms, CgNoticeSource.AgentBreakCustomsForIns, CgNoticeSource.AgentCustomsFromStorage });
                        }
                    }
                    else
                    {
                        data = data.SearchBySource((CgNoticeSource)Source);
                    }
                }
                if (!string.IsNullOrEmpty(PartNumber))
                {
                    data = data.SearchByPartNumber(PartNumber);
                }
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    data = data.SearchByManufacturer(Manufacturer);
                }
                if (!string.IsNullOrEmpty(ClientID))
                {
                    data = data.SearchByClientID(ClientID);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    data = data.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(EnterCode))
                {
                    data = data.SearchByEnterCode(EnterCode);
                }
                if (!string.IsNullOrEmpty(AdminName))
                {
                    data = data.SearchByAdminName(AdminName);
                }

                return Json(data.ToPage(PageIndex, PageSize), JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 出库报告分组
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult OutputReportGroup(JPost jpost)
        {

            var WarehouseID = jpost["WarehouseID"]?.Value<string>();
            var OrderID = jpost["OrderID"]?.Value<string>();
            var StartDate = jpost["StartDate"]?.Value<DateTime?>();
            var EndDate = jpost["EndDate"]?.Value<DateTime?>();
            var Source = jpost["Source"]?.Value<int?>();
            var PartNumber = jpost["PartNumber"]?.Value<string>();
            var Manufacturer = jpost["Manufacturer"]?.Value<string>();
            var Supplier = jpost["Supplier"]?.Value<string>();
            var ClientID = jpost["ClientID"]?.Value<string>();
            var ClientName = jpost["ClientName"]?.Value<string>();
            var EnterCode = jpost["EnterCode"]?.Value<string>();
            var PageIndex = jpost["PageIndex"]?.Value<int>() ?? 1;
            var PageSize = jpost["PageSize"]?.Value<int>() ?? 20;

            using (CgOutputReportsView view = new CgOutputReportsView())
            {
                var data = view.SearchByWareHouseID(WarehouseID);
                if (!string.IsNullOrEmpty(OrderID))
                {
                    data = data.SearchByOrderID(OrderID);
                }
                if (StartDate.HasValue || EndDate.HasValue)
                {
                    data = data.SearchByExitDate(StartDate, EndDate);
                }
                if (Source.HasValue)
                {
                    if (Source.Value == 1)
                    {
                        if (WarehouseID.StartsWith("HK"))
                        {
                            data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentSend, CgNoticeSource.Transfer, CgNoticeSource.AgentBreakCustoms, CgNoticeSource.AgentBreakCustomsForIns, CgNoticeSource.AgentCustomsFromStorage });
                        }
                        else
                        {
                            data = data.SearchBySource(new CgNoticeSource[] { CgNoticeSource.AgentBreakCustoms, CgNoticeSource.AgentBreakCustomsForIns, CgNoticeSource.AgentCustomsFromStorage });
                        }
                    }
                    else
                    {
                        data = data.SearchBySource((CgNoticeSource)Source);
                    }
                }
                if (!string.IsNullOrEmpty(PartNumber))
                {
                    data = data.SearchByPartNumber(PartNumber);
                }
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    data = data.SearchByManufacturer(Manufacturer);
                }
                if (!string.IsNullOrEmpty(ClientID))
                {
                    data = data.SearchByClientID(ClientID);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    data = data.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(EnterCode))
                {
                    data = data.SearchByEnterCode(EnterCode);
                }
                return Json(data.ToGroupPage(PageIndex, PageSize), JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 订单的出库报告
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult OrderOutputReport(string warehouseId, string orderId)
        {

            using (CgOutputReportsView view = new CgOutputReportsView())
            {
                var data = view.SearchByWareHouseID(warehouseId).SearchByOrderID(orderId).ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 订单的出库报告分组
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult OrderOutputReportGroup(string warehouseId, string orderId)
        {

            using (CgOutputReportsView view = new CgOutputReportsView())
            {
                var data = view.ToGroupByInput(view.SearchByWareHouseID(warehouseId).SearchByOrderID(orderId));
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
    }

    public class cgClientReportController : Controller
    {
        [HttpPayload]
        public ActionResult ClientReport(JPost jpost)
        {
            var parameters = new
            {
                WarehouseID = jpost["WarehouseID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                EnterCode = jpost["EnterCode"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PartNumber = jpost["PartNumber"]?.Value<string>(),
                Manufacturer = jpost["Manufacturer"]?.Value<string>(),
                PageIndex = jpost["PageIndex"]?.Value<int>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int>() ?? 20,
        };
            //var WarehouseID = jpost["WarehouseID"]?.Value<string>();
            //var ClientName = jpost["ClientName"]?.Value<string>();
            //var EnterCode = jpost["EnterCode"]?.Value<string>();
            //var StartDate = jpost["StartDate"]?.Value<DateTime?>();
            //var EndDate = jpost["EndDate"]?.Value<DateTime?>();
            //var PartNumber = jpost["PartNumber"]?.Value<string>();
            //var Manufacturer = jpost["Manufacturer"]?.Value<string>();

            using (CgClientReportsView view = new CgClientReportsView())
            {
                if (string.IsNullOrEmpty(parameters.WarehouseID))
                {
                    throw new Exception("库房ID不能为空");
                }
                var data = view.SearchByWareHouseID(parameters.WarehouseID);

                var startDate = (parameters.StartDate ?? DateTime.Now).Date;
                var endDate = (parameters.EndDate ?? DateTime.Now).Date;

                if (startDate == endDate)
                {
                    endDate = endDate.AddDays(1);
                }

                data = data.SearchByDate(startDate, endDate);


                if (!string.IsNullOrEmpty(parameters.Manufacturer))
                {
                    data = data.SearchByManufacturer(parameters.Manufacturer);
                }
                if (!string.IsNullOrEmpty(parameters.PartNumber))
                {
                    data = data.SearchByPartNumber(parameters.PartNumber);
                }
                if (!string.IsNullOrEmpty(parameters.ClientName))
                {
                    data = data.SearchByClientName(parameters.ClientName);
                }
                if (!string.IsNullOrEmpty(parameters.EnterCode))
                {
                    data = data.SearchByEnterCode(parameters.EnterCode);
                }

                var result = data.ToPage(parameters.PageIndex, parameters.PageSize);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}