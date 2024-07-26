using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Services;
using Needs.Ccs.Services.Views;
using Needs.Linq.Collections;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Wl.Warehouse.Services.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HKWarehouseNewController : MyApiController
    {
        #region 代装箱、已封箱列表数据      
        /// <summary>
        /// 代装箱列表数据
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult UnSortingList(JPost jpost)
        {
            if (jpost["OPTIONS"] == "True")
                return null;

            int page = jpost["PageIndex"];
            int rows = jpost["PageSize"];
            string OrderID = jpost["OrderID"];
            string ClientCode = jpost["ClientCode"];
            string SupplierName = jpost["SupplierName"];
            string HKDeliveryType = jpost["HKDeliveryType"];
            string StartDate = jpost["StartDate"];
            string EndDate = jpost["EndDate"];
            string Model = jpost["Model"];
            string WaybillNo = jpost["WaybillNo"];

            if (!string.IsNullOrWhiteSpace(Model))
            {
                using (var query = new Needs.Ccs.Services.Views.EntryNoticesWithItemsAllNew())
                {
                    var view = query;

                    view = view.SearchUnSealed();
                    view = view.SearchByModel(Model.Trim());

                    var result = view.ToMyPage(page, rows);
                    return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (!string.IsNullOrEmpty(WaybillNo))
            {
                using (var query = new Needs.Ccs.Services.Views.EntryNoticeWaybill())
                {
                    var view = query;

                    view = view.SearchUnSealed();
                    view = view.SearchByWaybillNo(WaybillNo.Trim());

                    var result = view.ToMyPage(page, rows);
                    return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                using (var query = new Needs.Ccs.Services.Views.EntryNoticesAllNew())
                {
                    var view = query;

                    view = view.SearchUnSealed();

                    if (!string.IsNullOrWhiteSpace(OrderID))
                    {
                        OrderID = OrderID.Trim();
                        view = view.SearchByOrderID(OrderID);
                    }

                    if (!string.IsNullOrWhiteSpace(ClientCode))
                    {
                        ClientCode = ClientCode.Trim();
                        view = view.SearchByClientCode(ClientCode);
                    }

                    if (!string.IsNullOrWhiteSpace(SupplierName))
                    {
                        SupplierName = SupplierName.Trim();
                        view = view.SearchBySupplierName(SupplierName);
                    }

                    if (!string.IsNullOrWhiteSpace(HKDeliveryType))
                    {
                        int intHKDeliveryType = 1;
                        if (int.TryParse(HKDeliveryType, out intHKDeliveryType))
                        {
                            view = view.SearchByHKDeliveryType((HKDeliveryType)intHKDeliveryType);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(StartDate))
                    {
                        StartDate = StartDate.Trim();
                        DateTime dtStart = Convert.ToDateTime(StartDate);
                        view = view.SearchByStartDate(dtStart);
                    }

                    if (!string.IsNullOrWhiteSpace(EndDate))
                    {
                        EndDate = EndDate.Trim();
                        DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                        view = view.SearchByEndDate(dtEnd);
                    }

                    var result = view.ToMyPage(page, rows);
                    return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
                }
            }

        }

        /// <summary>
        /// 已封箱列表数据
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult SealedList(JPost jpost)
        {
            if (jpost["OPTIONS"] == "True")
                return null;

            int page = jpost["PageIndex"];
            int rows = jpost["PageSize"];
            string OrderID = jpost["OrderID"];
            string ClientCode = jpost["ClientCode"];
            string SupplierName = jpost["SupplierName"];
            string HKDeliveryType = jpost["HKDeliveryType"];
            string StartDate = jpost["StartDate"];
            string EndDate = jpost["EndDate"];
            string Model = jpost["Model"];
            string WaybillNo = jpost["WaybillNo"];

            if (!string.IsNullOrWhiteSpace(Model))
            {
                using (var query = new Needs.Ccs.Services.Views.EntryNoticesWithItemsAllNew())
                {
                    var view = query;

                    view = view.SearchSealed();
                    view = view.SearchByModel(Model.Trim());

                    var result = view.ToMyPage(page, rows);
                    return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (!string.IsNullOrWhiteSpace(WaybillNo))
            {
                using (var query = new Needs.Ccs.Services.Views.EntryNoticeWaybill())
                {
                    var view = query;

                    view = view.SearchSealed();
                    view = view.SearchByWaybillNo(WaybillNo.Trim());

                    var result = view.ToMyPage(page, rows);
                    return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                using (var query = new Needs.Ccs.Services.Views.EntryNoticesAllNew())
                {
                    var view = query;

                    view = view.SearchSealed();

                    if (!string.IsNullOrWhiteSpace(OrderID))
                    {
                        OrderID = OrderID.Trim();
                        view = view.SearchByOrderID(OrderID);
                    }

                    if (!string.IsNullOrWhiteSpace(ClientCode))
                    {
                        ClientCode = ClientCode.Trim();
                        view = view.SearchByClientCode(ClientCode);
                    }

                    if (!string.IsNullOrWhiteSpace(SupplierName))
                    {
                        SupplierName = SupplierName.Trim();
                        view = view.SearchBySupplierName(SupplierName);
                    }

                    if (!string.IsNullOrWhiteSpace(HKDeliveryType))
                    {
                        int intHKDeliveryType = 1;
                        if (int.TryParse(HKDeliveryType, out intHKDeliveryType))
                        {
                            view = view.SearchByHKDeliveryType((HKDeliveryType)intHKDeliveryType);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(StartDate))
                    {
                        StartDate = StartDate.Trim();
                        DateTime dtStart = Convert.ToDateTime(StartDate);
                        view = view.SearchByStartDate(dtStart);
                    }

                    if (!string.IsNullOrWhiteSpace(EndDate))
                    {
                        EndDate = EndDate.Trim();
                        DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                        view = view.SearchByEndDate(dtEnd);
                    }

                    var result = view.ToMyPage(page, rows);
                    return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
                }
            }

        }
        #endregion

        #region 代报关装箱基础信息
        /// <summary>
        /// 代报关装箱基础信息
        /// </summary>
        /// <param name="entryNoticeID"></param>
        /// <returns></returns>
        public ActionResult SortingBasicInfo(string entryNoticeID)
        {
            if (string.IsNullOrEmpty(entryNoticeID))
                return null;

            BasicInfo basicInfo = new BasicInfo();
            basicInfo.EntryNoticeID = entryNoticeID;

            var entryNotice = new EntryNoticeView()[entryNoticeID];
            if (entryNotice != null)
            {
                basicInfo.EntryNoticeStatus = entryNotice.EntryNoticeStatus;
                //订单特殊类型
                //var orderVoyage = new OrderVoyagesOriginView().Where(t => t.Order.ID == entryNotice.Order.ID);
                //string spcialType = "";
                //foreach (var item in orderVoyage)
                //{
                //    spcialType += item.Type.GetDescription() + "|";
                //}
                basicInfo.ClientCode = entryNotice.Order.Client.ClientCode;
                basicInfo.ClientName = entryNotice.Order.Client.Company.Name;
                basicInfo.ClientRank = entryNotice.Order.Client.ClientRank;
                basicInfo.ChargeWH = entryNotice.Order.Client.ChargeWH;
                basicInfo.ChargeType = entryNotice.Order.Client.ChargeType == null ? "" : entryNotice.Order.Client.ChargeType.GetDescription();//新增的
                basicInfo.AmountWH = entryNotice.Order.Client.AmountWH ?? 0M;

                basicInfo.OrderID = entryNotice.Order.ID;
                basicInfo.MainOrderID = entryNotice.Order.MainOrderID;
                basicInfo.Merchandiser = entryNotice.Order.Client.Merchandiser;
                basicInfo.OrderCreateDate = entryNotice.Order.CreateDate;

                var orderConsignee = new OrderConsigneesView().Where(t => t.OrderID == entryNotice.Order.ID).FirstOrDefault();
                basicInfo.ClientSupplierName = orderConsignee.ClientSupplier.ChineseName;
                basicInfo.ClientSupplierGrade = orderConsignee.ClientSupplier.SupplierGrade;
                basicInfo.HKDeliveryType = orderConsignee.Type;

                basicInfo.PickUpTime = orderConsignee.PickUpTime?.ToString("yyyy-MM-dd");
                basicInfo.PickUpAddress = orderConsignee.Address;
                basicInfo.PickUpContactName = orderConsignee.Contact;
                basicInfo.PickUpContactPhone = orderConsignee.Tel + " " + orderConsignee.Mobile;

                basicInfo.CarrierID = orderConsignee.CarrierID;
                basicInfo.DriverID = orderConsignee.DriverID;
                basicInfo.CarNumber = orderConsignee.CarNumber;
                basicInfo.WayBillCode = orderConsignee.WayBillNo;
                basicInfo.PickupFiles = new List<PickupFile>();

                var Files = new CenterFilesTopView().Where(t => t.WsOrderID == basicInfo.MainOrderID && t.Type == (int)FileType.DeliveryFiles).FirstOrDefault();
                if (Files != null)
                {
                    PickupFile file = new PickupFile();
                    file.FileName = Files.CustomName;
                    file.FileUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"] + "/" + Files?.Url.ToUrl();
                    basicInfo.PickupFiles.Add(file);
                }

                if (basicInfo.HKDeliveryType == HKDeliveryType.PickUp)
                {
                    var deliveryNotice = new Needs.Ccs.Services.Views.DeliveryNoticeView().Where(t => t.Order.ID == basicInfo.OrderID).FirstOrDefault();
                    if (deliveryNotice != null)
                    {
                        basicInfo.DeliveryNoticeStatus = deliveryNotice.DeliveryNoticeStatus;
                    }
                }

                basicInfo.StorageFiles = new List<PickupFile>();
                var StorageFiles = new CenterFilesTopView().Where(t => t.WsOrderID == basicInfo.MainOrderID && t.Type == (int)FileType.StoragesPic).ToList();
                foreach (var item in StorageFiles)
                {
                    PickupFile file = new PickupFile();
                    file.ID = item.ID;
                    file.FileName = item.CustomName;
                    file.FileUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"] + "/" + item?.Url.ToUrl();
                    file.FileType = (int)FileType.StoragesPic;
                    basicInfo.StorageFiles.Add(file);
                }
            }

            return Json(new { obj = basicInfo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新香港收货信息
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult HKWaybillUpdate(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string orderID = jpost["orderID"];
                string CarrierID = jpost["CarrierID"];
                string DriverID = jpost["DriverID"];
                string CarNumber = jpost["CarNumber"];
                string WaybillNo = jpost["WaybillNo"];
                int Type = Convert.ToInt16(jpost["HKDeliveryType"]);
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderConsignees>(new
                    {
                        CarrierID = CarrierID,
                        DriverID = DriverID,
                        CarNumber = CarNumber,
                        WayBillNo = WaybillNo,
                        Type = Type,
                        UpdateDate = DateTime.Now
                    },
                    t => t.OrderID == orderID);
                }

                return Json(new { success = true, message = "保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "保存失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更改提货状态为提货中
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult DeliveryNoticesUpdate(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string orderID = jpost["orderID"];
                int DeliveryNoticeStatus = jpost["deliveryNoticeStatus"];


                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeliveryNotices>(new
                    {
                        DeliverNoticeStatus = DeliveryNoticeStatus,
                        UpdateDate = DateTime.Now
                    },
                    t => t.OrderID == orderID);
                }

                return Json(new { success = true, message = "保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "保存失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 待装箱产品数据
        /// <summary>
        /// 待装箱产品数据
        /// </summary>
        /// <param name="entryNoticeID"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public ActionResult LoadNoticeItems(string entryNoticeID, string keys = null)
        {
            var entryNotice = new EntryNoticesAllNew().Where(t => t.ID == entryNoticeID).FirstOrDefault();
            if (entryNotice == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var sortingView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()
                                              .Where(item => item.Status == (int)Status.Normal && item.WarehouseType == (int)WarehouseType.HongKong && item.OrderID == entryNotice.OrderID);
                var sortingSum = from sorting in sortingView
                                 group sorting by sorting.OrderItemID into g
                                 select new
                                 {
                                     OrderItemID = g.Key,
                                     Quantity = g.Sum(c => c.Quantity),
                                 };

                var EntryNoticeItems = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                                       join orderItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on item.OrderItemID equals orderItem.ID into orderItems
                                       from orderItem in orderItems.DefaultIfEmpty()
                                           //join sortingItem in sortingSum on orderItem.ID equals sortingItem.OrderItemID into sortingItems
                                           //from sortingItem in sortingItems.DefaultIfEmpty()
                                       where item.Status == (int)Status.Normal && item.EntryNoticeID == entryNoticeID
                                       select new HKEntryNoticeItem
                                       {
                                           ID = item.ID,
                                           EntryNoticeID = item.EntryNoticeID,
                                           EntryNoticeStatus = (EntryNoticeStatus)item.EntryNoticeStatus,
                                           Status = (Status)item.Status,
                                           CreateDate = item.CreateDate,
                                           UpdateDate = item.UpdateDate,
                                           OrderItemID = orderItem.ID,
                                           OrderItem = new OrderItem
                                           {
                                               ID = orderItem.ID,
                                               Model = orderItem.Model,
                                               Manufacturer = orderItem.Manufacturer,
                                               Quantity = orderItem.Quantity,
                                               Origin = orderItem.Origin,
                                               Batch = orderItem.Batch,
                                           }
                                       };

                var HKEntryNoticeItems = (from item in EntryNoticeItems
                                          join sorting in sortingSum on item.OrderItemID equals sorting.OrderItemID into sortingItems
                                          from sortingItem in sortingItems.DefaultIfEmpty()
                                          select new HKEntryNoticeItem
                                          {
                                              ID = item.ID,
                                              EntryNoticeID = item.EntryNoticeID,
                                              EntryNoticeStatus = item.EntryNoticeStatus,
                                              Status = item.Status,
                                              CreateDate = item.CreateDate,
                                              UpdateDate = item.UpdateDate,
                                              RelQuantity = item.OrderItem.Quantity - (sortingItem == null ? 0M : sortingItem.Quantity),
                                              OrderItem = item.OrderItem
                                          });

                //更新明细项的状态
                foreach (var item in HKEntryNoticeItems)
                {
                    if (item.RelQuantity == 0M)
                    {
                        if (item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed)
                        {
                            item.EntryNoticeStatus = EntryNoticeStatus.Boxed;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(
                                new { EntryNoticeStatus = EntryNoticeStatus.Boxed }, t => t.ID == item.ID);
                        }
                    }
                    else
                    {
                        if (item.EntryNoticeStatus == EntryNoticeStatus.Boxed)
                        {
                            item.EntryNoticeStatus = EntryNoticeStatus.UnBoxed;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(
                                new { EntryNoticeStatus = EntryNoticeStatus.UnBoxed }, t => t.ID == item.ID);
                        }
                    }
                }
                //更新自己的状态
                int count = HKEntryNoticeItems.Count(item => item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed);
                if (count == 0)
                {
                    if (entryNotice.EntryNoticeStatus == EntryNoticeStatus.UnBoxed)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                                new { EntryNoticeStatus = EntryNoticeStatus.Boxed }, item => item.ID == entryNotice.ID);
                    }
                }
                else
                {
                    if (entryNotice.EntryNoticeStatus == EntryNoticeStatus.Boxed)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(
                                new { EntryNoticeStatus = EntryNoticeStatus.UnBoxed }, item => item.ID == entryNotice.ID);
                    }
                }

                var data = HKEntryNoticeItems.Where(item => item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed);
                var items = data.Select(t => new UnPackedHKEntryNoticeItem
                {
                    ID = t.ID,
                    OrderItemID = t.OrderItem.ID,
                    Model = t.OrderItem.Model,
                    Brand = t.OrderItem.Manufacturer,
                    OrderItemQty = t.OrderItem.Quantity,
                    RelQty = t.RelQuantity,
                    Origin = t.OrderItem.Origin,
                    DateCode = t.OrderItem.Batch
                }).ToArray();

                if (keys != null)
                {
                    items = items.Where(t => t.Model.Contains(keys) || t.Brand.Contains(keys)).ToArray();
                }

                var countries = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>().ToArray();
                var orderItemIDs = items.Select(t => t.OrderItemID).ToList();
                var orderItemCatagories = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(t => orderItemIDs.Contains(t.OrderItemID)).ToArray();

                var results = from t in items
                              join country in countries on t.Origin equals country.Code
                              join orderItemCatagory in orderItemCatagories on t.OrderItemID equals orderItemCatagory.OrderItemID into categories
                              from orderItem in categories.DefaultIfEmpty()
                              select new UnPackedHKEntryNoticeItem
                              {
                                  ID = t.ID,
                                  OrderItemID = t.OrderItemID,
                                  Model = t.Model,
                                  Brand = t.Brand,
                                  OrderItemQty = t.OrderItemQty,
                                  RelQty = t.RelQty,
                                  Origin = t.Origin,
                                  DateCode = t.DateCode,
                                  OriginText = country.Name,
                                  OrderItemType = orderItem == null ? ItemCategoryType.Normal : (ItemCategoryType)orderItem.Type,
                              };

                return Json(new { obj = results }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 已装箱产品数据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        public ActionResult LoadPackedProduct(string orderID)
        {
            if (string.IsNullOrEmpty(orderID))
                return null;

            using (var query = new Needs.Ccs.Services.Views.PackedProductsViewNew(orderID))
            {
                var view = query;
                var result = view.ToMyPage(1, 100);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }


            //var result = list.Select(t => new PackedHKEntryNoticeItem
            //{
            //    ID = t.ID,
            //    PackingID = t.PackingID,
            //    OrderID = orderID,
            //    CaseNo = t.BoxIndex,
            //    Model = t.Model,
            //    Brand = t.Manufacturer,
            //    Origin = t.Origin,
            //    GrossWeight = t.GrossWeight,
            //    RelQty = t.Quantity,
            //    OrderItemQty = t.OrderItemQty
            //});

            //return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 带WayBill 信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadPackedProductWayBill(string orderID)
        {
            if (string.IsNullOrEmpty(orderID))
                return null;

            PackedProductsView view = new PackedProductsView(orderID);
            view.AllowPaging = false;
            var list = view.ToList();

            var result = list.Select(t => new PackedHKEntryNoticeItem
            {
                ID = t.ID,
                PackingID = t.PackingID,
                OrderID = orderID,
                CaseNo = t.BoxIndex,
                Model = t.Model,
                Brand = t.Manufacturer,
                Origin = t.Origin,
                GrossWeight = t.GrossWeight,
                RelQty = t.Quantity,
                OrderItemQty = t.OrderItemQty
            });

            return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 计算总件数
        /// </summary>
        public ActionResult totalPackNo(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                var requestModel = jpost.ToObject<JObject>();
                List<string> innerBoxes = JsonConvert.DeserializeObject<List<string>>(requestModel["boxes"].ToString());
                int innerCount = new CalculateContext(CompanyTypeEnums.Icgoo, innerBoxes).CalculatePacks();

                return Json(new { success = true, data = innerCount, message = "计算成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "计算失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 装箱
        /// <summary>
        /// 装箱操作
        /// </summary>
        public ActionResult PackingBoxIndex(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                var requestModel = jpost.ToObject<PackingInfo>();
                var hkSorting = new Needs.Ccs.Services.Models.HKSortingContext();
                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.AdminID)?.OriginID;
                hkSorting.Admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);
                hkSorting.EntryNoticeID = requestModel.EntryNoticeID;

                PackingModel packing = new PackingModel();

                packing.AdminID = XdtAdminId;
                packing.CenterAdminID = requestModel.AdminID;
                packing.OrderID = requestModel.OrderID;
                packing.BoxIndex = requestModel.BoxIndex;
                packing.Weight = requestModel.Weight;
                packing.WrapType = "22";
                packing.PackingDate = Convert.ToDateTime(requestModel.PackingDate);
                packing.Quantity = requestModel.PackingItems.Sum(t => t.Quantity);
                hkSorting.SetPacking(packing);
                hkSorting.Items = requestModel.PackingItems;

                var waybill = new Needs.Ccs.Services.Models.OrderWaybill();
                if (requestModel.IsExpress)
                {
                    var result = new Needs.Ccs.Services.Views.PvbCrmCarriersTopView().Where(x => x.ID == requestModel.CarrierID);
                    waybill.OrderID = requestModel.OrderID;
                    waybill.Carrier = new Carrier
                    {
                        ID = result.FirstOrDefault().ID
                    };
                    waybill.WaybillCode = requestModel.WaybillNo;
                    waybill.AdminID = XdtAdminId;
                    waybill.Enter();

                    hkSorting.SetWaybill(waybill.ID);
                }

                hkSorting.Pack();

                return Json(new { success = true, message = "装箱成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 验证箱号是否可用
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult BoxIndexValidate(JPost jpost)
        {
            if (jpost["OPTIONS"] == "True")
                return null;

            string BoxIndex = jpost["BoxIndex"];
            DateTime PackingDate = Convert.ToDateTime(jpost["PackingDate"]);
            DateTime PackingEnd = Convert.ToDateTime(jpost["PackingDate"] + " 23:59");
            //判断箱号是否已经用过           
            var packingView = new Needs.Ccs.Services.Views.PackingsView();
            var unExpectedView = new Needs.Ccs.Services.Views.UnExceptedItemsView();
            var packings = packingView.Where(item => item.PackingDate == PackingDate);
            var unExpecteds = unExpectedView.Where(item => item.Status == Status.Normal && item.CreateDate >= PackingDate && item.CreateDate <= PackingEnd && item.IsMapped == false);
            int[] arr1 = this.GetCaseNumbers(BoxIndex);
            int[] arr2 = this.GetCaseNumbers(packings);
            int[] arr3 = this.GetCaseNumbers(unExpecteds);
            var diffArr = arr1.Where(c => arr2.Contains(c)).ToArray();
            var diffArr2 = arr1.Where(c => arr3.Contains(c)).ToArray();
            if (diffArr.Count() > 0)
            {
                string caseNumber = "";
                foreach (var item in diffArr)
                {
                    caseNumber += "HXT" + item.ToString().PadLeft(3, '0') + "、";
                }
                return Json(new { success = false, message = "箱号" + caseNumber + "已使用过，请选择其它箱号。" }, JsonRequestBehavior.AllowGet);
            }
            if (diffArr2.Count() > 0)
            {
                string caseNumber = "";
                foreach (var item in diffArr2)
                {
                    caseNumber += "HXT" + item.ToString().PadLeft(3, '0') + "、";
                }
                return Json(new { success = false, message = "箱号" + caseNumber + "已使用过，请选择其它箱号。" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private int[] GetCaseNumbers(string CaseNumber)
        {
            if (string.IsNullOrEmpty(CaseNumber))
                return null;

            //外单多个订单一个箱子到货，使用特殊箱号 2022-02-21 ryan
            //处理特殊箱号：WL06-1   WL06-01
            List<int> list = new List<int>();
            if (CaseNumber.ToUpper().Contains("-HXT"))
            {
                string[] str = CaseNumber.Split('-');
                int box1 = int.Parse(str[0].Remove(0, 3));
                int box2 = int.Parse(str[1].Remove(0, 3));
                for (int i = box1; i < box2 + 1; i++)
                {
                    list.Add(i);
                }
            }
            else if (CaseNumber.Contains("-"))
            {
                //2022-05-20 这种情况不需要验证了，因为允许这样装箱
                //string caseno = CaseNumber.Split('-')[0];
                //list.Add(int.Parse(caseno.Remove(0, 2)));
            }
            else
            {
                list.Add(int.Parse(CaseNumber.Remove(0, 3)));
            }
            return list.ToArray();
        }

        private int[] GetCaseNumbers(IQueryable<Needs.Ccs.Services.Models.Packing> packings)
        {
            if (packings == null)
                return null;

            List<int> list = new List<int>();
            foreach (var packing in packings)
            {
                if (packing.BoxIndex.ToUpper().Contains("HXT"))
                {
                    int[] array = this.GetCaseNumbers(packing.BoxIndex);
                    list.AddRange(array);
                }
            }
            return list.ToArray();
        }

        private int[] GetCaseNumbers(IQueryable<Needs.Ccs.Services.Models.UnExpectedOrderItem> packings)
        {
            if (packings == null)
                return null;

            List<int> list = new List<int>();
            foreach (var packing in packings)
            {
                int[] array = this.GetCaseNumbers(packing.BoxIndex);
                list.AddRange(array);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 国际快递信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult OrderWaybillInfo(string orderID)
        {
            if (string.IsNullOrEmpty(orderID))
                return null;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var list = (from orderWaybill in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>()
                            join carrier in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>() on orderWaybill.CarrierID equals carrier.ID
                            where orderWaybill.OrderID == orderID && orderWaybill.Status == (int)Status.Normal
                            orderby orderWaybill.CreateDate descending
                            select new
                            {
                                OrderID = orderWaybill.OrderID,
                                WaybillNo = orderWaybill.WaybillCode,
                                CarrierID = carrier.ID,
                                CarrierCode = carrier.Code,
                            }).ToList();

                var orderConsignees = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>()
                                       join carrier in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>() on c.CarrierID equals carrier.ID into carries
                                       from d in carries.DefaultIfEmpty()
                                       where c.OrderID == orderID && c.WayBillNo != null
                                       select new
                                       {
                                           OrderID = c.OrderID,
                                           WaybillNo = c.WayBillNo,
                                           CarrierID = c.CarrierID,
                                           CarrierCode = d.Code,
                                       }).ToList();

                if (orderConsignees.Count() > 0 && !list.Any(t => t.WaybillNo == orderConsignees.FirstOrDefault().WaybillNo))
                {
                    list.Add(orderConsignees.FirstOrDefault());
                }

                var results = list.Select(t => new
                {
                    OrderID = t.OrderID,
                    WaybillNo = t.WaybillNo,
                    CarrierID = t.CarrierID,
                    CarrierCode = t.CarrierCode,
                });
                return Json(new { obj = results }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// 更改箱号
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult ChangeBoxIndex(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string PackingID = jpost["PackingID"];
                string PackingDate = jpost["PackingDate"];
                string NewBoxIndex = jpost["NewBoxIndex"];
                //string OldBoxIndex = jpost["OldBoxIndex"];
                string AdminID = jpost["AdminID"];
                string OrderID = jpost["OrderID"];

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var PackingInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>().Where(t => t.ID == PackingID).FirstOrDefault();

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new
                    {
                        BoxIndex = NewBoxIndex,
                        PackingDate = Convert.ToDateTime(PackingDate),
                        UpdateDate = DateTime.Now
                    },
                    t => t.ID == PackingID);

                    var sortingIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>()
                                      join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on c.SortingID equals d.ID
                                      where c.PackingID == PackingID
                                      select d.ID).ToList();

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new
                    {
                        BoxIndex = NewBoxIndex,
                        UpdateDate = DateTime.Now
                    }, t => sortingIDs.Contains(t.ID));

                    var XDTAdmin = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>().Where(t => t.ID == AdminID).FirstOrDefault();
                    if (XDTAdmin != null)
                    {
                        string logID = ChainsGuid.NewGuidUp();
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = logID,
                            Name = "箱号修改",
                            MainID = OrderID,
                            AdminID = XDTAdmin.OriginID,
                            Summary = "库房，将Packing:" + PackingID + ",装箱日期" + PackingInfo.PackingDate + "改为" + PackingDate + "箱号，由" + PackingInfo.BoxIndex + "改为" + NewBoxIndex,
                            CreateDate = DateTime.Now
                        });
                    }
                }


                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "更改箱号失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更改无通知录入产品箱号
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult ChangeUnExpectedBoxIndex(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string UnExpectedID = jpost["UnExpectedID"];
                string PackingDate = jpost["PackingDate"];
                string NewBoxIndex = jpost["NewBoxIndex"];
                string AdminID = jpost["AdminID"];
                string OrderID = jpost["OrderID"];

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var UnExpectedInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>().Where(t => t.ID == UnExpectedID).FirstOrDefault();

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>(new
                    {
                        BoxIndex = NewBoxIndex,
                        CreateDate = Convert.ToDateTime(PackingDate),
                        UpdateDate = DateTime.Now
                    }, t => t.ID == UnExpectedID);

                    var XDTAdmin = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>().Where(t => t.ID == AdminID).FirstOrDefault();
                    if (XDTAdmin != null)
                    {
                        string logID = ChainsGuid.NewGuidUp();
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = logID,
                            Name = "箱号修改",
                            MainID = OrderID,
                            AdminID = XDTAdmin.OriginID,
                            Summary = "库房，将无通知产品:" + UnExpectedID + ",装箱日期:" + UnExpectedInfo.CreateDate + "改为" + PackingDate + "箱号，由" + UnExpectedInfo.BoxIndex + "改为" + NewBoxIndex,
                            CreateDate = DateTime.Now
                        });
                    }
                }


                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "更改箱号失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 敏感地区不能和其他地区装在一个箱子里
        /// </summary>
        /// <param name="packings"></param>
        /// <returns></returns>
        public ActionResult SensitiveArea(List<PackingSensitiveModel> packings)
        {
            try
            {
                if (packings == null)
                    return null;

                PackingSensitiveCheck check = new PackingSensitiveCheck(packings);
                string sensitiveAreas = "";
                bool alert = check.Check(out sensitiveAreas);

                return Json(new { success = alert, message = sensitiveAreas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "判断失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 未处理的无通知录入产品，不能封箱
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult CanSeal(string orderID)
        {
            try
            {
                if (string.IsNullOrEmpty(orderID))
                    return null;

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var UnExpected = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>().Where(t => t.OrderID == orderID &&
                                                                                                                       t.IsMapped == false &&
                                                                                                                       t.Status == (int)Status.Normal).ToList();
                    if (UnExpected.Count() > 0)
                    {
                        return Json(new { success = false, message = "跟单有未处理的无通知录入产品，不能封箱" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "判断失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 香港操作日志
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult HKOperationLog(JPost jpost)
        {
            if (jpost["OPTIONS"] == "True")
                return null;

            int page = jpost["PageIndex"];
            int rows = jpost["PageSize"];
            string OrderID = jpost["OrderID"];
            string Key = jpost["Key"];

            using (var query = new Needs.Ccs.Services.Views.HKOperationLogView(OrderID))
            {
                var view = query;
                if (!string.IsNullOrEmpty(Key))
                {
                    view = view.SearchByKeys(Key);
                }
                var result = view.ToMyPage(page, rows);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 删除装箱、 封箱、取消封箱
        /// <summary>
        /// 删除装箱结果
        /// </summary>
        /// <param name="jpost"></param>
        public ActionResult DeletePacking(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string PackingID = jpost["PackingID"];
                string EntryNoticeID = jpost["EntryNoticeID"];
                string AdminID = jpost["AdminID"];

                var data = new Needs.Ccs.Services.Views.PackingsView().Where(item => item.ID == PackingID).FirstOrDefault();
                if (data.PackingStatus != PackingStatus.UnSealed)
                {
                    return Json(new { success = false, message = "删除失败：已经封箱或出库" }, JsonRequestBehavior.AllowGet);
                }

                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == AdminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);
                data.Delete(admin, EntryNoticeID);
                return Json(new { success = true, message = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "删除失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 封箱
        /// 要订单变更处理完成之后才能封箱，因为封箱之后就生成报关通知，报关通知是根据Sorting生成的，
        /// 如果有“无通知产品”，没有处理，就没有soriting信息，生成的报关通知就少这一项
        /// </summary>
        /// <param name="jpost"></param>
        public ActionResult Sealed(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string EntryNoticeID = jpost["EntryNoticeID"];
                string AdminID = jpost["AdminID"];
                string OrderID = jpost["OrderID"];

                var entryNotice = new HKEntryNoticeSimpleView()[EntryNoticeID];
                //判断是否已全部封箱
                if (entryNotice != null)
                {
                    if (entryNotice.EntryNoticeStatus == EntryNoticeStatus.Sealed)
                    {
                        return Json(new { success = false, message = "封箱失败：已封箱完成。" }, JsonRequestBehavior.AllowGet);
                    }
                }
                //判断是否已经归类完成
                //无通知产品录入，有可能自动归类没有完成，需要人工重新归类，但是已经装箱了，所以需要判断一下
                var OrderItemOrigin = new Needs.Ccs.Services.Views.OrderItemsView().Where(t => t.OrderID == OrderID).ToList();
                var unClassified = OrderItemOrigin.Where(t => t.ClassifyStatus != ClassifyStatus.Done).ToList();
                if (unClassified.Count() > 0)
                {
                    return Json(new { success = false, message = "封箱失败：存在未归类的产品!" }, JsonRequestBehavior.AllowGet);
                }

                //要订单变更处理完成之后才能封箱，因为封箱之后就生成报关通知，报关通知是根据Sorting生成的，
                //如果有“无通知产品”，没有处理，就没有soriting信息，生成的报关通知就少这一项
                var ModelChange = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView().GetTop(1, item => item.OrderID == OrderID && item.ProcessState == Needs.Ccs.Services.Enums.ProcessState.UnProcess).Count();
                if (ModelChange > 0)
                {
                    return Json(new { success = false, message = "封箱失败：存在未处理的产品变更!" }, JsonRequestBehavior.AllowGet);
                }

                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == AdminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);
                entryNotice.SetAdmin(admin);
                entryNotice.Seal();
                return Json(new { success = true, message = "封箱成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "封箱失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 取消封箱
        /// </summary>
        /// <param name="jpost"></param>
        public ActionResult CancelSealed(JPost jpost)
        {
            try
            {
                if (jpost["OPTIONS"] == "True")
                    return null;

                string PackingID = jpost["PackingID"];
                var packing = new Needs.Ccs.Services.Views.PackingsView().Where(item => item.ID == PackingID).FirstOrDefault();
                packing.CancelSealed();

                return Json(new { success = true, message = "取消封箱成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "取消封箱失败" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 拆分型号
        /// <summary>
        /// 拆分型号
        /// 拆分型号的同步动作（同步客户端，同步归类结果，都在OrderItem里面做了）
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult SplitModelData(JPost jPost)
        {
            try
            {

                if (jPost["OPTIONS"] == "True")
                    return null;

                string orderItemID = jPost["OrderItemID"];
                string origin = jPost["Origin"];
                string manufacturer = jPost["Manufacturer"];
                manufacturer = manufacturer.Replace("amp;", "");
                decimal quantity = Convert.ToDecimal(jPost["Quantity"]);
                string adminID = jPost["adminID"];

                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == adminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);
                var orderItem = new Needs.Ccs.Services.Views.OrderItemsView()[orderItemID];
                orderItem.SplitModel(origin, quantity, manufacturer, admin);

                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 型号、品牌、产地修改
        /// <summary>
        /// 修改型号
        /// </summary>
        /// <param name="jPost"></param>
        public ActionResult ChangeProductModel(JPost jPost)
        {
            try
            {
                if (jPost["OPTIONS"] == "True")
                    return null;

                string orderItemID = jPost["OrderItemID"];
                string adminID = jPost["adminID"];
                string productmodel = jPost["ProductModel"];
                productmodel = productmodel.Replace("amp;", "");
                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == adminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);

                var orderItem = new Needs.Ccs.Services.Views.OrderItemsView()[orderItemID];
                if (productmodel != orderItem.Model)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeProductModel(productmodel);
                    Post2Client(orderItem.OrderID);
                }

                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改产地
        /// </summary>
        /// <param name="jPost"></param>
        [System.Web.Http.HttpPost]
        public ActionResult ChangeOrigin(JPost jPost)
        {
            try
            {
                if (jPost["OPTIONS"] == "True")
                    return null;

                string origin = jPost["OriginValue"];
                string orderItemID = jPost["OrderItemID"];
                string AdminID = jPost["AdminID"];

                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == AdminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);

                var orderItem = new Needs.Ccs.Services.Views.OrderItemsView()[orderItemID];
                if (origin != orderItem.Origin)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeOrigin(origin);
                    Post2Client(orderItem.OrderID);
                }

                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="jPost"></param>
        public ActionResult ChangeManufacturer(JPost jPost)
        {
            try
            {
                if (jPost["OPTIONS"] == "True")
                    return null;

                string orderItemID = jPost["OrderItemID"];
                string AdminID = jPost["AdminID"];
                string manufacturer = jPost["Manufacturer"];
                manufacturer = manufacturer.Replace("amp;", "");
                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == AdminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);

                var orderItem = new Needs.Ccs.Services.Views.OrderItemsView()[orderItemID];
                if (manufacturer != orderItem.Manufacturer)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeManufacturer(manufacturer);
                    Post2Client(orderItem.OrderID);
                }

                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改批次号
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeBatch(JPost jPost)
        {
            try
            {
                if (jPost["OPTIONS"] == "True")
                    return null;

                string orderItemID = jPost["OrderItemID"];
                string AdminID = jPost["AdminID"];
                string batch = jPost["Batch"];
                batch = batch.Replace("amp;", "");
                var XdtAdminId = new AdminsTopView2().FirstOrDefault(t => t.ID == AdminID)?.OriginID;
                var admin = Needs.Underly.FkoFactory<Admin>.Create(XdtAdminId);

                var orderItem = new Needs.Ccs.Services.Views.OrderItemsView()[orderItemID];
                if (batch != orderItem.Manufacturer)
                {
                    orderItem.SorterAdmin = admin;
                    orderItem.ChangeBatch(batch);
                    Post2Client(orderItem.OrderID);
                }

                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 将型号、品牌、产地变更信息通给客户端
        /// </summary>
        /// <param name="orderID"></param>
        private void Post2Client(string orderID)
        {
            if (string.IsNullOrEmpty(orderID))
                return;

            string tinyOrderID = orderID;
            var OriginOrder = new Needs.Ccs.Services.Views.OrdersView().Where(t => t.ID == tinyOrderID).FirstOrDefault();
            MatchPost2AgentWarehouse post = new MatchPost2AgentWarehouse(OriginOrder.MainOrderID, OriginOrder);
            post.ItemChangePost();
        }
        #endregion

        #region 无通知产品录入
        /// <summary>
        /// 无通知录入用一张表记录
        /// 跟单拆分订单的页面显示这条记录，跟单录入品名和价格
        /// </summary>
        public ActionResult UnExpectedGoods(List<UnExpectedOrderItem> requestModel)
        {
            try
            {
                if (requestModel == null)
                    return null;

                foreach (var item in requestModel)
                {
                    item.ID = ChainsGuid.NewGuidUp();
                    string[] boxIndexes = item.BoxIndex.Split(']');
                    if (boxIndexes.Length > 1)
                    {
                        item.BoxIndex = boxIndexes[1];
                    }
                    else
                    {
                        item.BoxIndex = item.BoxIndex;
                    }
                    item.Enter();
                }
                return Json(new { success = true, message = "添加成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 无通知产品录入视图
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult UnExceptedList(string orderID)
        {
            if (string.IsNullOrEmpty(orderID))
                return null;

            using (var query = new Needs.Ccs.Services.Views.UnExceptedItemsView())
            {
                var view = query;
                view = view.SearchByOrderID(orderID);
                view = view.SearchByIsMapped(false);
                var result = view.ToMyPage(1, 100);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除无通知产品录入
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult DeleteUnExceptedList(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return null;

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>(new
                    {
                        Status = (int)Status.Delete,
                        UpdateDate = DateTime.Now
                    },
                    t => t.ID == ID);
                }

                return Json(new { success = true, message = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "删除失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 重量分配、重量预警
        /// <summary>
        /// 重量分配 不需要实现，装箱的时候，就已经分配了
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult AllocateWeight(JPost jPost)
        {
            try
            {
                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 重量预测
        /// 取每个型号的 单个标准重量，乘以每个型号的数量，加总
        /// 用加总之后的重量比较
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ActionResult WeightEstimate(List<WeightAlterItem> requestModel)
        {
            try
            {
                decimal totalEstimateWeight = EstimateWeight(requestModel);
                return Json(new { success = true, data = totalEstimateWeight }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "重量预测失败:" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///  传入预测重量，传入装箱重量，比较两者的值
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult WeightAlert(JPost jPost)
        {
            if (jPost["OPTIONS"] == "True")
                return null;

            decimal percentage = 0.5m;
            decimal estimateWeight = Convert.ToDecimal(jPost["EstimateWeight"]);
            decimal totalWeight = Convert.ToDecimal(jPost["TotalWeight"]);

            decimal minmunWeight = estimateWeight * (1 - percentage);
            decimal maximumWeight = estimateWeight * (1 + percentage);

            if (totalWeight < minmunWeight || totalWeight > maximumWeight)
            {
                return Json(new { success = true, message = "重量符合" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "重量浮动超过50%" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<ModelAvgWeight> StandardWeight(List<string> models)
        {
            var res = new ModelAvgWeightOrigin().Where(t => models.Contains(t.Model)).ToList();
            return res;
        }

        private decimal EstimateWeight(List<WeightAlterItem> items)
        {
            if (items == null)
                return 0M;
            decimal totalEstimateWeight = 0m;
            List<string> models = new List<string>();
            foreach (var item in items)
            {
                models.Add(item.Model);
            }
            var result = StandardWeight(models);

            var totalQty = items.Sum(t => t.Qty);
            foreach (var item in items)
            {
                var res = result.Where(t => t.Model == item.Model).FirstOrDefault();
                if (res != null)
                {
                    decimal? weight = res.Weight;
                    if (weight.HasValue)
                    {
                        totalEstimateWeight += item.Qty * weight.Value;
                    }
                }

            }
            return totalEstimateWeight;
        }
        #endregion

        #region 计算仓储天数
        /// <summary>
        /// 计算仓储天数
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult StockingDays(string orderID)
        {
            try
            {
                if (string.IsNullOrEmpty(orderID))
                    return null;

                StockingContext stockingContext = new StockingContext(orderID);
                int days = stockingContext.calculate();
                return Json(new { success = true, message = "计算成功", data = days }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "计算失败" + ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 运输批次信息
        /// <summary>
        /// 获取PvbCrm 承运商信息
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult CarrierInteLogisticsList()
        {
            var result = new CarriersView().Where(item => item.CarrierType == CarrierType.InteLogistics
                            && item.Status == Status.Normal).ToList();
            return Json(new { success = true, message = "计算成功", data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 报关运输列表数据
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult ManifestVoyageList(JPost jPost)
        {
            if (jPost["OPTIONS"] == "True")
                return null;

            string voyageNo = jPost["LotNumber"];
            string carrier = jPost["Carrier"];
            string cutStatus = jPost["Status"];
            string StartDate = jPost["StartDate"];
            string EndDate = jPost["EndDate"];
            int page = jPost["PageIndex"];
            int rows = jPost["PageSize"];

            using (var query = new Needs.Ccs.Services.Views.ManifestVoyageListViewNew())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(voyageNo))
                {
                    voyageNo = voyageNo.Trim();
                    view = view.SearchByVoyageNo(voyageNo);
                }

                if (!string.IsNullOrWhiteSpace(carrier))
                {
                    carrier = carrier.Trim();
                    view = view.SearchByCarrier(carrier);
                }


                if (!string.IsNullOrWhiteSpace(cutStatus))
                {
                    int intcutStatus = 1;
                    if (int.TryParse(cutStatus, out intcutStatus))
                    {
                        view = view.SearchByCutStatus((CutStatus)intcutStatus);
                    }
                }

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    StartDate = StartDate.Trim();
                    DateTime dtStart = Convert.ToDateTime(StartDate);
                    view = view.SearchByStartDate(dtStart);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    EndDate = EndDate.Trim();
                    DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByStartDate(dtEnd);
                }

                var result = view.ToMyPage(page, rows);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 报关运输基本信息数据
        /// </summary>
        /// <returns></returns>
        public ActionResult VoyageInfo(string voyageNo)
        {
            if (string.IsNullOrEmpty(voyageNo))
                return null;

            var VoyageInfo = new VoyagesView().Where(t => t.ID == voyageNo).FirstOrDefault();

            VoyageInfoViewModel voyage = new VoyageInfoViewModel();
            voyage.VoyageNo = voyageNo.ToString();
            voyage.CutStatus = VoyageInfo.CutStatus;
            voyage.CarrierName = VoyageInfo.CarrierName;
            voyage.VoyageType = VoyageInfo.Type;
            voyage.DriverName = VoyageInfo.DriverName;
            voyage.DriverPhone = VoyageInfo.DriverMobile;
            voyage.TransportTime = VoyageInfo.TransportTime;
            voyage.SealNo = VoyageInfo.HKSealNumber;

            var voyageDecHeads = new VoyageDecHeadsView().Where(vd => vd.VoyageNo == voyage.VoyageNo);
            var voyageDecHeadArr = voyageDecHeads.ToArray();
            if (voyageDecHeadArr.Count() > 0)
            {
                VoyagePackNo voyagePackNo = new VoyagePackNo(voyageDecHeadArr.Select(t => t.ID).ToList());
                int totalPack = voyagePackNo.Calculate();

                voyage.TotalPackNo = totalPack;
                voyage.TotalQuantity = voyageDecHeadArr.Sum(dh => dh.GQty);
                voyage.TotalGrossWt = voyageDecHeadArr.Sum(dh => dh.GrossWt);
                voyage.TotalAmount = voyageDecHeadArr.Sum(dh => dh.DeclTotal);
                voyage.TotalItems = voyageDecHeadArr.Sum(dh => dh.ItemsCount);
            }

            voyage.VoyageFiles = new List<PickupFile>();

            var Files = new CenterFilesTopView().Where(t => t.ShipID == VoyageInfo.ID && t.Type == (int)FileType.None && t.Status == FileDescriptionStatus.Normal).ToArray();
            foreach (var item in Files)
            {
                PickupFile file = new PickupFile();
                file.ID = item.ID;
                file.FileName = item.CustomName;
                file.FileUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"] + "/" + item?.Url.ToUrl();
                voyage.VoyageFiles.Add(file);
            }

            return Json(new { success = true, data = voyage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult VoyageDetail(JPost jPost)
        {
            if (jPost["OPTIONS"] == "True")
                return null;

            string voyageNo = jPost["voyageNo"];
            string clientType = jPost["clientType"];
            int page = jPost["PageIndex"];
            int rows = jPost["PageSize"];

            using (var query = new Needs.Ccs.Services.Views.VoyageDetailsViewNew())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(voyageNo))
                {
                    voyageNo = voyageNo.Trim();
                    view = view.SearchByVoyageNo(voyageNo);
                }


                if (!string.IsNullOrWhiteSpace(clientType))
                {
                    int intclientType = 1;
                    if (int.TryParse(clientType, out intclientType))
                    {
                        view = view.SearchByClientType((ClientType)intclientType);
                    }
                }


                var result = view.ToMyPage(page, rows);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改封条号
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult UpdateHKSealNo(JPost jPost)
        {
            if (jPost["OPTIONS"] == "True")
                return null;

            string voyageNo = jPost["voyageNo"];
            string sealNo = jPost["sealNo"];
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { HKSealNumber = sealNo }, t => t.ID == voyageNo);
                }
                return Json(new { success = true, message = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 运输批次 完成
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult UpdateVoyaeStatus(JPost jPost)
        {

            try
            {
                if (jPost["OPTIONS"] == "True")
                    return null;

                string voyageNo = jPost["voyageNo"];
                int packNo = Convert.ToInt16(jPost["packNo"]);
                int voyagePackNo = 0;

                var voyageDecHeads = new VoyageDecHeadsView().Where(vd => vd.VoyageNo == voyageNo);
                var voyageDecHeadArr = voyageDecHeads.ToArray();
                if (voyageDecHeadArr.Count() > 0)
                {
                    voyagePackNo = voyageDecHeadArr.Sum(dh => dh.PackNo);
                }

                if (packNo != voyagePackNo)
                {
                    return Json(new { success = false, data = "报关单件数不等于装箱件数" }, JsonRequestBehavior.AllowGet);
                }

                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { CutStatus = (int)CutStatus.Completed }, t => t.ID == voyageNo);
                }
                return Json(new { success = true, message = "确认成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 枚举以及基础信息
        private class NewSource<T>
        {
            public T value { get; set; }
            public string name { get; set; }

            public NewSource(T value, string name)
            {
                this.value = value;
                this.name = name;
            }
        }

        /// <summary>
        /// 获取 入库通知状态
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEntryNoticeStatus()
        {

            List<NewSource<EntryNoticeStatus>> newSortingExcuteStatusList = new List<NewSource<EntryNoticeStatus>>();
            foreach (EntryNoticeStatus item in Enum.GetValues(typeof(EntryNoticeStatus)))
            {
                if (item == EntryNoticeStatus.Sealed)
                {
                    continue;
                }
                newSortingExcuteStatusList.Add(new NewSource<EntryNoticeStatus>(item, item.GetDescription()));
            }
            return Json(new { obj = newSortingExcuteStatusList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取 香港提货方式
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHKDeliveryType()
        {

            List<NewSource<HKDeliveryType>> newSortingExcuteStatusList = new List<NewSource<HKDeliveryType>>();
            foreach (HKDeliveryType item in Enum.GetValues(typeof(HKDeliveryType)))
            {
                newSortingExcuteStatusList.Add(new NewSource<HKDeliveryType>(item, item.GetDescription()));
            }
            return Json(new { obj = newSortingExcuteStatusList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 产地信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrigin()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var result = (from ori in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>()
                              select new Needs.Ccs.Services.Models.Country
                              {
                                  Code = ori.Code,
                                  Name = ori.Name
                              }).OrderBy(t => t.Code).ToList();

                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 报关运输状态
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCutStatus()
        {

            List<NewSource<CutStatus>> newSortingExcuteStatusList = new List<NewSource<CutStatus>>();
            foreach (CutStatus item in Enum.GetValues(typeof(CutStatus)))
            {
                newSortingExcuteStatusList.Add(new NewSource<CutStatus>(item, item.GetDescription()));
            }
            return Json(new { obj = newSortingExcuteStatusList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 快递信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetExpressInfo()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var result = new Needs.Ccs.Services.Views.PvbCrmCarriersTopView().
                                 Where(x => x.IsInternational == true && x.Type == Needs.Ccs.Services.Models.PvbCarrierType.Express).ToList();
                var results = result.Select(ori => new
                {
                    ID = ori.ID,
                    Code = ori.Code,
                    Name = ori.Name
                }).OrderBy(ori => ori.Code);

                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 文件相关
        /// <summary>
        /// 生成提货委托书、货物流转确认单
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintExportFiles(JPost jPost)
        {
            try
            {
                if (jPost["OPTIONS"] == "True")
                    return null;

                string voyageNo = jPost["voyageNo"];
                string fileType = jPost["fileType"];
                int totalPackNo = jPost["totalPackNo"];
                decimal totalWeight = jPost["totalWeight"];

                string fileDomain = System.Configuration.ConfigurationManager.AppSettings["APIFileServerUrl"];
                string fileName = "";
                if (fileType == "thwt")
                {
                    PrintFile thwt = new GoodsPickUpPrint(voyageNo, totalPackNo, totalWeight);
                    fileName = thwt.GenerateFile();
                }
                else
                {
                    PrintFile thwt = new GoodsDeliveryPrint(voyageNo, totalPackNo, totalWeight);
                    fileName = thwt.GenerateFile();
                }
                string fileUrl = System.IO.Path.Combine(fileDomain, fileName);
                return Json(new { success = true, message = "文件生成成功", data = fileUrl }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "文件生成失败", data = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion
    }
}
