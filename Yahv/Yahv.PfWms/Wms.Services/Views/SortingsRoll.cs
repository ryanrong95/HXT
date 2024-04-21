using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Layers.Data.Sqls.PvWms;
using Yahv.Utils.Converters.Contents;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using System.Linq.Expressions;


/*
 * join summary in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Summaries>() on waybill.wbID equals summary.ID
*/

namespace Wms.Services.Views
{



    /// <summary>
    /// 分拣运单
    /// </summary>
    /// <remarks>
    ///  入库
    ///  出库
    ///  在库
    ///  对外通讯
    /// </remarks>
    public class SortingsWayBillRoll : QueryView<Models.DataSortingWaybill, PvWmsRepository>
    {

        public SortingsWayBillRoll()
        {

        }

        public SortingsWayBillRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public SortingsWayBillRoll(PvWmsRepository reponsitory, IQueryable<DataSortingWaybill> iQueryable) : base(reponsitory, iQueryable)
        {
        }


        protected override IQueryable<Models.DataSortingWaybill> GetIQueryable()
        {

            var clientView = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory);

            return from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                   join c in clientView on waybill.EnterCode equals c.EnterCode into clients
                   from client in clients.DefaultIfEmpty()
                   orderby waybill.CreateDate descending
                   select new Models.DataSortingWaybill
                   {
                       WaybillID = waybill.ID,
                       EnterCode = waybill.EnterCode,
                       ClientName = client == null ? null : client.Name,
                       CreateDate = waybill.CreateDate,
                       CarrierID = waybill.CarrierID,
                       Supplier = waybill.Supplier,
                       Condition = waybill.Condition,
                       ConsignorID = waybill.ConsignorID,
                       ExcuteStatus = waybill.ExcuteStatus,
                       Code = waybill.Code,
                       Place = waybill.Consignor.Place,
                       WaybillType = waybill.Type,
                       Status = waybill.Status,
                       Packaging = waybill.Packaging,
                       TransferID = waybill.TransferID,
                       FatherID = waybill.FatherID,
                       WayLoading = waybill.WayLoading,
                       Consignee = waybill.Consignee,
                       Consignor = waybill.Consignor,
                       CarrierName = waybill.CarrierName,
                       Summary = waybill.Summary,
                       WayCharge = waybill.WayCharge,
                       WayChcd = waybill.WayChcd,
                       ConsigneeID = waybill.ConsigneeID,
                       TotalParts = waybill.TotalParts,
                       TotalVolume = waybill.TotalVolume,
                       TotalWeight = waybill.TotalWeight,


                       //Summary=waybill.
                   };
        }

        public SortingsWayBillRoll SearchByPartNumber(string name)
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

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>内部页面分拣使用</remarks>
        public SortingWaybill SearchByPartNumberOrManufacturer(DataSortingWaybill roll, string name)
        {
            var notices = roll.Notices.Where(item => item.Product.PartNumber.StartsWith(name) || item.Product.Manufacturer.StartsWith(name));
            roll.Notices = notices.ToArray();
            return roll;
        }

        public SortingsWayBillRoll SearchByManufacturer(string name)
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

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchBySupplier(string name)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.Supplier.Contains(name)
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchByStartDate(DateTime startdate)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.CreateDate >= startdate
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchByEndDate(DateTime enddate)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.CreateDate < enddate.AddDays(1)
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }


        public SortingsWayBillRoll SearchByWaybillID(string waybillid)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.WaybillID == waybillid
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchByID(string id)
        {

            var waybillids = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             where waybill.wbCode.Contains(id) || waybill.wbEnterCode.Contains(id) || waybill.wbID.Contains(id)
                             select waybill.wbID;

            var waybillIDView = from input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on input.ID equals notice.InputID
                                where input.OrderID.Contains(id) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            List<string> wids = new List<string>();
            wids.AddRange(waybillids);
            wids.AddRange(waybillIDView);

            var topN = wids.Distinct().OrderByDescending(item => item);

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchByStatus(params SortingExcuteStatus[] status)
        {
            var arry = status.Select(item => (int?)item);
            var iQuery = from waybill in this.IQueryable
                         where arry.Contains(waybill.ExcuteStatus)
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchBySource(NoticeSource source)
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

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchByWareHouseID(string warehouseID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where notice.WareHouseID.Contains(warehouseID) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            List<string> topN = new List<string>();

            topN.AddRange(waybillIDView.Distinct().OrderBy(item => item).Take(500));

            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            var roll = new SortingsWayBillRoll(this.Reponsitory, iQuery);

            return roll;
        }


        public SortingsWayBillRoll SearchByNoticeType(params NoticeType[] types)
        {
            var whereIn = types.Select(item => (int)item);
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where whereIn.Contains(notice.Type) && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            //var dist = waybillIDView.Distinct().Take(50); 不能取前多少，这样有些数据就查不出来了。

            var dist = waybillIDView.Distinct();

            var iQuery = from waybill in this.IQueryable
                         join id in dist on waybill.WaybillID equals id
                         select waybill;

            var roll = new SortingsWayBillRoll(this.Reponsitory, iQuery);

            return roll;
        }

        public SortingsWayBillRoll SearchByWaybillD(string id)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.WaybillID == id
                         select waybill;

            return new SortingsWayBillRoll(this.Reponsitory, iQuery);
        }

        public SortingsWayBillRoll SearchByDataStatus(GeneralStatus status)
        {

            var iQuery = from waybill in this.IQueryable
                         where waybill.Status == status
                         select waybill;

            var roll = new SortingsWayBillRoll(this.Reponsitory, iQuery);

            return roll;
        }


        ///// <summary>
        ///// 历史到货
        ///// </summary>
        ///// <param name="waybaillid"></param>
        ///// <returns></returns>
        //public string[] History(string waybaillid)
        //{
        //    var code = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Where(item => item.wbID == waybaillid).FirstOrDefault().wbCode;
        //    var linq = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()                       
        //               join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on notice.ID equals storage.NoticeID
        //               where notice.WaybillID == waybaillid
        //               select string.Concat(storage.CreateDate.ToString("yyyy-MM-dd"),"_",code);
        //    return linq.Distinct().ToArray();
        //}

        public string[] Historys(string waybaillid)
        {
            var linq = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                       join sortings in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>() on waybill.wbID equals sortings.WaybillID
                       where waybill.wbFatherID == waybaillid
                       orderby waybill.wbID descending
                       select new { waybill.wbID };
            return linq.Select(item => string.Concat(item.wbID)).Distinct().ToArray();

        }


    }

    static public class SortingsWayBillExtends
    {

        /// <summary>
        /// 是数据库保存
        /// </summary>
        /// <param name="ienum"></param>
        static public void Enter(this IEnumerable<SortingWaybill> ienum)
        {
            using (PvWmsRepository repository = new PvWmsRepository())
            {

                var notices = new int[5];
                var waybill = new
                {
                    Notice = notices
                };

                repository.Insert(notices.Select(item => new Layers.Data.Sqls.PvWms.Notices
                {

                }).ToArray());

                Layers.Data.Sqls.PvWms.Inputs[] inputs;

                repository.Insert(inputs = notices.Select(item => new Layers.Data.Sqls.PvWms.Inputs
                {

                }).ToArray());

                repository.Insert(notices.Select(item => new Layers.Data.Sqls.PvWms.Notices
                {

                }).ToArray());


                //Action<>

                //foreach (var waybill in ienum)
                //{
                //    foreach (var notice in waybill.Notices)
                //    {
                //        //repository.Insert();

                //    }
                //}
            }
        }



        //推荐库位
        //private static string RecomandShelve(string warehouseID, string clientID, string inputID = null)
        //{
        //    //using (var rep = new Layers.Data.Sqls.PvWmsRepository())
        //    //{
        //    //    if (!string.IsNullOrEmpty(inputID))
        //    //    {
        //    //        var storages = rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => item.InputID == inputID).ToArray();
        //    //        if (storages.Length > 0)
        //    //        {
        //    //            return storages.First().ShelveID;
        //    //        }
        //    //    }

        //    //    string[] shelveids = null;
        //    //    var regions = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()/*.Where(item => item.FatherID == warehouseID)*/.ToList();
        //    //    if (regions.Count <= 0)
        //    //    {
        //    //        return "";
        //    //    }
        //    //    var ids = regions.Select(item => item.ID).ToArray();
        //    //    Expression<Func<Layers.Data.Sqls.PvWms.Shelves, bool>> exp = null;
        //    //    foreach (var id in ids)
        //    //    {
        //    //        exp = exp.Or(item => item.ID.StartsWith(id));
        //    //    }

        //    //    var shelves = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(exp).ToList();
        //    //    if (shelves.Count <= 0)
        //    //    {
        //    //        return "";
        //    //    }

        //    //    var currentSehlveids = shelves.Select(item => item.ID).ToArray();
        //    //    var ShelvesStock = rep.ReadTable<Layers.Data.Sqls.PvWms.ShelvesStockTopView>().Where(item => currentSehlveids.Contains(item.ID) && item.Quantity == 0).ToList();

        //    //    if (ShelvesStock.Count <= 0)
        //    //    {
        //    //        return "";
        //    //    }

        //    //    //得到租赁的库位
        //    //    var ls = rep.ReadTable<Layers.Data.Sqls.PvWms.LsNotice>().Where(item => item.ClientID == clientID && item.EndDate < DateTime.Now.AddDays(1)).ToList();
        //    //    if (ShelvesStock != null && ls.Count > 0)
        //    //    {
        //    //        var lsid = ls.Select(item => item.ID).ToArray();
        //    //        shelveids = ShelvesStock.Where(item => lsid.Contains(item.LeaseID) && currentSehlveids.Contains(item.ID)).Select(item => item.ID).ToArray();
        //    //        if (shelveids.Length > 0)
        //    //        {
        //    //            return shelveids[new Random().Next(0, shelveids.Length)]; ;
        //    //        }
        //    //    }
        //    //    //得到最近使用的
        //    //    var useds = rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => item.ClientID == clientID).ToList();
        //    //    if (useds.Count > 0)
        //    //    {
        //    //        var inputids = useds.Select(item => item.ID).ToArray();
        //    //        shelveids = rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => inputids.Contains(item.InputID) && currentSehlveids.Contains(item.ShelveID) && item.ShelveID != null && item.ShelveID != "").OrderByDescending(item => item.CreateDate).Select(item => item.ShelveID).ToArray();
        //    //        if (shelveids.Length > 0)
        //    //        {
        //    //            return shelveids.First();
        //    //        }
        //    //    }

        //    //    //随机库存为0的库位号
        //    //    var randomshelves = ShelvesStock.Where(item => item.Quantity == 0).ToList();
        //    //    if (randomshelves.Count > 0)
        //    //    {
        //    //        shelveids = randomshelves.Select(item => item.ID).ToArray();
        //    //        return shelveids[new Random().Next(0, shelveids.Length)];
        //    //    }
        //    //    return "";
        //    //}
        //}
        static public DataSortingWaybill[] ToFillArray(this IQueryable<DataSortingWaybill> iquery, PvWmsRepository rep = null)
        {
            throw new Exception("现有逻辑错误，Storage中不再包含NoticeID");
            /*
            var reponsitory = rep ?? new PvWmsRepository();
            try
            {
                var wabillids = new List<string>();
                var arry = iquery.ToArray();
                var wblsID = arry.Select(item => item.WaybillID).Distinct();
                wabillids.AddRange(wblsID);

                var noticesView = new SortingNoticesRoll(reponsitory);
                var noticeView = from notice in noticesView
                                 where wblsID.Contains(notice.WayBillID) && notice.Status != NoticesStatus.Closed
                                 select notice;

                var notices = noticeView.ToArray();

                var OriginIDs = notices.Select(item => item.InputID).Distinct().ToArray();

                //拼接 NoticeID =>  Sotrings => Storages

                var ntcsID = notices.Select(item => item.ID).Distinct().ToArray();
                var orderids = notices.Select(item => item.Input.OrderID).Distinct().ToArray();

                var linq_sortings = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Where(item => ntcsID.Contains(item.NoticeID));

                //分拣
                var sortings = linq_sortings.ToArray();
                var sortingsIDs = sortings.Select(item => item.ID);
                

                //var linq_inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => OriginIDs.Contains(item.ID) || OriginIDs.Contains(item.OriginID)).ToArray();
                //var linq_inputs = notices.Select(item => item.Input);
                //var productids = linq_inputs.Select(item => item.ProductID).Distinct().ToArray();

                var ostorages = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => sortingsIDs.Contains(item.SortingID)).ToArray();
                var productids = ostorages.Select(item => item.ProductID).Distinct();

                var products = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(reponsitory).Where(item => productids.Contains(item.ID));

                //库存
                var linq_storages = from storage in ostorages
                                    join product in products on storage.ProductID equals product.ID

                                    select new
                                    {
                                        SortingID = storage.SortingID,
                                        PartNumber = product.PartNumber,
                                        Manufacturer = product.Manufacturer,
                                        PackageCase = product.PackageCase,
                                        Packaging = product.Packaging,

                                        Quantity = storage.Quantity,
                                        Origin = storage.Origin,
                                        Supplier = storage.Supplier,
                                        DateCode = storage.DateCode,

                                        ShelveID = storage.ShelveID,
                                        storage.InputID,

                                    };
                var storages = linq_storages.ToArray();
                var inputids = storages.Select(item => item.InputID).Distinct();

                var linq_inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => inputids.Contains(item.ID)).ToArray();

                //分拣历史
                var sorted = from sort in sortings
                             join storage in storages on sort.ID equals storage.SortingID into _storages

                             select new
                             {
                                 sort.ID,
                                 sort.AdminID,
                                 sort.BoxCode,
                                 sort.Volume,
                                 sort.Weight,
                                 sort.NetWeight,
                                 sort.WaybillID,
                                 sort.NoticeID,
                                    
                                 SortingDate = sort.CreateDate,
                                 SortQuantity = sort.Quantity,
                                 Storages = _storages,

                             };

                wabillids.AddRange(sorted.Select(item => item.WaybillID).Distinct());

               var storagelinq = from storage in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                  where ntcsID.Contains(storage.NoticeID)
                                  select new
                                  {
                                      storage.InputID,
                                      storage.SortingID,
                                      storage.NoticeID,
                                      storage.Quantity
                                  };

                var stocks = storagelinq.ToArray();

                var filesView = from file in new Yahv.Services.Views.CenterFilesTopView()
                                where wabillids.Contains(file.WaybillID) || ntcsID.Contains(file.NoticeID) || orderids.Contains(file.WsOrderID)
                                select new CenterFileDescription
                                {

                                    ID = file.ID,
                                    WaybillID = file.WaybillID,
                                    NoticeID = file.NoticeID,
                                    StorageID = file.StorageID,
                                    CustomName = file.CustomName,
                                    Type = file.Type,
                                    Url = CenterFile.Web + file.Url,
                                    CreateDate = file.CreateDate,
                                    ClientID = file.ClientID,
                                    AdminID = file.AdminID,
                                    InputID = file.InputID,
                                    Status = file.Status,
                                };

                var files = filesView.ToArray();


                var arrys = arry.Select(waybill =>
                {
                    waybill.Notices = notices.Where(notice => notice.WayBillID == waybill.WaybillID).Select(item =>
                    {
                        item.ShelveID = RecomandShelve(item.WareHouseID, item.Input.ClientID, item.InputID);

                        var sorts = sorted.Where(sort => sort.NoticeID == item.ID);

                        //item.Sorted = sorts.Select(sort => new
                        //{

                        //    sort.NoticeID,
                        //    sort.ID,
                        //    sort.SortingDate,
                        //    sort.SortQuantity,
                        //    sort.AdminID,

                        //    Storage = sort.Storages.FirstOrDefault()
                        //});



                        if (sorts.Count() > 0)
                        {

                            item.Sorted = sorts.Select(sort =>
                            {

                                var input = linq_inputs.Where(tem => tem.ID == sort.Storages.Where(t=>t.InputID==tem.ID)?.FirstOrDefault()?.InputID).FirstOrDefault();

                                return new Models.SortingNotice
                                {
                                    ID = item.ID,
                                    BoxCode = sort.BoxCode,
                                    InputID = sort.Storages?.FirstOrDefault()?.InputID,
                                    Weight = sort.Weight,
                                    NetWeight = sort.NetWeight,
                                    Volume = sort.Volume,
                                    ShelveID = RecomandShelve(item.WareHouseID, item.Input.ClientID, sort.Storages?.FirstOrDefault()?.InputID),
                                    ProductID = input?.ProductID,
                                    Input = input == null ? null : new Input
                                    {
                                        ID = input.ID,
                                        OriginID = input.OriginID,
                                        PayeeID = input.PayeeID,
                                        ClientID = input.ClientID,
                                        Code = input.ID,
                                        Currency = (Currency)input.Currency,
                                        ItemID = input.ItemID,
                                        OrderID = input.OrderID,
                                        TinyOrderID = input.TinyOrderID,
                                        SalerID = input.SalerID,
                                        ProductID = input.ProductID,
                                        PurchaserID = input.PurchaserID,
                                        ThirdID = input.ThirdID,
                                        CreateDate = input.CreateDate,
                                        UnitPrice = input.UnitPrice,
                                        TrackerID = input.TrackerID
                                    },
                                    Product = new CenterProduct
                                    {
                                        PartNumber = sort.Storages?.FirstOrDefault()?.PartNumber,
                                        Manufacturer = sort.Storages?.FirstOrDefault()?.Manufacturer,
                                        PackageCase = sort.Storages?.FirstOrDefault()?.PackageCase,
                                        Packaging = sort.Storages?.FirstOrDefault()?.Packaging,

                                    },                                    
                                    DateCode = sort.Storages?.FirstOrDefault()?.DateCode,
                                    WareHouseID = item.WareHouseID,
                                    WayBillID = sort.WaybillID,
                                    Type = item.Type,
                                    Condition = item.Condition,
                                    Sorted = new Models.SortingNotice[] { },
                                    Source = item.Source,
                                    TruetoQuantity = sort.SortQuantity,
                                    SortedQuantity = sort.SortQuantity,
                                    Quantity = sort.SortQuantity,
                                    Files = files.Where(file => file.InputID == sort.Storages?.FirstOrDefault()?.InputID).ToArray(),
                                    Enabled = false,
                                    IsOriginalNotice = false
                                };


                            }).ToArray();
                        }
                        else
                        {
                            item.Sorted = new Models.SortingNotice[] { };
                        }



                        item.SortedQuantity = sorts.Sum(sort => sort.SortQuantity);
                        item.Enabled = true;
                        item.ClientID = item.Input.ClientID;
                        return item;
                    }).ToArray();

                    waybill.DataFiles = files;
                    waybill.ClientID = waybill.Notices?.FirstOrDefault()?.ClientID;
                    
                    return waybill;
                }).ToArray();

                return arrys;


            }
            finally
            {
                if (rep == null)
                {
                    reponsitory.Dispose();
                }
            }
            */
        }

        public static DataSortingWaybill Single(this IQueryable<DataSortingWaybill> iquery, string waybillid)
        {
            return iquery.Where(item => item.WaybillID == waybillid).ToFillArray().FirstOrDefault();
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="iquery"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        static public object ToFillPage(this IQueryable<DataSortingWaybill> iquery, int pageIndex = 1, int pageSize = 20)
        {
            int total = iquery.Count();
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query.ToFillArray()
            };

        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="iquery"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        static public object ToPage(this IQueryable<DataSortingWaybill> iquery, int pageIndex = 1, int pageSize = 20)
        {


            int total = iquery.Count();
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            //new Layers.Data.Sqls.PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(t => t.WaybillID == item.WaybillID).FirstOrDefault().Source
            var list = query.ToList();
            if (list.Count() > 0)
            {
                try
                {
                    var listwids = new List<string>();
                    listwids.AddRange(list.Select(item => item.WaybillID).Distinct());
                    listwids.AddRange(list.Select(item => item.FatherID).Distinct());
                    var wids = listwids.Distinct().ToArray();
                    var sources = new Layers.Data.Sqls.PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(t => wids.Contains(t.WaybillID)).Select(item => new { item.WaybillID, item.Source }).ToList();
                    if (sources.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.Source = (NoticeSource)sources?.Where(tem => tem.WaybillID == item.WaybillID)?.FirstOrDefault()?.Source;
                        }
                    }
                }
                catch
                { }
            }

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = list.Select(item => (SortingWaybill)item)
            };
        }
    }

    public class SortingNoticesRoll : QueryView<Models.SortingNotice, PvWmsRepository>
    {
        internal SortingNoticesRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.SortingNotice> GetIQueryable()
        {
            var productView = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);

            var linq = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                       join product in productView on notice.ProductID equals product.ID
                       join input in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on notice.InputID equals input.ID
                       select new Models.SortingNotice
                       {
                           ID = notice.ID,
                           PID = notice.ID,
                           WayBillID = notice.WaybillID,
                           InputID = notice.InputID,
                           ProductID = notice.ProductID,
                           Condition = notice.Conditions.JsonTo<NoticeCondition>(),
                           Source = (NoticeSource)notice.Source,
                           Status = (NoticesStatus)notice.Status,
                           Supplier = notice.Supplier,
                           Target = (NoticesTarget)notice.Target,
                           Type = (NoticeType)notice.Type,
                           WareHouseID = notice.WareHouseID,
                           DateCode = notice.DateCode,                           
                           Quantity = notice.Quantity,
                           TruetoQuantity = null,
                           BoxCode = notice.BoxCode,
                           ShelveID = notice.ShelveID,
                           Volume = notice.Volume,
                           Weight = notice.Weight,
                           NetWeight=notice.NetWeight,
                           Product = product,
                           Input = new Input
                           {
                               ID = input.ID, //四位年+2位月+2日+6位流水
                               Code = input.Code, //全局唯一码
                               OriginID = input.OriginID == null ? input.ID : input.OriginID,
                               OrderID = input.OrderID, //MainID                               
                               TinyOrderID = input.TinyOrderID,
                               ItemID = input.ItemID, //项ID
                               ClientID = input.ClientID, //所属企业
                               TrackerID = input.TrackerID, //跟单员
                               SalerID = input.SalerID, //AdminID
                               PurchaserID = input.PurchaserID, //采购员
                               Currency = (Currency)input.Currency, //保值
                               UnitPrice = input.UnitPrice, //保值
                               ProductID = input.ProductID,
                               CreateDate = input.CreateDate,
                               PayeeID = input.PayeeID,
                               ThirdID = input.ThirdID
                           },
                       };

            return linq;
        }


    }
}
