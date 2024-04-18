using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Notice;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class NoticeController : BaseController
    {
        #region 页面

        /// <summary>
        /// 手工发送通知页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Manual() { return View(); }

        #endregion

        /// <summary>
        /// 从当前数据中生成通知 Json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNoticeJsonFromCurrentData()
        {
            string OrderID = Request.Form["OrderID"];

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var order = repository.ReadTable<Layers.Data.Sqls.PsOrder.Orders>().Where(t => t.ID == OrderID).FirstOrDefault();

                if (order == null)
                {
                    return Json(new { type = "error", msg = "Orders 表中不存在该订单" }, JsonRequestBehavior.AllowGet);
                }

                var orderItems = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>()
                    .Where(t => t.OrderID == OrderID && t.Status == (int)GeneralStatus.Normal).ToArray();
                var productIDs = orderItems.Select(t => t.ProductID).ToArray();
                var products = repository.ReadTable<Layers.Data.Sqls.PsOrder.Products>().Where(t => productIDs.Contains(t.ID)).ToArray();
                var orderTransport = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>().Where(t => t.OrderID == OrderID).FirstOrDefault();
                var requires = repository.ReadTable<Layers.Data.Sqls.PsOrder.Requires>().Where(t => t.OrderID == OrderID).ToArray();
                var files = repository.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>().Where(t => t.MainID == OrderID).ToArray();

                var client = repository.ReadTable<Layers.Data.Sqls.PsOrder.Clients>().Where(t => t.ID == order.ClientID).FirstOrDefault();

                string strJson = string.Empty;

                if (order.Type == (int)Services.Enums.OrderType.Inbound)
                {
                    StorageInNoticeService noticeService = new StorageInNoticeService();
                    noticeService.Order = order;
                    noticeService.OrderItems = orderItems;
                    noticeService.Products = products;
                    noticeService.OrderTransport = orderTransport;
                    noticeService.Requires = requires;
                    noticeService.Files = files;
                    //strJson = noticeService.GenerateJson(client.TrackerID);
                    strJson = noticeService.GenerateJsonNew(client.TrackerID);
                }
                else if (order.Type == (int)Services.Enums.OrderType.Outbound)
                {
                    //var picker = repository.ReadTable<Layers.Data.Sqls.PsOrder.Pickers>().Where(t => t.ID == orderTransport.PickerID).FirstOrDefault();

                    StorageOutNoticeService noticeService = new StorageOutNoticeService();
                    noticeService.Order = order;
                    noticeService.OrderItems = orderItems;
                    noticeService.Products = products;
                    noticeService.OrderTransport = orderTransport;
                    noticeService.Requires = requires;
                    //noticeService.Picker = picker;
                    noticeService.FileInfos = files;
                    strJson = noticeService.GenerateJson(client.TrackerID);
                }

                return new DebugJsonResult(strJson);
            }
        }

        public JsonResult GetNoticeJsonFromCurrentDataFile()
        {
            string OrderID = Request.Form["OrderID"];

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var order = repository.ReadTable<Layers.Data.Sqls.PsOrder.Orders>().Where(t => t.ID == OrderID).FirstOrDefault();

                if (order == null)
                {
                    return Json(new { type = "error", msg = "Orders 表中不存在该订单" }, JsonRequestBehavior.AllowGet);
                }

                var orderItems = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>()
                    .Where(t => t.OrderID == OrderID && t.Status == (int)GeneralStatus.Normal).ToArray();
                var productIDs = orderItems.Select(t => t.ProductID).ToArray();
                var products = repository.ReadTable<Layers.Data.Sqls.PsOrder.Products>().Where(t => productIDs.Contains(t.ID)).ToArray();
                var orderTransport = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>().Where(t => t.OrderID == OrderID).FirstOrDefault();
                var requires = repository.ReadTable<Layers.Data.Sqls.PsOrder.Requires>().Where(t => t.OrderID == OrderID).ToArray();
                var files = repository.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>().Where(t => t.MainID == OrderID).ToArray();

                var client = repository.ReadTable<Layers.Data.Sqls.PsOrder.Clients>().Where(t => t.ID == order.ClientID).FirstOrDefault();

                string strJson = string.Empty;

                if (order.Type == (int)Services.Enums.OrderType.Inbound)
                {
                    StorageInNoticeService noticeService = new StorageInNoticeService();
                    noticeService.Order = order;
                    noticeService.OrderItems = orderItems;
                    noticeService.Products = products;
                    noticeService.OrderTransport = orderTransport;
                    noticeService.Requires = requires;
                    noticeService.Files = files;
                    strJson = noticeService.GenerateJsonFile();
                }
                else if (order.Type == (int)Services.Enums.OrderType.Outbound)
                {
                    StorageOutNoticeService noticeService = new StorageOutNoticeService();
                    noticeService.Order = order;
                    noticeService.OrderItems = orderItems;
                    noticeService.Products = products;
                    noticeService.OrderTransport = orderTransport;
                    noticeService.Requires = requires;
                    //noticeService.Picker = picker;
                    noticeService.FileInfos = files;
                    strJson = noticeService.GenerateJsonFile();
                }

                return new DebugJsonResult(strJson);
            }
        }

        /// <summary>
        ///  发送通知
        /// </summary>
        /// <returns></returns>
        public JsonResult SendNotice()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];
                string StrJson = Request.Form["StrJson"];

                Services.Enums.OrderType orderType;

                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    var order = repository.ReadTable<Layers.Data.Sqls.PsOrder.Orders>().Where(t => t.ID == OrderID).FirstOrDefault();
                    orderType = (Services.Enums.OrderType)order.Type;
                }

                string strJsonResponse = string.Empty;

                if (orderType == Services.Enums.OrderType.Inbound)
                {
                    StorageInNoticeService noticeService = new StorageInNoticeService();
                    noticeService.StrJsonReq = StrJson;
                    noticeService.SendNotice(OrderID);
                    strJsonResponse = noticeService.StrJsonRes;
                }
                else if (orderType == Services.Enums.OrderType.Outbound)
                {
                    StorageOutNoticeService noticeService = new StorageOutNoticeService();
                    noticeService.StrJsonReq = StrJson;
                    noticeService.SendNotice(OrderID);
                    strJsonResponse = noticeService.StrJsonRes;
                }

                return Json(new { type = "success", msg = strJsonResponse, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}