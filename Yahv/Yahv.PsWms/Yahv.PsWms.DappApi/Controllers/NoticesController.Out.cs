using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.Linq.Extends;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Http;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    /// <summary>
    /// 出库通知
    /// </summary>
    public class OutNoticesController : NoticesController
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                var noticeModel = jpost.ToObject<NoticeModel>();

                //保存送货信息
                var consignee = noticeModel.Consignee;
                consignee.Enter();
                //保存通知信息
                var notice = new Notice();
                notice.WarehouseID = noticeModel.WarehouseID;
                notice.ClientID = noticeModel.ClientID;
                notice.CompanyID = noticeModel.CompanyID;
                notice.NoticeType = NoticeType.Outbound;
                notice.FormID = noticeModel.FormID;
                notice.WarehouseID = noticeModel.WarehouseID;
                notice.ConsigneeID = consignee.ID;
                notice.TrackerID = "Admin00057";
                notice.Enter();
                //保存订单项
                foreach (var item in noticeModel.Items)
                {
                    var product = item.Product;
                    product.Enter();

                    var noticeItem = new NoticeItem();
                    noticeItem.NoticeID = notice.ID;
                    noticeItem.Source = NoticeSource.Tracker;
                    noticeItem.ProductID = product.ID;
                    noticeItem.CustomCode = item.CustomCode;
                    noticeItem.StocktakingType = (StocktakingType)Enum.Parse(typeof(StocktakingType), item.StocktakingType);
                    noticeItem.Mpq = item.Mpq;
                    noticeItem.PackageNumber = item.PackageNumber;
                    noticeItem.Total = item.Total;
                    noticeItem.Currency = Currency.CNY;
                    noticeItem.UnitPrice = item.UnitPrice;
                    noticeItem.Supplier = item.Supplier;
                    noticeItem.ClientID = item.ClientID;
                    noticeItem.FormID = item.FormID;
                    noticeItem.FormItemID = item.FormItemID;
                    noticeItem.StorageID = item.StorageID;
                    noticeItem.ShelveID = item.ShelveID;
                    noticeItem.Enter();
                }
                //保存特殊要求
                foreach (var req in noticeModel.Requires)
                {
                    req.NoticeID = notice.ID;
                    req.NoticeTransportID = consignee.ID;
                    req.Enter();
                }

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ActionResult Detail(string id)
        {
            try
            {
                //通知
                var notice = new Services.Views.NoticesOutView().SearchByNoticeID(id).Single();
                //通知费用
                var charges = new Services.Views.ChargesView().Where(t => t.NoticeID == id);
                //通知项
                var items = new Services.Views.NoticeItems_Show_View().Where(t => t.NoticeID == notice.ID);
                //特殊要求
                var requires = new Services.Views.RequiresView().Where(t => t.NoticeID == id);
                //页面model
                var model = new
                {
                    Notice = notice,
                    Items = items,
                    Charges = charges,
                    Requres = requires,

                    NoticeTypeDec = notice.NoticeType.GetDescription(),
                    IDTypeDec = notice.Consignee?.TakerIDType == null ? "" : notice.Consignee.TakerIDType.GetDescription(),
                    TransportModeDec = notice.Consignee?.TransportMode == null ? "" : notice.Consignee.TransportMode.GetDescription(),
                    ExpressPayerDec = notice.Consignee?.ExpressPayer == null ? "" : notice.Consignee.ExpressPayer.GetDescription(),
                    ExpressTransportDec = notice.Consignee?.ExpressTransportDec,
                    ExpressTransportInt = notice.Consignee?.ExpressTransportInt,
                    PartnumberCount = items.Select(t => t.Partnumber).ToArray().Count(),
                };
                Response.StatusCode = 200;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 复核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Review(JPost jpost)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public override ActionResult Show(JPost jpost)
        {
            var args = new
            {
                Partnumber = jpost["Partnumber"]?.Value<string>(),
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View();

            if (!string.IsNullOrWhiteSpace(args.Partnumber))
            {
                notice = notice.SearchByPartnumber(args.Partnumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        #region 其它列表页面

        /// <summary>
        /// 待出库分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_UnExited(JPost jpost)
        {
            var args = new
            {
                Partnumber = jpost["Partnumber"]?.Value<string>(),
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().Notict_UnExited();

            if (!string.IsNullOrWhiteSpace(args.Partnumber))
            {
                notice = notice.SearchByPartnumber(args.Partnumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        /// <summary>
        /// 已出库分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Exited(JPost jpost)
        {
            var args = new
            {
                Partnumber = jpost["Partnumber"]?.Value<string>(),
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().Notict_Exited();

            if (!string.IsNullOrWhiteSpace(args.Partnumber))
            {
                notice = notice.SearchByPartnumber(args.Partnumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        /// <summary>
        /// 待提货分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_PickUp_NotArranged(JPost jpost)
        {
            var args = new
            {
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().NoticePickUp_NotArranged();

            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        /// <summary>
        /// 已提货分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_PickUp_Arranged(JPost jpost)
        {
            var args = new
            {
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().NoticePickUp_Arranged();

            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        /// <summary>
        /// 待安排分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Delivery_NotArranged(JPost jpost)
        {
            var args = new
            {
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().NoticeDelivery_NotArranged();

            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        /// <summary>
        /// 已安排分页列表
        /// </summary>
        /// /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Delivery_Arranged(JPost jpost)
        {
            var args = new
            {
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().NoticeDelivery_Arranged();

            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }

        /// <summary>
        /// 已完成分页列表
        /// </summary>
        /// /// <returns></returns>
        [HttpPost]
        public ActionResult Show_Delivery_Completed(JPost jpost)
        {
            var args = new
            {
                FormID = jpost["FormID"]?.Value<string>(),
                ClientName = jpost["ClientName"]?.Value<string>(),
                Status = jpost["Status"]?.Value<string>(),
                StartDate = jpost["StartDate"]?.Value<DateTime?>(),
                EndDate = jpost["EndDate"]?.Value<DateTime?>(),
                PageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                PageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            var notice = new Services.Views.NoticesOut_Show_View().NoticeDelivery_Completed();

            if (!string.IsNullOrWhiteSpace(args.FormID))
            {
                notice = notice.SearchByFromID(args.FormID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.ClientName))
            {
                notice = notice.SearchByClientName(args.ClientName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(args.Status))
            {
                var status = (NoticeStatus)Enum.Parse(typeof(NoticeStatus), args.Status.Trim());
                notice = notice.SearchByStatus(status);
            }
            if (args.StartDate != null || args.EndDate != null)
            {
                notice = notice.SearchByDate(args.StartDate, args.EndDate);
            }

            var query = notice.ToMyPage(args.PageIndex, args.PageSize);

            Response.StatusCode = 200;
            return Json(new { Data = query });
        }


        #endregion

        /// <summary>
        /// 出库提货（客户提货人）
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PickUp_Arranged(JPost jpost)
        {
            try
            {
                var args = new
                {
                    ID = jpost["ID"]?.Value<string>(),
                    TakerName = jpost["TakerName"]?.Value<string>(),
                    TakingTime = jpost["TakingTime"]?.Value<DateTime?>(),
                    TakerPhone = jpost["TakerPhone"]?.Value<string>(),
                    Address = jpost["Address"]?.Value<string>(),
                    TakerIDType = jpost["TakerIDType"]?.Value<string>(),
                    TakerIDCode = jpost["TakerIDCode"]?.Value<string>(),
                    Summary = jpost["Summary"]?.Value<string>(),
                };
                var consignee = new Services.Views.NoticeTransportsView().SingleOrDefault(t => t.ID == args.ID);
                if (consignee == null)
                {
                    throw new Exception("找不到出库通知对应的Consignee对象！！！");
                }
                consignee.TakerName = args.TakerName;
                consignee.TakingTime = args.TakingTime;
                consignee.TakerPhone = args.TakerPhone;
                consignee.TakerIDType = (Services.Enums.IDType)Enum.Parse(typeof(Services.Enums.IDType), args.TakerIDType);
                consignee.TakerIDCode = args.TakerIDCode;
                consignee.Address = args.Address;
                consignee.Summary = args.Summary;

                consignee.Enter();

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        /// <summary>
        /// 出库送货（送货司机）
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delivery_Arranged(JPost jpost)
        {
            try
            {
                var args = new
                {
                    ID = jpost["ID"]?.Value<string>(),
                    TakerName = jpost["TakerName"]?.Value<string>(),
                    TakingTime = jpost["TakingTime"]?.Value<DateTime?>(),
                    TakerPhone = jpost["TakerPhone"]?.Value<string>(),
                    Address = jpost["Address"]?.Value<string>(),
                    TakerLicense = jpost["TakerLicense"]?.Value<string>(),
                    Summary = jpost["Summary"]?.Value<string>(),
                };
                var consignee = new Services.Views.NoticeTransportsView().SingleOrDefault(t => t.ID == args.ID);
                if (consignee == null)
                {
                    throw new Exception("找不到出库通知对应的Consignee对象！！！");
                }
                consignee.TakerName = args.TakerName;
                consignee.TakingTime = args.TakingTime;
                consignee.TakerPhone = args.TakerPhone;
                consignee.TakerLicense = args.TakerLicense;
                consignee.Address = args.Address;
                consignee.Summary = args.Summary;

                consignee.Enter();

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        /// <summary>
        /// 快递送货（承运商变更）
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Express_Arranged(JPost jpost)
        {
            try
            {
                var args = new
                {
                    ID = jpost["ID"]?.Value<string>(),
                    Carrier = jpost["Carrier"]?.Value<string>(),
                    WaybillCode = jpost["WaybillCode"]?.Value<string>(),
                    TrackingCode = jpost["TrackingCode"]?.Value<string>(),
                    FreightPayer = jpost["FreightPayer"]?.Value<string>(),
                    ExpressTransport = jpost["ExpressTransport"]?.Value<string>(),
                    ExpressEscrow = jpost["ExpressEscrow"]?.Value<string>(),
                    Summary = jpost["Summary"]?.Value<string>(),
                };
                var consignee = new Services.Views.NoticeTransportsView().SingleOrDefault(t => t.ID == args.ID);
                if (consignee == null)
                {
                    throw new Exception("找不到出库通知对应的Consignee对象！！！");
                }
                consignee.Carrier = args.Carrier;
                consignee.WaybillCode = args.WaybillCode;
                consignee.TrackingCode = args.TrackingCode;
                consignee.ExpressPayer = (Services.Enums.FreightPayer)Enum.Parse(typeof(Services.Enums.FreightPayer), args.FreightPayer);
                consignee.ExpressTransport = args.ExpressTransport;
                consignee.ExpressEscrow = args.ExpressEscrow;
                consignee.Summary = args.Summary;

                consignee.Enter();

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        /// <summary>
        /// 打印预出库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Print_PreDeliveryFile(string id)
        {
            try
            {
                var notice = new Services.Views.NoticesOutView().Single(t => t.ID == id);
                if (notice.Status == NoticeStatus.Processing)
                {
                    notice.Status = NoticeStatus.Reviewing;
                    notice.Enter();
                }
                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        /// <summary>
        /// 上传客户签字文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload_CustomSignFile(string id)
        {
            try
            {
                var notice = new Services.Views.NoticesOutView().Single(t => t.ID == id);
                if (notice.Status == NoticeStatus.Arrivaling || notice.Status == NoticeStatus.Completed)
                {
                    notice.Status = NoticeStatus.Completed;
                    notice.Enter();

                    // 通知管理端更新订单状态
                    string url = Services.Enums.FromType.RealChangeOrder.GetDescription();
                    ApiHelper.Current.JPost(url, new { OrderID = notice.FormID, OrderStatus = 600 });
                }
                else
                {
                    throw new Exception("只有等待收货状态的订单才能修改为已完成状态");
                }
                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

        /// <summary>
        /// 拣货异常提交
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ItemException(JPost jpost)
        {
            try
            {
                var args = new
                {
                    ID = jpost["ID"]?.Value<string>(),
                    Exception = jpost["Exception"]?.Value<string>(),
                };
                var item = new Services.Views.NoticeItemsView().Single(t => t.ID == args.ID);
                item.Exception = args.Exception;
                item.Enter();

                Response.StatusCode = 200;
                return Json(new { Success = true, Data = string.Empty });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { Success = false, Data = ex.Message, });
            }
        }

    }
}