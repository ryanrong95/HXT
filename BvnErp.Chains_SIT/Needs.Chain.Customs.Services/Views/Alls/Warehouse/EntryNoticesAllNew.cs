using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class EntryNoticesAllNew : QueryView<HKEntryNoticeViewModel, ScCustomsReponsitory>
    {
        public EntryNoticesAllNew()
        {
        }

        internal EntryNoticesAllNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected EntryNoticesAllNew(ScCustomsReponsitory reponsitory, IQueryable<HKEntryNoticeViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<HKEntryNoticeViewModel> GetIQueryable()
        {
            var noticesView = new Origins.EntryNoticesOrigin(this.Reponsitory);
            var iQuery = from notice in noticesView
                         join orderConsignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>() on notice.OrderID equals orderConsignee.OrderID
                         join clientSupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on orderConsignee.ClientSupplierID equals clientSupplier.ID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on notice.OrderID equals order.ID
                         where notice.Status == Enums.Status.Normal && notice.WarehouseType == Enums.WarehouseType.HongKong
                         select new HKEntryNoticeViewModel
                         {
                             ID = notice.ID,
                             OrderID = notice.OrderID,
                             DecHeadID = notice.DecHeadID,
                             ClientCode = notice.ClientCode,
                             SortingRequire = notice.SortingRequire,
                             WarehouseType = notice.WarehouseType,
                             EntryNoticeStatus = notice.EntryNoticeStatus,
                             Status = notice.Status,
                             CreateDate = notice.CreateDate,
                             UpdateDate = notice.UpdateDate,
                             Summary = notice.Summary,
                             HKDeliveryType = (HKDeliveryType)orderConsignee.Type,
                             ClientSupplierName = clientSupplier.ChineseName,
                             EnterCode = order.EnterCode,
                             WayBillNo = orderConsignee.WayBillNo
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.HKEntryNoticeViewModel> iquery = this.IQueryable.Cast<Models.HKEntryNoticeViewModel>().OrderByDescending(item => item.UpdateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

            var results = ienum_EntryNotices;

            var res = results.Select(
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.OrderID,
                            EntryNumber = item.ClientCode,
                            SupplierName = item.ClientSupplierName,
                            Type = item.HKDeliveryType.GetDescription(),
                            Status = item.EntryNoticeStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm"),
                            EnterCode = item.EnterCode,
                            WayBillNo = item.WayBillNo
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public EntryNoticesAllNew SearchByStatus(EntryNoticeStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus == status
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchUnSealed()
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus != EntryNoticeStatus.Sealed
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchSealed()
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus == EntryNoticeStatus.Sealed
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }
        public EntryNoticesAllNew SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode) || query.EnterCode.Contains(clientCode)
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchBySupplierName(string supplierName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientSupplierName.Contains(supplierName)
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchByHKDeliveryType(HKDeliveryType hKDeliveryType)
        {
            var linq = from query in this.IQueryable
                       where query.HKDeliveryType == hKDeliveryType
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchByStartDate(DateTime dtStart)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= dtStart
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesAllNew SearchByEndDate(DateTime dtEnd)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < dtEnd
                       select query;

            var view = new EntryNoticesAllNew(this.Reponsitory, linq);
            return view;
        }
    }

    public class EntryNoticesWithItemsAllNew : QueryView<HKEntryNoticeViewModel, ScCustomsReponsitory>
    {
        public EntryNoticesWithItemsAllNew()
        {
        }

        internal EntryNoticesWithItemsAllNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected EntryNoticesWithItemsAllNew(ScCustomsReponsitory reponsitory, IQueryable<HKEntryNoticeViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<HKEntryNoticeViewModel> GetIQueryable()
        {
            var orderItemsView = new Origins.OrderItemsOrigin(this.Reponsitory);
            var noticeItemsView = new Origins.EntryNoticeItemsOrigin(this.Reponsitory);
            var hkNoticeItemsView = from item in noticeItemsView
                                    join orderItem in orderItemsView on item.OrderItemID equals orderItem.ID
                                    where item.Status == Enums.Status.Normal
                                    select new HKEntryNoticeItem
                                    {
                                        ID = item.ID,
                                        EntryNoticeID = item.EntryNoticeID,
                                        OrderItemID = item.OrderItemID,
                                        OrderItem = orderItem,
                                        EntryNoticeStatus = item.EntryNoticeStatus,
                                    };


            var noticesView = new Origins.EntryNoticesOrigin(this.Reponsitory);
            var iQuery = from notice in noticesView
                         join item in hkNoticeItemsView on notice.ID equals item.EntryNoticeID into items
                         where notice.Status == Enums.Status.Normal && notice.WarehouseType == Enums.WarehouseType.HongKong
                         orderby notice.CreateDate descending
                         select new HKEntryNoticeViewModel
                         {
                             ID = notice.ID,
                             OrderID = notice.OrderID,
                             DecHeadID = notice.DecHeadID,
                             ClientCode = notice.ClientCode,
                             SortingRequire = notice.SortingRequire,
                             WarehouseType = notice.WarehouseType,
                             EntryNoticeStatus = notice.EntryNoticeStatus,
                             Status = notice.Status,
                             CreateDate = notice.CreateDate,
                             UpdateDate = notice.UpdateDate,
                             Summary = notice.Summary,

                             HKItems = items
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.HKEntryNoticeViewModel> iquery = this.IQueryable.Cast<Models.HKEntryNoticeViewModel>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

            var orderIDs = ienum_EntryNotices.Select(item => item.OrderID);

            var linq_orderConsigneeInfo = from orderConsignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>()
                                          join clientSupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on orderConsignee.ClientSupplierID equals clientSupplier.ID
                                          where orderIDs.Contains(orderConsignee.OrderID)
                                          select new
                                          {
                                              OrderID = orderConsignee.OrderID,
                                              HKDeliveryType = (HKDeliveryType)orderConsignee.Type,
                                              ClientSupplierName = clientSupplier.ChineseName
                                          };
            var ienums_orderConsigneeInfo = linq_orderConsigneeInfo.ToArray();

            var linq_orderEnterCode = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                      where orderIDs.Contains(order.ID)
                                      select new
                                      {
                                          OrderID = order.ID,
                                          EnterCode = order.EnterCode,
                                      };

            var ienums_orderEnterCode = linq_orderEnterCode.ToArray();


            var results = from notice in ienum_EntryNotices
                          join orderCon in ienums_orderConsigneeInfo on notice.OrderID equals orderCon.OrderID
                          join order in ienums_orderEnterCode on notice.OrderID equals order.OrderID
                          select new HKEntryNoticeViewModel
                          {
                              ID = notice.ID,
                              OrderID = notice.OrderID,
                              DecHeadID = notice.DecHeadID,
                              ClientCode = notice.ClientCode,
                              SortingRequire = notice.SortingRequire,
                              WarehouseType = notice.WarehouseType,
                              EntryNoticeStatus = notice.EntryNoticeStatus,
                              Status = notice.Status,
                              CreateDate = notice.CreateDate,
                              UpdateDate = notice.UpdateDate,
                              Summary = notice.Summary,
                              HKDeliveryType = orderCon.HKDeliveryType,
                              ClientSupplierName = orderCon.ClientSupplierName,
                              EnterCode = order.EnterCode,
                          };

            var res = results.Select(
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.OrderID,
                            EntryNumber = item.ClientCode,
                            SupplierName = item.ClientSupplierName,
                            Type = item.HKDeliveryType.GetDescription(),
                            Status = item.EntryNoticeStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            EnterCode = item.EnterCode
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public EntryNoticesWithItemsAllNew SearchUnSealed()
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus != EntryNoticeStatus.Sealed
                       select query;

            var view = new EntryNoticesWithItemsAllNew(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticesWithItemsAllNew SearchSealed()
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus == EntryNoticeStatus.Sealed
                       select query;

            var view = new EntryNoticesWithItemsAllNew(this.Reponsitory, linq);
            return view;
        }
        public EntryNoticesWithItemsAllNew SearchByModel(string model)
        {
            var linq = from query in this.IQueryable
                       where query.HKItems.Select(i => i.OrderItem.Model).Contains(model)
                       select query;

            var view = new EntryNoticesWithItemsAllNew(this.Reponsitory, linq);
            return view;
        }
    }

    public class EntryNoticeWaybill : QueryView<HKEntryNoticeViewModel, ScCustomsReponsitory>
    {
        public EntryNoticeWaybill()
        {
        }

        internal EntryNoticeWaybill(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected EntryNoticeWaybill(ScCustomsReponsitory reponsitory, IQueryable<HKEntryNoticeViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<HKEntryNoticeViewModel> GetIQueryable()
        {
            
            var iQuery = from orderWaybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>()  
                         join notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>() on orderWaybill.OrderID equals notice.OrderID
                         where orderWaybill.Status == (int)Enums.Status.Normal && notice.WarehouseType == (int)Enums.WarehouseType.HongKong && notice.Status == (int)Enums.Status.Normal
                         select new HKEntryNoticeViewModel
                         {                            
                             OrderID = orderWaybill.OrderID,                           
                             WayBillNo = orderWaybill.WaybillCode,
                             CreateDate = orderWaybill.CreateDate,
                             EntryNoticeStatus = (EntryNoticeStatus)notice.EntryNoticeStatus
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.HKEntryNoticeViewModel> iquery = this.IQueryable.Cast<Models.HKEntryNoticeViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_Waybills = iquery.ToArray();
            var orderIDs = ienum_Waybills.Select(item => item.OrderID);

            var noticesView = new Origins.EntryNoticesOrigin(this.Reponsitory);
            var linq_EntryNotices = from notice in noticesView
                                    join orderConsignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>() on notice.OrderID equals orderConsignee.OrderID
                                    join clientSupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on orderConsignee.ClientSupplierID equals clientSupplier.ID
                                    join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on notice.OrderID equals order.ID
                                    where notice.Status == Enums.Status.Normal && 
                                        notice.WarehouseType == Enums.WarehouseType.HongKong &&
                                        orderIDs.Contains(notice.OrderID)
                                    select new HKEntryNoticeViewModel
                                    {
                                        ID = notice.ID,
                                        OrderID = notice.OrderID,
                                        DecHeadID = notice.DecHeadID,
                                        ClientCode = notice.ClientCode,
                                        SortingRequire = notice.SortingRequire,
                                        WarehouseType = notice.WarehouseType,
                                        EntryNoticeStatus = notice.EntryNoticeStatus,
                                        Status = notice.Status,
                                        CreateDate = notice.CreateDate,
                                        UpdateDate = notice.UpdateDate,
                                        Summary = notice.Summary,
                                        HKDeliveryType = (HKDeliveryType)orderConsignee.Type,
                                        ClientSupplierName = clientSupplier.ChineseName,
                                        EnterCode = order.EnterCode,                                       
                                    };

            var ienums_EntryNotices = linq_EntryNotices.ToArray();


            var results = from orderWaybill  in ienum_Waybills 
                          join notice in ienums_EntryNotices on orderWaybill.OrderID equals notice.OrderID
                          select new HKEntryNoticeViewModel
                          {
                              ID = notice.ID,
                              OrderID = notice.OrderID,
                              DecHeadID = notice.DecHeadID,
                              ClientCode = notice.ClientCode,
                              SortingRequire = notice.SortingRequire,
                              WarehouseType = notice.WarehouseType,
                              EntryNoticeStatus = notice.EntryNoticeStatus,
                              Status = notice.Status,
                              CreateDate = notice.CreateDate,
                              UpdateDate = notice.UpdateDate,
                              Summary = notice.Summary,
                              HKDeliveryType = notice.HKDeliveryType,
                              ClientSupplierName = notice.ClientSupplierName,
                              EnterCode = notice.EnterCode,
                              WayBillNo = orderWaybill.WayBillNo
                          };

            var res = results.Select(
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.OrderID,
                            EntryNumber = item.ClientCode,
                            SupplierName = item.ClientSupplierName,
                            Type = item.HKDeliveryType.GetDescription(),
                            Status = item.EntryNoticeStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                            UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm"),
                            EnterCode = item.EnterCode,
                            WayBillNo = item.WayBillNo
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public EntryNoticeWaybill SearchUnSealed()
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus != EntryNoticeStatus.Sealed
                       select query;

            var view = new EntryNoticeWaybill(this.Reponsitory, linq);
            return view;
        }

        public EntryNoticeWaybill SearchSealed()
        {
            var linq = from query in this.IQueryable
                       where query.EntryNoticeStatus == EntryNoticeStatus.Sealed
                       select query;

            var view = new EntryNoticeWaybill(this.Reponsitory, linq);
            return view;
        }
        public EntryNoticeWaybill SearchByWaybillNo(string waybillNo)
        {
            var linq = from query in this.IQueryable
                       where query.WayBillNo == waybillNo
                       select query;

            var view = new EntryNoticeWaybill(this.Reponsitory, linq);
            return view;
        }
    }
}
