using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 深证库房 上架操作视图(原 待入库)
    /// </summary>
    public class SZOnStockView
    {
        private ScCustomsReponsitory _reponsitory;

        public SZOnStockView()
        {
            _reponsitory = new ScCustomsReponsitory();
        }

        public SZOnStockView(ScCustomsReponsitory reponsitory)
        {
            _reponsitory = reponsitory;
        }

        #region 1. 上架通知列表(运输批次详情)

        public class VoyageListModel
        {
            /// <summary>
            /// 运输批次号
            /// </summary>
            public string VoyageID { get; set; } = string.Empty;

            /// <summary>
            /// 承运商
            /// </summary>
            public string CarrierName { get; set; } = string.Empty;

            /// <summary>
            /// 车牌号
            /// </summary>
            public string HKLicense { get; set; } = string.Empty;

            /// <summary>
            /// 运输时间
            /// </summary>
            public DateTime? TransportTime { get; set; }

            /// <summary>
            /// 驾驶员姓名
            /// </summary>
            public string DriverName { get; set; } = string.Empty;

            /// <summary>
            /// 运输类型
            /// </summary>
            public Enums.VoyageType VoyageType { get; set; }

            /// <summary>
            /// 报关单 申报状态（回执状态）
            /// </summary>
            public string CusDecStatus { get; set; } = string.Empty;


            public string OrderID { get; set; } = string.Empty;

            public string BoxIndex { get; set; } = string.Empty;

            public DateTime PackingDate { get; set; }

            public int AllBoxNum { get; set; }

            public int StockedBoxNum { get; set; }
        }

        private IEnumerable<VoyageListModel> GetVoyageListModelBase(params LambdaExpression[] expressions)
        {
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();
            var voyages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var carriers = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>();

            var targetVoyages = from decHead in decHeads
                                join entryNotice in entryNotices
                                    on new
                                    {
                                        DecHeadID = decHead.ID,
                                        WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                        EntryNoticeNormalStatus = (int)Enums.Status.Normal,
                                        EntryNoticeStatus = (int)Enums.EntryNoticeStatus.UnBoxed,
                                    }
                                    equals new
                                    {
                                        DecHeadID = entryNotice.DecHeadID,
                                        WarehouseType = entryNotice.WarehouseType,
                                        EntryNoticeNormalStatus = entryNotice.Status,
                                        EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                                    }
                                select new VoyageListModel
                                {
                                    VoyageID = decHead.VoyNo,
                                    CusDecStatus = decHead.CusDecStatus,
                                };

            targetVoyages = targetVoyages.Where(t => t.CusDecStatus != "04");
            targetVoyages = from targetVoyage in targetVoyages
                            group targetVoyage by new { targetVoyage.VoyageID } into g
                            select new VoyageListModel
                            {
                                VoyageID = g.Key.VoyageID
                            };

            targetVoyages = from targetVoyage in targetVoyages
                            join voyage in voyages
                              on new { VoyageID = targetVoyage.VoyageID, VoyageStatus = (int)Enums.Status.Normal }
                              equals new { VoyageID = voyage.ID, VoyageStatus = voyage.Status }
                              into voyages2
                            from voyage in voyages2.DefaultIfEmpty()
                            join carrier in carriers
                                  on new { CarrierCode = voyage.CarrierCode, CarrierStatus = (int)Enums.Status.Normal }
                                  equals new { CarrierCode = carrier.Code, CarrierStatus = carrier.Status }
                                  into carriers2
                            from carrier in carriers2.DefaultIfEmpty()
                            select new VoyageListModel
                            {
                                VoyageID = targetVoyage.VoyageID,
                                CarrierName = carrier.Name,
                                HKLicense = voyage.HKLicense,
                                TransportTime = voyage.TransportTime,
                                DriverName = voyage.DriverName,
                                VoyageType = (Enums.VoyageType)((voyage.Type != null) ? voyage.Type : 0),
                            };

            foreach (var expression in expressions)
            {
                targetVoyages = targetVoyages.Where(expression as Expression<Func<VoyageListModel, bool>>);
            }

            return targetVoyages;
        }

        private IEnumerable<VoyageListModel> GetBoxNum(string[] voyageIDs)
        {
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();
            var entryNoticeItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>();
            var sortings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var packings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();



            var theDecHeads = from decHead in decHeads
                              where decHead.CusDecStatus != "04" && voyageIDs.Contains(decHead.VoyNo)
                              select decHead;

            var results = from decHead in theDecHeads
                          join entryNotice in entryNotices
                                on new
                                {
                                    DecHeadID = decHead.ID,
                                    WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                    DataStatus = (int)Enums.Status.Normal,
                                    EntryNoticeStatus = (int)Enums.EntryNoticeStatus.UnBoxed,
                                }
                                equals new
                                {
                                    DecHeadID = entryNotice.DecHeadID,
                                    WarehouseType = entryNotice.WarehouseType,
                                    DataStatus = entryNotice.Status,
                                    EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                                }
                          join entryNoticeItem in entryNoticeItems
                                  on new { EntryNoticeID = entryNotice.ID, }
                                  equals new { EntryNoticeID = entryNoticeItem.EntryNoticeID, }
                          join sorting in sortings
                                  on new { EntryNoticeItemID = entryNoticeItem.ID, }
                                  equals new { EntryNoticeItemID = sorting.EntryNoticeItemID, }
                          join packing in packings
                                  on new { OrderID = entryNotice.OrderID, BoxIndex = sorting.BoxIndex, }
                                  equals new { OrderID = packing.OrderID, BoxIndex = packing.BoxIndex, }
                          select new VoyageListModel
                          {
                              VoyageID = decHead.VoyNo,
                              OrderID = decHead.OrderID,
                              BoxIndex = sorting.BoxIndex,
                              PackingDate = packing.PackingDate,
                          };

            results = from result in results
                          //group result by new { result.VoyageID, result.OrderID, result.BoxIndex, result.PackingDate, } into g
                      group result by new { result.VoyageID, result.BoxIndex, result.PackingDate, } into g
                      select new VoyageListModel
                      {
                          VoyageID = g.Key.VoyageID,
                          //OrderID = g.Key.OrderID,
                          BoxIndex = g.Key.BoxIndex,
                          PackingDate = g.Key.PackingDate,
                      };

            return results;
        }

        private IEnumerable<BoxInfoListModel> GetStockedBoxNum(string[] voyageIDs)
        {
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();
            var entryNoticeItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>();
            var sortings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var packings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var orders = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var orderItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var storeStorages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>();

            var theDecHeads = from decHead in decHeads
                              where decHead.CusDecStatus != "04" && voyageIDs.Contains(decHead.VoyNo)
                              select decHead;

            var targetPackings = from packing in packings
                                 join order in orders
                                        on new { OrderID = packing.OrderID, PackingStatus = packing.Status, OrderStatus = (int)Enums.Status.Normal, }
                                        equals new { OrderID = order.ID, PackingStatus = (int)Enums.Status.Normal, OrderStatus = order.Status, }
                                 join decHead in theDecHeads
                                        on new { OrderID = packing.OrderID, }
                                        equals new { OrderID = decHead.OrderID, }
                                 join entryNotice in entryNotices
                                         on new
                                         {
                                             DecHeadID = decHead.ID,
                                             WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                             DataStatus = (int)Enums.Status.Normal,
                                             EntryNoticeStatus = (int)Enums.EntryNoticeStatus.UnBoxed,
                                         }
                                         equals new
                                         {
                                             DecHeadID = entryNotice.DecHeadID,
                                             WarehouseType = entryNotice.WarehouseType,
                                             DataStatus = entryNotice.Status,
                                             EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                                         }
                                 select new BoxInfoListModel
                                 {
                                     VoyageID = decHead.VoyNo,
                                     OrderID = packing.OrderID,
                                     ClientID = order.ClientID,
                                     PackingDate = packing.PackingDate,
                                     BoxIndex = packing.BoxIndex,
                                 };

            targetPackings = from targetPacking in targetPackings
                             group targetPacking by new { targetPacking.VoyageID, targetPacking.OrderID, targetPacking.ClientID, targetPacking.PackingDate, targetPacking.BoxIndex, } into g
                             select new BoxInfoListModel
                             {
                                 VoyageID = g.Key.VoyageID,
                                 OrderID = g.Key.OrderID,
                                 ClientID = g.Key.ClientID,
                                 PackingDate = g.Key.PackingDate,
                                 BoxIndex = g.Key.BoxIndex,
                             };

            var stockCodeInfos = from targetPacking in targetPackings
                                 join orderItem in orderItems
                                         on new { OrderID = targetPacking.OrderID, OrderItemStatus = (int)Enums.Status.Normal, }
                                         equals new { OrderID = orderItem.OrderID, OrderItemStatus = orderItem.Status, }
                                     into orderItems2
                                 from orderItem in orderItems2.DefaultIfEmpty()
                                 join storeStorage in storeStorages
                                         on new
                                         {
                                             BoxIndex = targetPacking.BoxIndex,
                                             OrderItemID = orderItem.ID,
                                             StoreStorageStatus = (int)Enums.Status.Normal,
                                             StoreStoragePurpose = (int)Enums.StockPurpose.Storaged
                                         }
                                         equals new
                                         {
                                             BoxIndex = storeStorage.BoxIndex,
                                             OrderItemID = storeStorage.OrderItemID,
                                             StoreStorageStatus = storeStorage.Status,
                                             StoreStoragePurpose = storeStorage.Purpose,
                                         }
                                         into storeStorages2
                                 from storeStorage in storeStorages2.DefaultIfEmpty()
                                 select new BoxInfoListModel
                                 {
                                     OrderID = targetPacking.OrderID,
                                     BoxIndex = targetPacking.BoxIndex,
                                     StockCode = storeStorage.StockCode,
                                 };

            stockCodeInfos = stockCodeInfos.Where(t => t.StockCode != null);

            stockCodeInfos = from stockCodeInfo in stockCodeInfos
                             group stockCodeInfo by new { stockCodeInfo.OrderID, stockCodeInfo.BoxIndex, stockCodeInfo.StockCode } into g
                             select new BoxInfoListModel
                             {
                                 OrderID = g.Key.OrderID,
                                 BoxIndex = g.Key.BoxIndex,
                                 StockCode = g.Key.StockCode,
                             };



            var results = from targetPacking in targetPackings
                          join stockCodeInfo in stockCodeInfos on new { OrderID = targetPacking.OrderID, BoxIndex = targetPacking.BoxIndex, }
                                 equals new { OrderID = stockCodeInfo.OrderID, BoxIndex = stockCodeInfo.BoxIndex, }
                                 into stockCodeInfos2
                          from stockCodeInfo in stockCodeInfos2.DefaultIfEmpty()
                          select new BoxInfoListModel
                          {
                              VoyageID = targetPacking.VoyageID,
                              PackingDate = targetPacking.PackingDate,
                              BoxIndex = targetPacking.BoxIndex,
                              StockCode = stockCodeInfo.StockCode,
                          };

            results = results.Select(t => new BoxInfoListModel()
            {
                VoyageID = t.VoyageID,
                PackingDate = t.PackingDate,
                BoxIndex = t.BoxIndex,
                StockCode = t.StockCode,
            }).Distinct();

            return results;
        }

        public IEnumerable<VoyageListModel> GetVoyageListModel(out int total, int page, int rows, params LambdaExpression[] expressions)
        {
            var baseView = GetVoyageListModelBase(expressions);

            total = baseView.Count();


            var theNeedDatas = baseView.Skip(rows * (page - 1)).Take(rows).ToList();

            string[] voyageIDs = theNeedDatas.Select(t => t.VoyageID).ToArray();

            var boxNums = GetBoxNum(voyageIDs).ToList();
            var stockedBoxNums = GetStockedBoxNum(voyageIDs).ToList();

            foreach (var theNeedData in theNeedDatas)
            {
                var theBoxNums = boxNums.Where(t => t.VoyageID == theNeedData.VoyageID).ToList();
                //计算个数,赋值到 AllBoxNum
                theNeedData.AllBoxNum = CaleBoxNum(theBoxNums);

                var theStockedBoxNums = stockedBoxNums.Where(t => t.VoyageID == theNeedData.VoyageID && t.StockCode != null).ToList();
                //计算个数,赋值到 StockedBoxNum
                theNeedData.StockedBoxNum = CaleBoxNum(theStockedBoxNums);
            }

            return theNeedDatas;
        }

        private int CaleBoxNum(List<VoyageListModel> theBoxNums)
        {
            var alonePack = theBoxNums.Select(item => item.BoxIndex).ToList();
            return CaleBoxNum(alonePack);
        }

        private int CaleBoxNum(List<BoxInfoListModel> theBoxNums)
        {
            var alonePack = theBoxNums.Select(item => item.BoxIndex).ToList();
            return CaleBoxNum(alonePack);
        }

        public int CaleBoxNum(string voyageID)
        {
            var targetPackings = GetTargetPackings(voyageID);
            var alonePack = targetPackings.Select(t => new { t.PackingDate, t.BoxIndex, }).Distinct().Select(t => t.BoxIndex).ToList();
            return CaleBoxNum(alonePack);
        }

        public int CaleBoxNum(List<string> alonePack)
        {
            //计算件数             
            var sumPacks = 0;

            var multi = alonePack.Where(a => a.IndexOf('-') > -1 && a.IndexOf("WL") > -1).Select(a => a.ToUpper()).Distinct().ToList();
            //var sumRepeat = 0;

            multi.ForEach(a =>
            {
                try
                {
                    var arry = a.Split('-');
                    //型号A 装了WL01；型号B装了 WL01-WL05；这种件数是5件;
                    //WL08 和WL05-WL10 同时存在  
                    int startCase = int.Parse(arry[0].Replace("WL", ""));
                    int endCase = int.Parse(arry[1].Replace("WL", ""));
                    //var repeat = alonePack.Where(c => !c.Contains("-")).Where(c => int.Parse(c.ToUpper().Replace("WL", "")) >= startCase && int.Parse(c.ToUpper().Replace("WL", "")) <= endCase).Count();
                    //20190902 解决库房WL08 与WL07-WL10同时存在的问题，算5个箱子
                    //sumRepeat += repeat;
                    sumPacks += endCase - startCase + 1;
                }
                catch (Exception ex)
                {
                    ex.CcsLog("深圳库房上架计算箱号方法，异常箱号：" + a);
                }
            });

            sumPacks += alonePack.Count() - multi.Count();  // - sumRepeat;
            return sumPacks;
        }

        #endregion

        #region 2. 上架通知详情(运输批次中所有的箱子信息)

        public decimal CalcTotalGrossWt(string voyageID)
        {
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();

            var results = from decHead in decHeads
                          join entryNotice in entryNotices
                                  on new
                                  {
                                      VoyNo = decHead.VoyNo,
                                      DecHeadID = decHead.ID,
                                      WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                      DataStatus = (int)Enums.Status.Normal,
                                      EntryNoticeStatus = (int)Enums.EntryNoticeStatus.UnBoxed,
                                  }
                                  equals new
                                  {
                                      VoyNo = voyageID,
                                      DecHeadID = entryNotice.DecHeadID,
                                      WarehouseType = entryNotice.WarehouseType,
                                      DataStatus = entryNotice.Status,
                                      EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                                  }
                          select new
                          {
                              DecHeadID = decHead.ID,
                              GrossWt = decHead.GrossWt,
                              CusDecStatus = decHead.CusDecStatus,
                          };

            results = results.Where(t => t.CusDecStatus != "04");
            results = from result in results
                      group result by new { result.DecHeadID, result.GrossWt, result.CusDecStatus, } into g
                      select new
                      {
                          DecHeadID = g.Key.DecHeadID,
                          GrossWt = g.Key.GrossWt,
                          CusDecStatus = g.Key.CusDecStatus,
                      };

            return results.Sum(t => t.GrossWt);
        }

        public class BoxInfoListModel
        {
            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// ClientID
            /// </summary>
            public string ClientID { get; set; } = string.Empty;

            /// <summary>
            /// 客户类型
            /// </summary>
            public int ClientType { get; set; }

            /// <summary>
            /// 客户编号
            /// </summary>
            public string ClientCode { get; set; } = string.Empty;

            /// <summary>
            /// 客户名称
            /// </summary>
            public string ClientName { get; set; } = string.Empty;

            /// <summary>
            /// 装箱日期
            /// </summary>
            public DateTime PackingDate { get; set; }

            /// <summary>
            /// 箱号
            /// </summary>
            public string BoxIndex { get; set; } = string.Empty;

            /// <summary>
            /// 库位号
            /// </summary>
            public string StockCode { get; set; } = string.Empty;

            /// <summary>
            /// 是否上架
            /// </summary>
            public bool IsOnStock { get; set; }

            /// <summary>
            /// 报关单 申报状态（回执状态）
            /// </summary>
            public string CusDecStatus { get; set; } = string.Empty;

            public string VoyageID { get; set; } = string.Empty;
        }

        private IEnumerable<BoxInfoListModel> GetTargetPackings(string voyageID)
        {
            var packings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var orders = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();

            var targetPackings = from packing in packings
                                 join order in orders
                                        on new { OrderID = packing.OrderID, PackingStatus = packing.Status, OrderStatus = (int)Enums.Status.Normal, }
                                        equals new { OrderID = order.ID, PackingStatus = (int)Enums.Status.Normal, OrderStatus = order.Status, }
                                 join decHead in decHeads
                                        on new { OrderID = packing.OrderID, VoyNo = voyageID }
                                        equals new { OrderID = decHead.OrderID, VoyNo = decHead.VoyNo }
                                 join entryNotice in entryNotices
                                         on new
                                         {
                                             DecHeadID = decHead.ID,
                                             WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                             DataStatus = (int)Enums.Status.Normal,
                                             EntryNoticeStatus = (int)Enums.EntryNoticeStatus.UnBoxed,
                                         }
                                         equals new
                                         {
                                             DecHeadID = entryNotice.DecHeadID,
                                             WarehouseType = entryNotice.WarehouseType,
                                             DataStatus = entryNotice.Status,
                                             EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                                         }
                                 select new BoxInfoListModel
                                 {
                                     OrderID = packing.OrderID,
                                     ClientID = order.ClientID,
                                     PackingDate = packing.PackingDate,
                                     BoxIndex = packing.BoxIndex,
                                     CusDecStatus = decHead.CusDecStatus,
                                 };

            targetPackings = targetPackings.Where(t => t.CusDecStatus != "04");

            targetPackings = from targetPacking in targetPackings
                             group targetPacking by new { targetPacking.OrderID, targetPacking.ClientID, targetPacking.PackingDate, targetPacking.BoxIndex, } into g
                             select new BoxInfoListModel
                             {
                                 OrderID = g.Key.OrderID,
                                 ClientID = g.Key.ClientID,
                                 PackingDate = g.Key.PackingDate,
                                 BoxIndex = g.Key.BoxIndex,
                             };

            return targetPackings;
        }

        public IEnumerable<BoxInfoListModel> GetBoxInfoListModel(out int totalCount, string voyageID, params LambdaExpression[] expressions)
        {
            var clients = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var orderItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var storeStorages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>();

            var targetPackings = GetTargetPackings(voyageID).AsQueryable();

            var stockCodeInfos = from targetPacking in targetPackings
                                 join orderItem in orderItems
                                         on new { OrderID = targetPacking.OrderID, OrderItemStatus = (int)Enums.Status.Normal, }
                                         equals new { OrderID = orderItem.OrderID, OrderItemStatus = orderItem.Status, }
                                     into orderItems2
                                 from orderItem in orderItems2.DefaultIfEmpty()
                                 join storeStorage in storeStorages
                                         on new
                                         {
                                             BoxIndex = targetPacking.BoxIndex,
                                             OrderItemID = orderItem.ID,
                                             StoreStorageStatus = (int)Enums.Status.Normal,
                                             StoreStoragePurpose = (int)Enums.StockPurpose.Storaged
                                         }
                                         equals new
                                         {
                                             BoxIndex = storeStorage.BoxIndex,
                                             OrderItemID = storeStorage.OrderItemID,
                                             StoreStorageStatus = storeStorage.Status,
                                             StoreStoragePurpose = storeStorage.Purpose,
                                         }
                                         into storeStorages2
                                 from storeStorage in storeStorages2.DefaultIfEmpty()
                                 select new BoxInfoListModel
                                 {
                                     OrderID = targetPacking.OrderID,
                                     BoxIndex = targetPacking.BoxIndex,
                                     StockCode = storeStorage.StockCode,
                                 };

            stockCodeInfos = stockCodeInfos.Where(t => t.StockCode != null);

            stockCodeInfos = from stockCodeInfo in stockCodeInfos
                             group stockCodeInfo by new { stockCodeInfo.OrderID, stockCodeInfo.BoxIndex, stockCodeInfo.StockCode } into g
                             select new BoxInfoListModel
                             {
                                 OrderID = g.Key.OrderID,
                                 BoxIndex = g.Key.BoxIndex,
                                 StockCode = g.Key.StockCode,
                             };


            var result = from targetPacking in targetPackings
                         join stockCodeInfo in stockCodeInfos on new { OrderID = targetPacking.OrderID, BoxIndex = targetPacking.BoxIndex, }
                                equals new { OrderID = stockCodeInfo.OrderID, BoxIndex = stockCodeInfo.BoxIndex, }
                                into stockCodeInfos2
                         from stockCodeInfo in stockCodeInfos2.DefaultIfEmpty()
                         join client in clients on new { ClientID = targetPacking.ClientID, ClientStatus = (int)Enums.Status.Normal, }
                                 equals new { ClientID = client.ID, ClientStatus = client.Status, }
                                 into clients2
                         from client in clients2.DefaultIfEmpty()
                         join company in companies on new { CompanyID = client.CompanyID, CompanyStatus = (int)Enums.Status.Normal, }
                                equals new { CompanyID = company.ID, CompanyStatus = company.Status, }
                                into companies2
                         from company in companies2.DefaultIfEmpty()
                         select new BoxInfoListModel
                         {
                             OrderID = targetPacking.OrderID,
                             ClientID = client.ID,
                             ClientType = (client.ClientType != null) ? (int)client.ClientType : 0,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             PackingDate = targetPacking.PackingDate,
                             BoxIndex = targetPacking.BoxIndex,
                             StockCode = stockCodeInfo.StockCode,
                             IsOnStock = stockCodeInfo.StockCode != null,
                         };

            totalCount = result.Count();

            foreach (var expression in expressions)
            {
                result = result.Where(expression as Expression<Func<BoxInfoListModel, bool>>);
            }

            //var listResult = result.OrderBy(t => t.ClientCode).ThenBy(t => t.OrderID).ThenBy(t => t.PackingDate).ThenBy(t => t.BoxIndex).ToList();
            var listResult = result.OrderBy(t => t.PackingDate).ThenBy(t => t.BoxIndex).ThenBy(t => t.ClientCode).ThenBy(t => t.OrderID).ToList();

            return listResult;
        }

        #endregion

        #region 3. 查出一个箱子中，对应的所有 Sortings

        public class TargetSortingModel
        {
            public string SortingID { get; set; } = string.Empty;

            public string OrderItemID { get; set; } = string.Empty;

            //public string ProductID { get; set; } = string.Empty;

            public decimal Quantity { get; set; }

            public string BoxIndex { get; set; } = string.Empty;

            public string EntryNoticeID { get; set; } = string.Empty;

            public string CusDecStatus { get; set; } = string.Empty;

            public string OrderID { get; set; } = string.Empty;
        }

        public IEnumerable<TargetSortingModel> GetTargetSortingModel(string voyageID, string orderID, string boxIndex)
        {
            var sortings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var entryNoticeItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>();
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var results = from sorting in sortings
                          join entryNoticeItem in entryNoticeItems
                                 on new
                                 {
                                     EntryNoticeItemID = sorting.EntryNoticeItemID,
                                     OrderID = sorting.OrderID,
                                     BoxIndex = sorting.BoxIndex,
                                     SortingStatus = sorting.Status,
                                     WarehouseType = sorting.WarehouseType,
                                     EntryNoticeItemStatus = (int)Enums.Status.Normal,
                                 }
                                 equals new
                                 {
                                     EntryNoticeItemID = entryNoticeItem.ID,
                                     OrderID = orderID,
                                     BoxIndex = boxIndex,
                                     SortingStatus = (int)Enums.Status.Normal,
                                     WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                     EntryNoticeItemStatus = entryNoticeItem.Status,
                                 }
                          join entryNotice in entryNotices
                                on new
                                {
                                    EntryNoticeID = entryNoticeItem.EntryNoticeID,
                                    OrderID = orderID,
                                    EntryNoticeStatus = (int)Enums.Status.Normal,
                                    WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                                }
                                equals new
                                {
                                    EntryNoticeID = entryNotice.ID,
                                    OrderID = entryNotice.OrderID,
                                    EntryNoticeStatus = entryNotice.Status,
                                    WarehouseType = entryNotice.WarehouseType,
                                }
                          join decHead in decHeads
                                  on new
                                  {
                                      DecHeadID = entryNotice.DecHeadID,
                                      OrderID = orderID,
                                      VoyageID = voyageID,
                                  }
                                  equals new
                                  {
                                      DecHeadID = decHead.ID,
                                      OrderID = decHead.OrderID,
                                      VoyageID = decHead.VoyNo
                                  }
                          select new TargetSortingModel
                          {
                              SortingID = sorting.ID,
                              OrderItemID = sorting.OrderItemID,
                              //ProductID = sorting.ProductID,
                              Quantity = sorting.Quantity,
                              BoxIndex = sorting.BoxIndex,
                              EntryNoticeID = entryNotice.ID,
                              CusDecStatus = decHead.CusDecStatus,
                          };

            results = results.Where(t => t.CusDecStatus != "04");
            results = from result in results
                      //group result by new { result.SortingID, result.OrderItemID, result.ProductID, result.Quantity, result.BoxIndex, result.EntryNoticeID, } into g
                      group result by new { result.SortingID, result.OrderItemID, result.Quantity, result.BoxIndex, result.EntryNoticeID, } into g
                      select new TargetSortingModel
                      {
                          SortingID = g.Key.SortingID,
                          OrderItemID = g.Key.OrderItemID,
                          //ProductID = g.Key.ProductID,
                          Quantity = g.Key.Quantity,
                          BoxIndex = g.Key.BoxIndex,
                          EntryNoticeID = g.Key.EntryNoticeID,
                      };

            return results;
        }

        #endregion

        #region 4. 查出一个运输批次对应的所有"深圳入库通知ID"

        public class SZEntryNoticeModel
        {
            public string EntryNoticeID { get; set; } = string.Empty;

            public string CusDecStatus { get; set; } = string.Empty;
        }

        public IEnumerable<SZEntryNoticeModel> GetSZEntryNoticeModel(string voyageID)
        {
            var entryNotices = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>();
            var decHeads = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var results = from entryNotice in entryNotices
                          join decHead in decHeads on new
                          {
                              DecHeadID = entryNotice.DecHeadID,
                              DataStatus = entryNotice.Status,
                              WarehouseType = entryNotice.WarehouseType,
                              EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                              VoyNo = voyageID,
                          }
                          equals new
                          {
                              DecHeadID = decHead.ID,
                              DataStatus = (int)Enums.Status.Normal,
                              WarehouseType = (int)Enums.WarehouseType.ShenZhen,
                              EntryNoticeStatus = (int)Enums.EntryNoticeStatus.UnBoxed,
                              VoyNo = decHead.VoyNo,
                          }
                          select new SZEntryNoticeModel
                          {
                              EntryNoticeID = entryNotice.ID,
                              CusDecStatus = decHead.CusDecStatus,
                          };

            results = results.Where(t => t.CusDecStatus != "04");

            results = from result in results
                      group result by new { result.EntryNoticeID } into g
                      select new SZEntryNoticeModel
                      {
                          EntryNoticeID = g.Key.EntryNoticeID,
                      };

            return results;
        }

        #endregion
    }
}
