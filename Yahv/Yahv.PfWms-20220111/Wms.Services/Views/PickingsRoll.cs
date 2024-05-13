using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Services.Enums;

using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters;
using Yahv.Utils.Serializers;

namespace Wms.Services.Views
{
    public class PickingWaybillRoll : QueryView<Models.DataPickingWaybill, PvWmsRepository>
    {
        public PickingWaybillRoll()
        {

        }

        public PickingWaybillRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public PickingWaybillRoll(PvWmsRepository reponsitory, IQueryable<DataPickingWaybill> iQueryable) : base(reponsitory, iQueryable)
        {
        }


        protected override IQueryable<Models.DataPickingWaybill> GetIQueryable()
        {

            var clientView = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory);

            //送货上门的提前

            var linqsend = from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                           join entity in clientView on waybill.EnterCode equals entity.EnterCode into client
                           where waybill.Type == WaybillType.DeliveryToWarehouse
                           orderby waybill.CreateDate descending
                           select new Models.DataPickingWaybill
                           {
                               WaybillID = waybill.ID,
                               WaybillType = waybill.Type,
                               EnterCode = waybill.EnterCode,
                               ClientName = client == null ? null : client.FirstOrDefault().Name,
                               CreateDate = waybill.CreateDate,
                               Supplier = waybill.Supplier,
                               Condition = waybill.Condition,
                               ConsignorID = waybill.ConsignorID,
                               ExcuteStatus = waybill.ExcuteStatus,
                               Code = waybill.Code,
                               CarrierID = waybill.CarrierID,
                               CarrierName = waybill.CarrierName,
                               Place = waybill.Consignee.Place,
                               Consignee = waybill.Consignee,
                               Consignor = waybill.Consignor,
                               WayLoading = waybill.WayLoading,
                               Status = waybill.Status,
                               Packaging = waybill.Packaging,
                               TransferID = waybill.TransferID,
                               CuttingOrderStatus = waybill.CuttingOrderStatus == null ? CgCuttingOrderStatus.Waiting : (CgCuttingOrderStatus)waybill.CuttingOrderStatus


                           };


            var linq = from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                       join entity in clientView on waybill.EnterCode equals entity.EnterCode into client
                       where waybill.Type != WaybillType.DeliveryToWarehouse
                       orderby waybill.CreateDate descending
                       select new Models.DataPickingWaybill
                       {
                           WaybillID = waybill.ID,
                           WaybillType = waybill.Type,
                           EnterCode = waybill.EnterCode,
                           ClientName = client == null ? null : client.FirstOrDefault().Name,
                           CreateDate = waybill.CreateDate,
                           Supplier = waybill.Supplier,
                           Condition = waybill.Condition,
                           ConsignorID = waybill.ConsignorID,
                           ExcuteStatus = waybill.ExcuteStatus,
                           Code = waybill.Code,
                           CarrierID = waybill.CarrierID,
                           CarrierName = waybill.CarrierName,
                           Place = waybill.Consignee.Place,
                           Consignee = waybill.Consignee,
                           Consignor = waybill.Consignor,
                           WayLoading = waybill.WayLoading,
                           Status = waybill.Status,
                           Packaging = waybill.Packaging,
                           TransferID = waybill.TransferID,
                           CuttingOrderStatus = waybill.CuttingOrderStatus == null ? CgCuttingOrderStatus.Waiting : (CgCuttingOrderStatus)waybill.CuttingOrderStatus


                       };



            var dpws = new List<DataPickingWaybill>();
            dpws.AddRange(linqsend.Select(item => item));
            dpws.AddRange(linq.Select(item => item));


            return dpws.Select(item => item).AsQueryable();

        }

        public PickingWaybillRoll SearchByPartNumber(string name)
        {
            //string name = "AD620";
            var _productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            var waybillIDView = from product in _productView
                                join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
                                where product.PartNumber.Contains(name) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>内部页面分拣使用</remarks>
        public PickingWaybill SearchByPartNumberOrManufacturer(DataPickingWaybill roll, string name)
        {
            var notices = roll.Notices.Where(item => (item.Product.PartNumber.StartsWith(name) || item.Product.Manufacturer.StartsWith(name)) && item.Status != NoticesStatus.Closed);
            roll.Notices = notices.ToArray();
            return roll;
        }

        public PickingWaybillRoll SearchByManufacturer(string name)
        {
            //string name = "AD620";
            var _productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            var waybillIDView = from product in _productView
                                join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
                                where product.Manufacturer.Contains(name) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        public PickingWaybillRoll SearchBySupplier(string name)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.Supplier.Contains(name)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        public PickingWaybillRoll SearchByStartDate(DateTime startdate)
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_Waybills>()
                       where entity.CreateDate >= startdate
                       select new { entity.MainID };

            var waybillids = linq.Distinct().Select(item => item.MainID);


            var iQuery = from waybill in this.IQueryable
                         where waybillids.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        public PickingWaybillRoll SearchByEndDate(DateTime enddate)
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_Waybills>()
                       where entity.CreateDate < enddate.AddDays(1)
                       select new { entity.MainID };

            var waybillids = linq.Distinct().Select(item => item.MainID);


            var iQuery = from waybill in this.IQueryable
                         where waybillids.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        public PickingWaybillRoll SearchByID(string id)
        {

            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.wbID.Contains(id) || waybill.wbCode.Contains(id) || waybill.wbEnterCode.Contains(id)
                             select waybill.wbID;


            var waybillIDView = from output in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on output.ID equals notice.OutputID
                                where output.OrderID.Contains(id) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids?.Distinct());
            wids.AddRange(waybillIDView?.Distinct());

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        public PickingWaybillRoll SearchByStatus(params PickingExcuteStatus[] excuteStatus)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where excuteStatus.Contains((PickingExcuteStatus)waybill.wbExcuteStatus)
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }



        public PickingWaybillRoll SearcBySource(NoticeSource source)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where notice.Source == (int)source && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillIDView);

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }

        public PickingWaybillRoll SearchByWareHouseID(string warehouseID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where notice.WareHouseID.Contains(warehouseID) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            List<string> topN = new List<string>();

            topN.AddRange(waybillIDView.Distinct().OrderBy(item => item).Take(500));

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            var roll = new PickingWaybillRoll(this.Reponsitory, iQuery);

            return roll;
        }

        public PickingWaybillRoll SearchByNoticeType(params NoticeType[] types)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where types.Contains((NoticeType)notice.Type) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            List<string> topN = new List<string>();

            topN.AddRange(waybillIDView.Distinct());

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            var roll = new PickingWaybillRoll(this.Reponsitory, iQuery);

            return roll;
        }


        public PickingWaybillRoll SearchByDataStatus(GeneralStatus status)
        {

            var iQuery = from waybill in this.IQueryable
                         where waybill.Status == status
                         select waybill;

            var roll = new PickingWaybillRoll(this.Reponsitory, iQuery);

            return roll;
        }

        public PickingWaybillRoll SearchByWaybillD(string id)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.WaybillID == id
                         select waybill;

            return new PickingWaybillRoll(this.Reponsitory, iQuery);
        }


    }


    //static public class PickingsWayBillExtends
    //{


    //    static public DataPickingWaybill[] ToFillArray(this IQueryable<DataPickingWaybill> iquery, PvWmsRepository rep = null)
    //    {

    //        var reponsitory = rep ?? new PvWmsRepository();
    //        try
    //        {
    //            var arry = iquery.ToArray();
    //            var wblsID = arry.Select(item => item.WaybillID).Distinct();


    //            var noticesView = new PickingNoticesRoll(reponsitory);
    //            var noticeView = from notice in noticesView
    //                             where wblsID.Contains(notice.WaybillID) && notice.Status != NoticesStatus.Closed
    //                             select notice;

    //            var notices = noticeView.ToArray();


    //            var inputids = notices.Select(item => item.InputID).Distinct().ToArray();

    //            var inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => inputids.Contains(item.ID)).Select(item => new Yahv.Services.Models.Input
    //            {
    //                ID = item.ID,
    //                ClientID = item.ClientID,
    //                Code = item.Code,
    //                CreateDate = item.CreateDate,
    //                Currency = (Currency)item.Currency,
    //                OrderID = item.OrderID,
    //                ItemID = item.ItemID,
    //                UnitPrice = item.UnitPrice,
    //                ProductID = item.ProductID,
    //                OriginID = item.OriginID,
    //                PayeeID = item.PayeeID,
    //                PurchaserID = item.PurchaserID,
    //                SalerID = item.SalerID,
    //                ThirdID = item.ThirdID,
    //                TinyOrderID = item.TinyOrderID,
    //                TrackerID = item.TrackerID
    //            }
    //                ).ToArray();

    //            var ntcsID = notices.Select(item => item.ID).Distinct().ToArray();
    //            var orderids = notices.Select(item => item.Output.OrderID).Distinct().ToArray();

    //            var linq_pickings = from entity in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
    //                                where ntcsID.Contains(entity.NoticeID)
    //                                group entity by entity.NoticeID into tem
    //                                select new { tem.Key, Quantity = tem.Sum(item => item.Quantity) };

    //            var pickings = linq_pickings.ToArray();

    //            var storageids = notices.Select(item => item.StorageID).Distinct().ToArray();
    //            var storages = new Yahv.Services.Views.StoragesTopView<PvWmsRepository>(reponsitory).Where(item => storageids.Contains(item.ID)).Select(item => new { item.ID, item.Quantity, item.ShelveID }).ToArray();

    //            var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
    //                            where wblsID.Contains(file.WaybillID) || ntcsID.Contains(file.NoticeID) || orderids.Contains(file.WsOrderID)
    //                            select new Yahv.Services.Models.CenterFileDescription
    //                            {
    //                                ID = file.ID,
    //                                WaybillID = file.WaybillID,
    //                                NoticeID = file.NoticeID,
    //                                StorageID = file.StorageID,
    //                                CustomName = file.CustomName,
    //                                Type = file.Type,
    //                                Url = Yahv.Services.Models.CenterFile.Web + file.Url,
    //                                CreateDate = file.CreateDate,
    //                                ClientID = file.ClientID,
    //                                AdminID = file.AdminID,
    //                                InputID = file.InputID,
    //                                Status = (Yahv.Services.Models.FileDescriptionStatus)file.Status,
    //                            };

    //            var files = filesView.ToArray();


    //            return arry.Select(waybill =>
    //            {

    //                var ns = notices.Where(item => item.WaybillID == waybill.WaybillID && item.Status != NoticesStatus.Closed);
    //                if (ns.Count() > 0)
    //                {
    //                    var cur = ns.First().Output.Currency;
    //                    var totalGoodsVaoue = cur?.GetCurrency().ShortSymbol + ns.Select(item => item.Quantity * item.Output.Price).Sum()?.Fours();
    //                    waybill.TotalGoodsValue = totalGoodsVaoue;
    //                    waybill.TotalPieces = ns.Sum(item => item.Quantity);
    //                }

    //                waybill.Notices = notices.Where(notice => notice.WaybillID == waybill.WaybillID)?.Select(item =>
    //                {
    //                    item.Files = files.Where(tem => tem.NoticeID == item.ID).ToArray();
    //                    var picking = pickings.Where(tem => tem.Key == item.ID).FirstOrDefault();
    //                    item.HappenedQuantity = picking?.Quantity ?? 0;
    //                    var storage = storages.Where(tem => tem.ID == item.StorageID)?.FirstOrDefault();
    //                    item.StockQuantity = storage?.Quantity ?? 0;
    //                    item.ShelveID = storage?.ShelveID; item.Input = inputs.Where(tem => tem.ID == item.InputID)?.FirstOrDefault();
    //                    return item;
    //                }).ToArray();
    //                waybill.DataFiles = files.Where(item => item.WaybillID == waybill.WaybillID).ToArray();
    //                return waybill;
    //            }).ToArray();
    //        }
    //        finally
    //        {
    //            if (rep == null)
    //            {
    //                reponsitory.Dispose();
    //            }
    //        }

    //    }

    //    public static DataPickingWaybill Single(this IQueryable<DataPickingWaybill> iquery, string waybillid)
    //    {
    //        return iquery.Where(item => item.WaybillID == waybillid).ToFillArray().FirstOrDefault();
    //    }

    //    /// <summary>
    //    /// 分页
    //    /// </summary>
    //    /// <param name="iquery"></param>
    //    /// <param name="pageIndex"></param>
    //    /// <param name="pageSize"></param>
    //    /// <returns></returns>
    //    static public object ToFillPage(this IQueryable<DataPickingWaybill> iquery, int pageIndex = 1, int pageSize = 20)
    //    {
    //        int total = iquery.Count();
    //        var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

    //        return new
    //        {
    //            Total = total,
    //            Size = pageSize,
    //            Index = pageIndex,
    //            Data = query.ToFillArray().Select(item => (PickingWaybill)item).ToArray()
    //        };

    //    }

    //    /// <summary>
    //    /// 分页
    //    /// </summary>
    //    /// <param name="iquery"></param>
    //    /// <param name="pageIndex"></param>
    //    /// <param name="pageSize"></param>
    //    /// <returns></returns>
    //    static public object ToPage(this IQueryable<DataPickingWaybill> iquery, int pageIndex = 1, int pageSize = 20)
    //    {
    //        int total = iquery.Count();
    //        var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

    //        var list = query.ToList();
    //        if (list.Count() > 0)
    //        {
    //            var wids = list.Select(item => item.WaybillID).Distinct();
    //            var sources = new Layers.Data.Sqls.PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(t => wids.Contains(t.WaybillID)).Select(item => new { item.WaybillID, item.Source }).ToList();
    //            foreach (var item in list)
    //            {
    //                item.Source = (CgNoticeSource)sources.Where(tem => tem.WaybillID == item.WaybillID).FirstOrDefault().Source;
    //            }
    //        }
    //        return new
    //        {
    //            Total = total,
    //            Size = pageSize,
    //            Index = pageIndex,
    //            Data = list.Select(item => (PickingWaybill)item)
    //        };
    //    }
    //}

    public class PickingNoticesRoll : QueryView<PickingNotice, PvWmsRepository>
    {
        internal PickingNoticesRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PickingNotice> GetIQueryable()
        {


            var productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            return from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                   join product in productView on notice.ProductID equals product.ID
                   join output in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                   where notice.Status != (int)NoticesStatus.Closed && output.ID == notice.OutputID
                   select new PickingNotice
                   {
                       ID = notice.ID,
                       WaybillID = notice.WaybillID,
                       InputID = notice.InputID,
                       ProductID = notice.ProductID,
                       Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                       Source = (CgNoticeSource)notice.Source,
                       Status = (NoticesStatus)notice.Status,
                       Supplier = notice.Supplier,
                       Target = (NoticesTarget)notice.Target,
                       Type = (CgNoticeType)notice.Type,
                       WareHouseID = notice.WareHouseID,
                       DateCode = notice.DateCode,
                       Quantity = notice.Quantity,
                       BoxCode = notice.BoxCode,
                       ShelveID = notice.ShelveID,
                       Volume = notice.Volume,
                       Weight = notice.Weight,
                       NetWeight = notice.NetWeight,
                       Product = product,
                       BoxingSpecs = notice.BoxingSpecs,
                       StorageID = notice.StorageID,
                       Output = new Yahv.Services.Models.Output
                       {
                           OrderID = output.OrderID,
                           Price = output.Price,
                           Currency = (Currency)output.Currency,

                       }


                   };

        }
    }


}
