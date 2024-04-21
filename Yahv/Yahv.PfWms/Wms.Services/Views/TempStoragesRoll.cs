using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Wms.Services.Views
{
    public class TempStoragesWayBillRoll : QueryView<Models.DataTempStorageWaybill, PvWmsRepository>
    {
        public TempStoragesWayBillRoll()
        {

        }
        public TempStoragesWayBillRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public TempStoragesWayBillRoll(PvWmsRepository reponsitory, IQueryable<DataTempStorageWaybill> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DataTempStorageWaybill> GetIQueryable()
        {
            return from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                   orderby waybill.CreateDate descending
                   select new Models.DataTempStorageWaybill
                   {
                       WaybillID = waybill.ID,
                       EnterCode = waybill.EnterCode,
                       CreateDate = waybill.CreateDate,
                       CarrierID = waybill.CarrierID,
                       Supplier = waybill.Supplier,
                       Condition = waybill.Condition,
                       ConsignorID = waybill.ConsignorID,
                       ExcuteStatus = waybill.ExcuteStatus,
                       Code = waybill.Code,
                       Place = waybill.Consignor.Place,
                       WaybillType = waybill.Type,
                       Summary = waybill.Summary,
                       CarrierName = waybill.CarrierName,
                       Consignor = waybill.Consignor,
                       TempEnterCode = waybill.TempEnterCode//入库单号
                   };
        }

        /// <summary>
        /// 根据库房编号查询暂存库存
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByWarehouseID(string warehouseID)
        {
            var waybillIDView = from storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                join sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                on storage.SortingID equals sorting.ID
                                where storage.WareHouseID == warehouseID
                                select sorting.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据库位号查询暂存运单
        /// </summary>
        /// <param name="shelveID"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByShelveID(string shelveID)
        {
            var waybillIDView = from storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                join sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                on storage.SortingID equals sorting.ID
                                where storage.ShelveID == shelveID
                                select sorting.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据状态查询暂存运单
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByStatus(TempStockExcuteStatus status)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.wbExcuteStatus == (int)status
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据入仓号查询暂存运单
        /// </summary>
        /// <param name="enterCode"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByEnterCode(string enterCode)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.wbEnterCode == enterCode
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据入库单号查询暂存运单
        /// </summary>
        /// <param name="tepmEnterCode"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByTempEnterCode(string tepmEnterCode)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.TempEnterCode == tepmEnterCode
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据承运商查询暂存运单
        /// </summary>
        /// <param name="carrierID"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByCarrierID(string carrierID)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.wbCarrierID == carrierID
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);
            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;
            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据输送地（原产地）查询暂存运单
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByPlace(string place)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.corPlace == place
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);
            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;
            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据运单编号查询
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByID(string waybillID)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.WaybillID == waybillID
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据运单Code查询
        /// </summary>
        /// <param name="waybillCode"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByCode(string waybillCode)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.Code.Contains(waybillCode)
                         select waybill;

            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }


        /// <summary>
        /// 根据时间查询
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public TempStoragesWayBillRoll SearchByDate(string date)
        {
            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.wbCreateDate.Date.ToString().Contains(date)
                             select waybill.wbID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);
            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;
            return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        }

        //public TempStoragesWayBillRoll SearchByPartNumber(string partNumber)
        //{
        //    var _productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

        //    var waybillIDView = from product in _productView
        //                        join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
        //                        where product.PartNumber.Contains(partNumber) && notice.Status != (int)Yahv.Services.Enums.NoticesStatus.Closed
        //                        select notice.WaybillID;

        //    var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
        //    var iQuery = from waybill in this.IQueryable
        //                 where topN.Contains(waybill.WaybillID)
        //                 select waybill;

        //    return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        //}

        //public TempStoragesWayBillRoll SearchByManufacture(string manufacture)
        //{
        //    var _productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

        //    var waybillIDView = from product in _productView
        //                        join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
        //                        where product.Manufacturer.Contains(manufacture) && notice.Status != (int)Yahv.Services.Enums.NoticesStatus.Closed
        //                        select notice.WaybillID;

        //    var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
        //    var iQuery = from waybill in this.IQueryable
        //                 where topN.Contains(waybill.WaybillID)
        //                 select waybill;

        //    return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        //}

        //public TempStoragesWayBillRoll SearchByQuantity(int? quantity)
        //{
        //    var waybillIDView = from storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
        //                        join sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
        //                        on storage.SortingID equals sorting.ID
        //                        where storage.Quantity == quantity
        //                        select sorting.WaybillID;

        //    var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
        //    var iQuery = from waybill in this.IQueryable
        //                 where topN.Contains(waybill.WaybillID)
        //                 select waybill;

        //    return new TempStoragesWayBillRoll(this.Reponsitory, iQuery);
        //}
    }

    static public class TempStoragesWayBillExtends
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="iquery"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        static public object ToPage(this IQueryable<DataTempStorageWaybill> iquery, int pageIndex = 1, int pageSize = 20)
        {
            int total = iquery.Count();
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            //var reponsitory = new PvWmsRepository();
            //var arry = iquery.ToArray();
            //var wblsID = arry.Select(item => item.WaybillID).Distinct();
            ////获得所有的库存+分拣+产品
            //var storagesView = new TempStorageRoll(reponsitory);

            ////根据运单编号获得对应的库存+分拣+产品
            //var storageView = from storage in storagesView
            //                  where wblsID.Contains(storage.Sorting.WaybillID)
            //                  select storage;
            //var storages = storageView.ToArray();

            //arry = arry.Select(waybill =>
            // {
            //     waybill.ProductStorages = storages.Where(storage => string.IsNullOrWhiteSpace(storage.Summary)).ToArray();
            //     waybill.SummaryStorages = storages.Where(storage => string.IsNullOrWhiteSpace(storage.ProductID)).ToArray();
            //     return waybill;
            // }).ToArray();
            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query.Select(item => (DataTempStorageWaybill)item).ToArray()
            };
        }


        /// <summary>
        /// 暂存运单详情的处理
        /// </summary>
        /// <param name="iquery"></param>
        /// <param name="rep"></param>
        /// <returns></returns>
        static public DataTempStorageWaybill[] ToFillArray(this IQueryable<DataTempStorageWaybill> iquery, PvWmsRepository rep = null)
        {
            var reponsitory = rep ?? new PvWmsRepository();
            try
            {
                var arry = iquery.ToArray();
                var wblsID = arry.Select(item => item.WaybillID).Distinct();

                //获得所有的库存+分拣+产品
                var storagesView = new TempStorageRoll(reponsitory);

                //根据运单编号获得对应的库存+分拣+产品
                var storageView = from storage in storagesView
                                  where wblsID.Contains(storage.Sorting.WaybillID)
                                  select storage;
                var storages = storageView.ToArray();

                var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                                where wblsID.Contains(file.WaybillID)
                                select new CenterFileDescription
                                {
                                    ID = file.ID,
                                    WaybillID = file.WaybillID,
                                    NoticeID = file.NoticeID,
                                    StorageID = file.StorageID,
                                    CustomName = file.CustomName,
                                    Type = file.Type,
                                    Url = file.Url,
                                    CreateDate = file.CreateDate,
                                    ClientID = file.ClientID,
                                    AdminID = file.AdminID,
                                    InputID = file.InputID,
                                    Status = (FileDescriptionStatus)file.Status,
                                };

                var files = filesView.ToArray();

                return arry.Select(waybill =>
                {
                    waybill.ProductStorages = storages.Where(storage => string.IsNullOrWhiteSpace(storage.Summary)).ToArray();
                    waybill.SummaryStorages = storages.Where(storage => string.IsNullOrWhiteSpace(storage.ProductID)).ToArray();
                    waybill.DataFiles = files;

                    return waybill;
                }).ToArray();


            }
            catch (Exception ex)
            {

                throw ex;
            }


        }


        static public object LinkedOrder(this IQueryable<DataTempStorageWaybill> iquery, PvWmsRepository rep = null, int pageIndex = 1, int pageSize = 20)
        {
            int total = iquery.Count();

            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var reponsitory = rep ?? new PvWmsRepository();
            var arry = iquery.ToArray();
            var wblsID = arry.Select(item => item.WaybillID).Distinct();
            //获得所有的库存+分拣+产品
            var storagesView = new TempStorageRoll(reponsitory);

            //根据运单编号获得对应的库存+分拣+产品
            var storageView = from storage in storagesView
                              where wblsID.Contains(storage.Sorting.WaybillID)
                              select storage;
            var storages = storageView.ToArray();

            arry = arry.Select(waybill =>
             {
                 waybill.ProductStorages = storages.Where(storage => string.IsNullOrWhiteSpace(storage.Summary)).ToArray();
                 waybill.SummaryStorages = storages.Where(storage => string.IsNullOrWhiteSpace(storage.ProductID)).ToArray();
                 return waybill;
             }).ToArray();
            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query.Select(item => (DataTempStorageWaybill)item).ToArray()
            };
        }
    }

    public class TempStorageRoll : QueryView<TempStorage, PvWmsRepository>
    {
        internal TempStorageRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<TempStorage> GetIQueryable()
        {
            var productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            return from storage in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                   join product in productView on storage.ProductID equals product.ID into pro //左外连接
                   from p in pro.DefaultIfEmpty()
                   join sorting in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on storage.SortingID equals sorting.ID
                   select new TempStorage
                   {
                       ID = storage.ID,
                       Type = (StoragesType)storage.Type,
                       WareHouseID = storage.WareHouseID,
                       SortingID = storage.SortingID,
                       InputID = storage.InputID,
                       ProductID = storage.ProductID,
                       Quantity = storage.Quantity,
                       Place = storage.Origin/*(Origin)(int.Parse(storage.Origin ?? null))*/,
                       IsLock = storage.IsLock,
                       CreateDate = storage.CreateDate,
                       Status = (StoragesStatus)storage.Status,
                       ShelveID = storage.ShelveID,
                       Supplier = storage.Supplier,
                       DateCode = storage.DateCode,
                       Summary = storage.Summary,
                       Product = p,
                       Sorting = new Sorting
                       {
                           ID = sorting.ID,
                           NoticeID = sorting.NoticeID,
                           WaybillID = sorting.WaybillID,
                           BoxCode = sorting.BoxCode,
                           Quantity = sorting.Quantity,
                           AdminID = sorting.AdminID,
                           CreateDate = sorting.CreateDate,
                           Weight = sorting.Weight,
                           Volume = sorting.Volume,
                       }
                   };
        }
    }
}
