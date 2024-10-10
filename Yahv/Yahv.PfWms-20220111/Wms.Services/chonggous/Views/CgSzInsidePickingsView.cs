using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Services.Models;
using Yahv.Utils.Serializers;
using Layers.Data;
using Wms.Services.Models;
using Yahv.Payments;
using Yahv.Utils.Converters.Contents;
using Yahv.Services;
using System.Linq.Expressions;
using Wms.Services.chonggous.Models;
using Wms.Services.Enums;
using Newtonsoft.Json;

namespace Wms.Services.chonggous.Views
{
    /// <summary>
    /// 出库分拣
    /// </summary>
    /// <remarks>
    /// 依据目前的需求做如下提示：
    /// 完成部分出库
    /// 出库通知可以具体到一条库存
    /// 出库通知数量列中显示：应出，已出，剩余
    /// 应出：出库通知数量，X
    /// 已出：建议显示已经分拣数量，Y
    /// 剩余：X-Y
    /// 
    /// 平经理要求：出库也要在出库单上实现分批出库，说了很多理由这里就不重复。
    /// </remarks>
    public class CgSzInsidePickingsView : QueryView<object, PvWmsRepository>
    {
        /// <summary>
        /// 固定跟单员
        /// </summary>
        public const string FixedTrackerID = "Admin00548";

        #region 构造器
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CgSzInsidePickingsView()
        {
        }

        /// <summary>
        /// 有参构造函数，外部调用使用
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        protected CgSzInsidePickingsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 有参构造函数，条件查询使用
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        /// <param name="iQueryable">查询结果集</param>
        protected CgSzInsidePickingsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        #endregion

        /// <summary>
        /// 列表集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<object> GetIQueryable()
        {
            var clientView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>();
            var carriersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>();
            //var sources = new int[] { (int)CgNoticeSource.AgentSend, (int)CgNoticeSource.Transfer, (int)CgNoticeSource.AgentCustomsFromStorage, (int)CgNoticeSource.AgentBreakCustomsForIns, (int)CgNoticeSource.AgentBreakCustoms }.ToList();
            var sources = new int[] { (int)CgNoticeSource.AgentSend, (int)CgNoticeSource.Transfer, (int)CgNoticeSource.AgentCustomsFromStorage }.ToList();
            var wsnSuppliersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.wsnSuppliersTopView>();
            

            var linqs = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                        join _client in clientView on waybill.wbEnterCode equals _client.EnterCode into clients
                        from client in clients.DefaultIfEmpty()
                        join _carrier in carriersTopView on waybill.wbCarrierID equals _carrier.ID into carriers
                        from carrier in carriers.DefaultIfEmpty()
                        join _supplier in wsnSuppliersTopView on new { EnterCode = waybill.wbEnterCode, SupplierName = waybill.wbSupplier } equals new { EnterCode = _supplier.EnterCode, SupplierName = _supplier.RealEnterpriseName } into Suppliers
                        from supplier in Suppliers.DefaultIfEmpty()                        
                            //可能理解错误，陈翰
                            //where (waybill.NoticeType == (int)CgNoticeType.Out && (waybill.Source == (int)CgNoticeSource.Transfer || waybill.Source == (int)CgNoticeSource.AgentSend))
                            //  || (waybill.NoticeType == (int)CgNoticeType.Boxing && waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage)

                            //(waybill.NoticeType == (int)CgNoticeType.Out && (waybill.Source == (int)CgNoticeSource.Transfer || waybill.Source == (int)CgNoticeSource.AgentSend))
                            //  || (waybill.NoticeType == (int)CgNoticeType.Boxing && waybill.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
                            //|| waybill.NoticeType == (int)CgNoticeType.Boxing) && sources.Contains((int)waybill.Source)
                            //按照现在的要求：装箱通知也要开发在出库中因此，要增加如下逻辑

                            // 视图 会与其他 视图进行关联， 原有排序是无用的
                            //orderby waybill.wbType
                        orderby waybill.wbCreateDate descending
                        select new MyWaybill
                        {
                            ID = waybill.wbID,
                            CreateDate = waybill.wbCreateDate,
                            EnterCode = waybill.wbEnterCode,
                            Supplier = waybill.wbSupplier,
                            SupplierName = supplier.ChineseName,
                            SupplierGrade = supplier.nGrade,
                            Packaging = waybill.wbPackaging,
                            ExcuteStatus = (CgPickingExcuteStatus)waybill.wbExcuteStatus,
                            Type = (WaybillType)waybill.wbType,
                            Code = waybill.wbCode,
                            CarrierID = waybill.wbCarrierID,
                            CarrierName = carrier != null ? carrier.Name : null,
                            ConsignorID = waybill.wbConsignorID,
                            Place = waybill.corPlace,
                            ConsignorPlace = waybill.corPlace,
                            OrderID = waybill.OrderID,
                            Source = (CgNoticeSource)waybill.Source,
                            NoticeType = (CgNoticeType)waybill.NoticeType,
                            AppointTime = waybill.AppointTime,
                            TransferID = waybill.wbTransferID,
                            wldDriver = waybill.wldDriver,
                            wldTakingPhone = waybill.wldTakingPhone,
                            chcdDriver = waybill.chcdDriver,
                            chcdCarNumber1 = waybill.chcdCarNumber1,
                            chcdCarNumber2 = waybill.chcdCarNumber2,
                            wldCarNumber1 = waybill.wldCarNumber1,
                            LoadingExcuteStatus = (CgLoadingExcuteStauts?)waybill.loadExcuteStatus,

                            CoeAddress = waybill.coeAddress,
                            TakingContact = waybill.corContact,
                            IDNumber = waybill.coeIDNumber,
                            IDType = (IDType?)waybill.coeIDType,
                            TakingPhone = waybill.corPhone,

                            Extype = waybill.ExType,
                            ExPayType = waybill.ExPayType,

                            chgTotalPrice = waybill.chgTotalPrice,
                            ClientName = client == null ? null : client.Name,
                            coeContact = waybill.coeContact,
                            coePhone = waybill.coePhone,
                            corContact = waybill.corContact,
                            corCompany = waybill.corCompany,
                            corPhone = waybill.corPhone,
                            corAddress = waybill.corAddress,
                            TotalWeight = waybill.wbTotalWeight,//总重量
                            TotalParts = waybill.wbTotalParts,
                            Condition = waybill.wbCondition,
                            LotNumber = waybill.chcdLotNumber, //运输批次                            

                        };
            return linqs;
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetPagelistData(int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.IQueryable.Cast<MyWaybill>();
            int total = iquery.Count();
            //执行sql语句
            var waybills = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).OrderByDescending(x => x.CreateDate).ToArray();
            var ordersId = waybills.Select(item => item.OrderID).Distinct();

            var merchandiserTopView = from merchandiser in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderMerchandiserTopView>()
                                      where ordersId.Contains(merchandiser.OrderID)
                                      select new
                                      {
                                          merchandiser.OrderID,
                                          merchandiser.RealName,
                                      };

            var trackers = merchandiserTopView.Distinct().ToArray();

            var result = from waybill in waybills
                             //join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.ID equals notice.WaybillID into notices
                             //from notice in notices.DefaultIfEmpty()
                             //join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID into  g
                             //from  output in g.DefaultIfEmpty()
                         select new
                         {

                             ID = waybill.ID,
                             CreateDate = waybill.CreateDate,
                             EnterCode = waybill.EnterCode,
                             ClientName = waybill.ClientName,
                             Supplier = waybill.Supplier,
                             waybill.SupplierName,
                             waybill.SupplierGrade,
                             Packaging = waybill.Packaging == null ? "" : ((Package)(int.Parse(waybill.Packaging))).GetDescription(),
                             ExcuteStatus = waybill.ExcuteStatus,
                             ExcuteStatusDescription = waybill.ExcuteStatus.GetDescription(),
                             Type = waybill.Type,
                             WaybillTypeDescription = waybill.Type.GetDescription(),
                             Code = waybill.Code,
                             CarrierID = waybill.CarrierID,
                             CarrierName = waybill.CarrierName,
                             ConsignorID = waybill.ConsignorID,
                             Place = waybill.Place,
                             ConsignorPlace = waybill.ConsignorPlace,
                             OrderID = waybill.OrderID,
                             Source = waybill.Source,
                             SourceDescription = waybill.Source.GetDescription(),
                             NoticeType = waybill.NoticeType,
                             AppointTime = waybill.AppointTime,
                             TransferID = waybill.TransferID,
                             //Driver = waybill.Driver,
                             //CarNumber1 = waybill.CarNumber1,
                             LoadingExcuteStatus = waybill.LoadingExcuteStatus,
                             LotNumber = waybill.LotNumber,
                             Merchandiser = trackers.SingleOrDefault(item => item.OrderID == waybill.OrderID)?.RealName
                         };

            var results = result.ToArray();
            

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = results,
            };
        }

        [Obsolete("思路废弃")]
        Admin GetTracker()
        {
            //var client = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory)
            //    .FirstOrDefault(x => x.EnterCode == waybill.EnterCode);
            //string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            //var RealID = string.Join("", companyid, client?.ID).MD5();
            //var map = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.MapsTrackerTopView>().FirstOrDefault(x => x.RealID == RealID && x.Type == 76);
            //var traker = map == null ? null :
            //    new Yahv.Services.Views.AdminsAll<PvWmsRepository>(this.Reponsitory).FirstOrDefault(x => x.ID == map.AdminID);

            //return traker;

            return null;
        }

        /// <summary>
        /// 获取详情通知数据
        /// </summary>
        /// <param name="WaybillID"></param>
        /// <returns></returns>
        public object GetDetail(string WaybillID)
        {
            var waybill = this.IQueryable.Cast<MyWaybill>().SingleOrDefault(item => item.ID == WaybillID);
            var productview = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);
            var noticeView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybill.ID);
            var orderItem = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderItemsTopView>().Where(item => item.OrderID == waybill.OrderID).ToArray();// 取报关总货值
            var totalPriceInstance = orderItem.FirstOrDefault();
            var orderInfo = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsOrdersTopView>().SingleOrDefault(item => item.ID == waybill.OrderID);
            var creditView = orderInfo == null ? null : this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CreditsStatisticsView>().Where(item => item.Currency == 1 && item.Payer == orderInfo.ClientID && item.Payee == orderInfo.PayeeID);
            var isCustoms = waybill.Source == CgNoticeSource.AgentBreakCustoms || waybill.Source == CgNoticeSource.AgentBreakCustomsForIns || waybill.Source == CgNoticeSource.AgentCustomsFromStorage;
            var business = isCustoms ? ConductConsts.代报关 : ConductConsts.代仓储;
            var overDue = orderInfo == null ? true : PaymentManager.Npc[orderInfo.ClientID, orderInfo.PayeeID][business].DebtTerm[DateTime.Now].IsOverdue;
            var wareHouseID = noticeView.Select(item => item.WareHouseID).Distinct().ToArray().FirstOrDefault();

            ///查询跟单信息

            ///只显示已上架的
            var storageView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>();

            #region 通知数据

            //获取通知的数据和已拣货数据
            var noticesView = from notice in noticeView
                              join storage in storageView on notice.StorageID equals storage.ID
                              //join product in productview on notice.ProductID equals product.ID
                              join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                              select new
                              {
                                  #region 视图

                                  notice.ID,
                                  notice.WaybillID,
                                  notice.OutputID,
                                  notice.DateCode,
                                  notice.Quantity,
                                  Origin = notice.Origin,
                                  notice.Conditions,
                                  Source = (NoticeSource)notice.Source,
                                  Type = (CgNoticeType)notice.Type,
                                  notice.Weight,
                                  notice.NetWeight,
                                  notice.Volume,
                                  storage.ShelveID,
                                  notice.Summary,
                                  notice.CreateDate,//制单时间
                                  notice.StorageID,
                                  notice.InputID,
                                  notice.BoxCode,
                                  notice.CustomsName,
                                  //Product = new
                                  //{
                                  //    product.PartNumber,
                                  //    product.Manufacturer,
                                  //},
                                  Output = new
                                  {
                                      output.ID,
                                      output.OrderID,
                                      output.TinyOrderID,
                                      output.ItemID,
                                      output.Price,
                                      output.Currency,
                                      output.TrackerID
                                  },

                                  #endregion
                              };

            var ienums_notice = noticesView.ToArray();
            //已拣货数据 
            //理论上能够出库的只能是流水库的数据
            var pickingview = from notice in noticeView
                              join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID
                              select picking;
            var ienum_pickings = pickingview.ToArray();

            var ienum_notices = from notice in ienums_notice
                                join picking in ienum_pickings on notice.ID equals picking.NoticeID into pickings
                                select new
                                {
                                    #region 视图

                                    notice.ID,
                                    notice.WaybillID,
                                    notice.OutputID,
                                    notice.DateCode,
                                    notice.Quantity,
                                    Origin = notice.Origin,
                                    notice.Source,
                                    notice.CreateDate,
                                    notice.Type,
                                    notice.Weight,
                                    notice.NetWeight,
                                    notice.Volume,
                                    notice.ShelveID,
                                    notice.Summary,
                                    //notice.Product,
                                    notice.CustomsName,
                                    Output = notice.Output,
                                    Currency = notice.Output.Currency,

                                    notice.StorageID,
                                    notice.InputID,
                                    notice.BoxCode,
                                    Pickings = pickings.Select(item => new
                                    {
                                        item.ID,
                                        item.StorageID,
                                        item.NoticeID,
                                        item.OutputID,
                                        item.BoxCode,
                                        item.Quantity,
                                        item.AdminID,
                                        item.Weight,
                                        item.NetWeight,
                                        item.Volume,
                                        item.Summary,
                                        item.CreateDate,
                                    }).ToArray(),
                                    PickedQuantity = pickings.Sum(s => (decimal?)s.Quantity) ?? 0m,
                                    LeftQuantity = notice.Quantity - pickings.Sum(s => (decimal?)s.Quantity) ?? 0m,
                                    CurrentQuantity = notice.Quantity - pickings.Sum(s => (decimal?)s.Quantity) ?? 0m,
                                    Conditions = notice.Conditions,
                                    totalPrice = notice.Quantity * notice.Output.Price
                                    #endregion
                                };
            #endregion


            var totalPrice = ienum_notices.Sum(x => x.totalPrice);
            var totalPriceCurrency = ienum_notices.Select(x => x.Currency).ToArray().Distinct();

            var storagesID = ienum_notices.Select(item => item.StorageID).Where(item => item != null).Distinct();
            var suppliers = from notice in noticeView
                            join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on notice.StorageID equals storage.ID
                            join _supplier in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.wsnSuppliersTopView>() on storage.Supplier equals _supplier.RealEnterpriseName into supplierWithGrade
                            from supplier in supplierWithGrade.DefaultIfEmpty()
                            where supplier.EnterCode == waybill.EnterCode
                            select new
                            {
                                storage.Supplier,
                                supplier.ChineseName,
                                supplier.nGrade
                            };
            var ienum_suppliers = suppliers.ToArray();
            var group_suppliers = from item in ienum_suppliers
                                  group item by item.Supplier into items
                                  select new
                                  {
                                      Supplier = items.Key,
                                      SupplierName = items.FirstOrDefault().ChineseName,
                                      SupplierGrade = items.FirstOrDefault().nGrade
                                  };

            var adminView = new Yahv.Services.Views.AdminsAll<PvWmsRepository>(this.Reponsitory);
            var trackerID = ienums_notice.Select(item => item.Output.TrackerID).Where(item => item != null).FirstOrDefault();
            var tracker = adminView.SingleOrDefault(item => item.ID == trackerID);

            #region 获取随货文件
            var files = (from file in new Yahv.Services.Views.CenterFilesTopView()
                         where waybill.ID == file.WaybillID
                              //|| ienums_notice.Contains(file.NoticeID)
                              || file.WsOrderID == waybill.OrderID
                         //|| storagesID.Contains(file.StorageID)
                         //|| inputsID.Contains(file.InputID)
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
                         }).ToArray();

            //随货文件
            var file_followgoods = files.Where(item => item.Type == (int)FileType.FollowGoods).ToArray();
            //发货文件
            var file_delivergood = files.Where(item => item.Type == (int)FileType.DeliverGoods && item.WaybillID == WaybillID).ToArray();
            //标签文件
            var file_Label = files.Where(item => item.Type == (int)FileType.Label).ToArray();

            var file_Imagefile = files.Where(item => item.Type == (int)FileType.StoragesPic).ToArray();
            var file_sendGoods = files.Where(item => item.Type == (int)FileType.SendGoods && item.WaybillID == WaybillID).ToArray();

            #endregion


            var Waybill = new
            {
                waybill.ID,
                waybill.CreateDate,
                waybill.EnterCode,
                waybill.ClientName,
                Supplier_bak = waybill.Supplier,
                Supplier = group_suppliers.Select(item => new
                {
                    item.Supplier,
                    item.SupplierName,
                    item.SupplierGrade,
                }),
                Packaging = waybill.Packaging == null ? "" : ((Package)(int.Parse(waybill.Packaging))).GetDescription(),
                waybill.ExcuteStatus,
                waybill.Type,
                waybill.Code,
                waybill.CarrierID,
                waybill.CarrierName,
                Driver = (wareHouseID.StartsWith("HK") && waybill.NoticeType == CgNoticeType.Out && waybill.Source == CgNoticeSource.AgentBreakCustoms || waybill.Source == CgNoticeSource.AgentCustomsFromStorage) ? waybill.chcdDriver : waybill.wldDriver,
                wldTakingPhone = waybill.wldTakingPhone,
                waybill.ConsignorID,
                waybill.ConsignorPlace,
                waybill.Source,
                waybill.NoticeType,
                CarNumberID = (wareHouseID.StartsWith("HK") && waybill.NoticeType == CgNoticeType.Out && waybill.Source == CgNoticeSource.AgentBreakCustoms || waybill.Source == CgNoticeSource.AgentCustomsFromStorage) ? (waybill.chcdCarNumber1 + " " + waybill.chcdCarNumber2) : waybill.wldCarNumber1,
                waybill.AppointTime,
                waybill.TransferID,
                waybill.LoadingExcuteStatus,
                waybill.OrderID,
                ConsignorPlaceName = (Origin)Enum.Parse(typeof(Origin), string.IsNullOrEmpty(waybill.Place) ? nameof(Origin.Unknown) : waybill.Place),
                ConsignorPlaceID = ((int)Enum.Parse(typeof(Origin), string.IsNullOrEmpty(waybill.Place) ? nameof(Origin.Unknown) : waybill.Place)).ToString(),
                ConsignorPlaceText = waybill.ConsignorPlace + " " + ((Origin)Enum.Parse(typeof(Origin), string.IsNullOrEmpty(waybill.Place) ? nameof(Origin.Unknown) : waybill.Place)).GetDescription(),
                TypeName = waybill.Type.GetDescription(),
                ExcuteStatusName = waybill.ExcuteStatus.GetDescription(),
                Files = file_followgoods,
                FeliverGoodFile = file_delivergood,
                LableFile = file_Label,
                SendGoodsFile = file_sendGoods,//送货文件
                waybill.IDNumber,
                waybill.IDType,
                IDTypeName = string.IsNullOrEmpty(waybill.IDType.ToString()) ? "" : ((IDType)Enum.Parse(typeof(IDType), waybill.IDType.ToString())).GetDescription(),
                SourceName = ((CgNoticeSource)Enum.Parse(typeof(CgNoticeSource), waybill.Source.ToString())).GetDescription(),
                waybill.TakingContact,
                waybill.TakingPhone,
                waybill.ExPayType,
                waybill.Extype,
                chgTotalPrice = totalPrice,
                chgTotalCurrency = ((Yahv.Underly.Currency)(totalPriceCurrency.Count() == 0 ? 0 : totalPriceCurrency.First())).GetCurrency().ShortName,
                waybill.coePhone,
                waybill.coeContact,
                waybill.CoeAddress,
                waybill.corContact,
                waybill.corPhone,
                waybill.LotNumber,
                waybill.Merchandiser,
                total = creditView == null ? 0 : creditView.Where(item => item.Business == business).ToArray()?.Sum(item => item.Total),
                totalDebt = creditView == null ? 0 : creditView.Where(item => item.Business == business).ToArray()?.Sum(item => item.Cost),
                overDue,
                Condition = waybill.Condition.JsonTo<WayCondition>(),
                //跟单信息
                Tracker = new
                {
                    tracker?.ID,
                    tracker?.RealName,
                    tracker?.SelCode
                },
                //发件人
                Sender = new
                {
                    Company = waybill.corCompany,
                    Name = waybill.corContact,
                    Mobile = waybill.corPhone,
                    Address = waybill.corAddress
                },
                //收件人
                Receiver = new
                {
                    Company = waybill.ClientName,
                    Mobile = waybill.coePhone,
                    Name = waybill.coeContact,
                    Address = waybill.CoeAddress
                },
                TotalParts = ienum_notices.Select(item => item.BoxCode).Distinct().ToArray().Count(),//总件数
                waybill.TotalWeight, //总重量
                TinyOrderIDCount = ienum_notices.Select(notice => notice.Output.TinyOrderID).Distinct().ToArray().Count(),
                BoxingDate = ienums_notice.First().CreateDate, //.ToShortDateString(),

            };

            return new
            {
                Waybill = Waybill,
                Notices = ienum_notices.Select(notice => new
                {
                    ShelveID = notice.ShelveID?.Substring(3),
                    //notice.CustomsName,
                    BoxCode = notice.BoxCode, //乔霞的硬要求
                    TinyOrderID = notice.Output.TinyOrderID,  //乔霞的硬要求
                    OrderID = notice.Output.OrderID,
                }).Distinct().ToArray().OrderBy(item => item.OrderID),
            };

        }

        /// <summary>
        /// 获取详情通知数据
        /// </summary>
        /// <param name="WaybillID"></param>
        /// <returns></returns>
        public object GetSzPrintData(string WaybillID)
        {
            var waybill = this.IQueryable.Cast<MyWaybill>().SingleOrDefault(item => item.ID == WaybillID);
            var productview = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);
            var noticeView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybill.ID);

            var wareHouseID = noticeView.Select(item => item.WareHouseID).Distinct().ToArray().FirstOrDefault();

            var storageView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>();
            #region 通知数据

            //获取通知的数据和已拣货数据
            var noticesView = from notice in noticeView
                              join storage in storageView on notice.StorageID equals storage.ID
                              join product in productview on notice.ProductID equals product.ID
                              join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                              select new
                              {
                                  #region 视图

                                  notice.ID,
                                  notice.WaybillID,
                                  notice.Quantity,
                                  notice.Conditions,
                                  Source = (NoticeSource)notice.Source,
                                  Type = (CgNoticeType)notice.Type,
                                  storage.ShelveID,
                                  notice.Summary,
                                  notice.CreateDate,//制单时间
                                  notice.StorageID,
                                  notice.BoxCode,
                                  notice.CustomsName,
                                  product.PartNumber,
                                  product.Manufacturer,
                                  output.OrderID,
                                  output.TinyOrderID,

                                  #endregion

                              };

            var ienum_notices = noticesView.ToArray();

            var linqByTinyOrderID = from notice in ienum_notices
                                group notice by notice.TinyOrderID into notices
                                select new
                                {
                                    TinyOrderID = notices.Key,
                                    TinyOrderTotalPart = notices.Select(item => item.BoxCode).Distinct().ToArray().Count(),
                                    notices = notices.Select(item => new
                                    {
                                        item.ID,
                                        item.WaybillID,
                                        item.Quantity,
                                        item.Conditions, //= item.Conditions.JsonTo<NoticeCondition>(),
                                        item.ShelveID,
                                        item.Summary,
                                        item.CreateDate,
                                        item.BoxCode,
                                        item.CustomsName,
                                        item.PartNumber,
                                        item.Manufacturer,
                                        item.OrderID
                                    }),
                                };

            var ienum_linqByTinyOrderID = linqByTinyOrderID.ToArray();

            var linqByBoxCode = ienum_linqByTinyOrderID.Select(item => new
            {
                item.TinyOrderID,
                item.TinyOrderTotalPart,
                notices = item.notices.Select(notice => new
                {
                    notice.ID,
                    notice.WaybillID,
                    notice.Quantity,
                    IsCCC = notice.Conditions.JsonTo<NoticeCondition>().IsCCC,
                    IsCIQ = notice.Conditions.JsonTo<NoticeCondition>().IsCIQ,
                    IsHighPrice = notice.Conditions.JsonTo<NoticeCondition>().IsHighPrice,
                    notice.BoxCode,
                    notice.CustomsName,
                    ShelveID = notice.ShelveID?.Substring(3),
                    notice.PartNumber,
                    notice.Manufacturer,
                    notice.OrderID,
                    notice.Summary,
                    notice.CreateDate,
                }).OrderBy(notice => notice.ShelveID).ThenBy(notice => notice.BoxCode)
            });
            #endregion

            return linqByBoxCode.ToArray();

        }

        #region 拣货出库操作
        /// <summary>
        /// 拣货完成
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>
        /// 捡货完成，这里其实等待出库
        /// </remarks>
        public void Completed(string waybillID)
        {
            //可以调用部分拣货的方法
            //目前出库就是出库，点击完成后，只是判断状态。
            //如果是全都出库了，就修改状态：CgLoadingExcuteStauts
            //扣除库存,原则上能出库的一定是流水库

            //深圳报关内单出库任务化后，出库完成点击操作只是更新订单的执行状态
            var result = GetDetail(waybillID).Json();

            var newNotices = JsonConvert.DeserializeObject<InsNotices>(result);
            var waybill = this.IQueryable.Cast<MyWaybill>().SingleOrDefault(item => item.ID == waybillID);

            var isShelveIDNull = newNotices.Notices.Any(item => string.IsNullOrEmpty(item.ShelveID));

            if (isShelveIDNull)
            {
                throw new Exception("请完成上架操作后再进行内单出库操作!");
            }

            using (var reponsitory = new PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)CgPickingExcuteStatus.Completed
                }, item => item.ID == waybillID);

                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.TasksPool
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = "深圳报关内单出库",
                    MainID = waybillID,
                    CreateDate = DateTime.Now,
                });

                var tinyOrderIDs = newNotices.Notices.Select(item => item.TinyOrderID).Distinct().ToArray();

                //foreach (var tinyOrderID in tinyOrderIDs)
                //{ 
                //    PushMsg pushMsg = new PushMsg(1, (int)SpotName.DSZSend, tinyOrderID, "", waybill.wldDriver, waybill.wldTakingPhone);
                //    pushMsg.push();
                //}
            }
        }

        /// <summary>
        /// 深圳报关内单出库，任务操作内容
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>
        /// </remarks>
        public void SZCustomInsExit(string waybillID)
        {
            //可以调用部分拣货的方法
            //目前出库就是出库，点击完成后，只是判断状态。
            //如果是全都出库了，就修改状态：CgLoadingExcuteStauts
            //扣除库存,原则上能出库的一定是流水库
            string[] ordersID;
            List<CgLogs_Operator> logsOperatorList = new List<CgLogs_Operator>();

            using (var repository = new PvWmsRepository())
            using (var tran = repository.OpenTransaction())
            {

                var noticeView = repository.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillID);
                var storagesView = repository.ReadTable<Layers.Data.Sqls.PvWms.Storages>();
                var sortingsView = repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>();
                var outputsView = repository.ReadTable<Layers.Data.Sqls.PvWms.Outputs>();
                var linqs_picking = from notice in noticeView
                                    join storage in storagesView on notice.StorageID equals storage.ID
                                    join sorting in sortingsView on storage.SortingID equals sorting.ID
                                    join output in outputsView on notice.OutputID equals output.ID
                                    select new
                                    {
                                        //ID = null,
                                        //AdminID = null,
                                        BoxCode = sorting.BoxCode,
                                        CreateDate = DateTime.Now,
                                        NetWeight = sorting.NetWeight,
                                        NoticeID = notice.ID,
                                        OutputID = notice.OutputID,
                                        Quantity = notice.Quantity,
                                        StorageID = notice.StorageID,
                                        Summary = notice.Summary,
                                        Volume = sorting.Volume,
                                        Weight = sorting.Weight,
                                        StorageQuantity = storage.Quantity,
                                        output.OrderID
                                    };


                var ienums_notice = linqs_picking.ToArray();
                ordersID = ienums_notice.Select(item => item.OrderID).Distinct().ToArray();

                if (ienums_notice.Any(item => item.Quantity > item.StorageQuantity))
                {
                    throw new Exception("通知数量>库存数量，WaybillID:" + waybillID);
                }

                foreach (var notice in ienums_notice)
                {
                    repository.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    {
                        Quantity = notice.StorageQuantity - notice.Quantity

                    }, item => item.ID == notice.StorageID);
                }

                tran.Commit();
            }

            var notices = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillID)
                          join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
                          select new
                          {
                              notice.Origin,
                              product.PartNumber,
                              product.Manufacturer,
                              notice.Quantity,
                          };

            var realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == FixedTrackerID).RealName;
            foreach (var notice in notices.ToArray())
            {
                CgLogs_Operator logsOperator = new CgLogs_Operator();
                logsOperator.MainID = waybillID;
                logsOperator.Type = LogOperatorType.Insert;
                logsOperator.Conduct = "拣货";
                logsOperator.CreatorID = FixedTrackerID;
                logsOperator.CreateDate = DateTime.Now;
                logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 拣货, 型号: {notice.PartNumber}, 品牌: {notice.Manufacturer}, 产地: {notice.Origin} 数量: {notice.Quantity}";
                logsOperatorList.Add(logsOperator);
            }

            if (logsOperatorList.Count() > 0)
            {
                foreach (var log in logsOperatorList)
                {
                    log.Enter(this.Reponsitory);
                }
            }

            using (var reponsitory = new PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false,//建议询问小辉
                }, item => item.Type == (int)Yahv.Underly.Enums.WaybillStatusType.ExecutionStatus && item.MainID == waybillID);

                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = waybillID,
                    Type = (int)Yahv.Underly.Enums.WaybillStatusType.ExecutionStatus,//建议询问小辉
                    Status = (int)CgPickingExcuteStatus.Completed,//建议询问小辉
                    CreateDate = DateTime.Now,
                    CreatorID = FixedTrackerID,
                    IsCurrent = true,
                });

                /* 根据新要求, 代报关,转报关, 代报关内单的已发货状态已经不再使用
                foreach (var orderid in ordersID)
                {
                    //保存订单日志, 非代报关业务订单的状态为已发货
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == orderid);

                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderid,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.已发货,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = FixedTrackerID,
                        IsCurrent = true,
                    });
                }
                */
            }


            // 深圳库房出库成功后调用华芯通的接口,判断一下如果是深圳出库 就调用

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(FromType.XdtSZShiped.GetDescription(), new SZToXDT
            {
                AdminID = FixedTrackerID,
                OrdersID = ordersID,
                WaybillID = waybillID
            });
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// 更新运单的累加状态
        /// </summary>
        /// <param name="waybillid"></param>
        /// <param name="excuteStatus"></param>
        /// <param name="adminID"></param>
        public void UpdateLogWaybillStatus(string waybillid, CgPickingExcuteStatus excuteStatus, string adminID)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                #region 运单状态日志记录，订单状态日志记录
                pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false,//建议询问小辉
                }, item => item.Type == (int)Yahv.Underly.Enums.WaybillStatusType.ExecutionStatus && item.MainID == waybillid);

                pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = waybillid,
                    Type = (int)Yahv.Underly.Enums.WaybillStatusType.ExecutionStatus,//建议询问小辉
                    Status = (int)excuteStatus,//建议询问小辉
                    CreateDate = DateTime.Now,
                    CreatorID = adminID,
                    IsCurrent = true,
                });
                #endregion
            }
        }

        /// <summary>
        /// 更新订单主状态
        /// </summary>
        /// <param name="source"></param>
        /// <param name="waybillorderid"></param>
        /// <param name="adminID"></param>
        public void UpdateOrderStatus(CgNoticeSource noticeSource, CgNoticeType noticeType, string waybillorderid, string adminID)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                if (noticeType == CgNoticeType.Out)
                {
                    //保存订单日志, 非代报关业务订单的状态为已发货
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == waybillorderid);

                    var orderID = waybillorderid;
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.客户待收货,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });

                    if (noticeSource == CgNoticeSource.AgentSend || noticeSource == CgNoticeSource.Transfer)
                    {
                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, item => item.Type == (int)OrderStatusType.PaymentStatus && item.MainID == waybillorderid);


                        pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = orderID,
                            Type = (int)OrderStatusType.PaymentStatus, //订单支付状态，  
                            Status = (int)OrderPaymentStatus.Confirm,//建议询问小辉
                            CreateDate = DateTime.Now,
                            CreatorID = adminID,
                            IsCurrent = true,
                        });
                    }
                }
                else if (noticeType == CgNoticeType.Boxing)
                {
                    //保存订单日志, 非代报关业务订单的状态为已收货
                    pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == waybillorderid);

                    var orderID = waybillorderid;
                    pvcenterReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = orderID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.待报关,
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });
                }
            }
        }

        /// <summary>
        /// 获取订单的支付状态
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public OrderPaymentStatus GetOrderStatus(string orderID)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                var status = pvcenterReponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>().Single(item => item.Type == (int)OrderStatusType.PaymentStatus && item.MainID == orderID && item.IsCurrent == true).Status;
                return (OrderPaymentStatus)status;
            }
        }

        #endregion

        #region 出库流程讨论注释
        // 出库通知发出的过程，如果使用非 forms 方式开发，
        // 那么出库通知 就是 lock  +  notcie 的过程
        // 50个AD620 放在一个包装中（supplier标签）
        // 出库捡货 只出5个
        // 一定从原包装中拆解出来5放到一个小包装中，就相当与流水库库存(逻辑上的认为流水库生成的新数据就是要锁定的数据)
        // 如果在出库前的任意时刻取消 出库 Notice ，把流水库中的货物合并会 原有包装是可做的
        // 流水库提供合并用能，如果点击(自动化处理取消订单)：把流水库中删除并吧库存加入回到 库存库(原有库存中)中，inputID 要一致，Output(此时是否能够得知销项信息？)
        // 如果无法合并 ，处理也是唯一的：更正storage.Type  为  库存库 inputID 要一致

        // picking 的生成需要去流水库中去对应要出库数据

        //库存分别存在 3个地方，每个地方的数量分别为：1，3，6
        //Notcie 同一input  发出两条数量分别为 ： 4、5 对包装有要求的，这样的通知是不能具体到库位 与库存上的。
        //出库条件唯独不能有对包装内数量的要求，这个要做一定以通知的方式做。让实际库房人员在库存中凑货

        //如果通知包涵：storageID 等具体定为信息，就需要库房人员到具体位置进行拣货。这样做的唯一的优势是：不用做拣货指导的相关功能
        #endregion

        /// <summary>
        /// 出库异常
        /// </summary>
        public void Excepted(string waybillid, string adminid, string orderID, string summry)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)CgPickingExcuteStatus.Anomalous,
                    Summary = summry,
                    ModifyDate = DateTime.Now,

                }, item => item.ID == waybillid);

                #region 运单状态日志记录
                UpdateLogWaybillStatus(waybillid, CgPickingExcuteStatus.Anomalous, adminid);
                #endregion
            }
        }

        #region Helper Class
        /// <summary>
        /// 符合Picking视图头部定义的内部类
        /// </summary>
        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime CreateDate { get; set; }
            public string EnterCode { get; set; }
            public string ClientName { get; set; }
            public string Supplier { get; set; }
            public string SupplierName { get; set; }
            public int? SupplierGrade { get; set; }
            public CgPickingExcuteStatus ExcuteStatus { get; set; }
            public WaybillType Type { get; set; }
            public string Code { get; set; }
            public string CarrierID { get; set; }
            public string CarrierName { get; set; }
            public string ConsignorID { get; set; }
            public string ConsignorPlace { get; set; }
            public string Place { get; set; }
            public string OrderID { get; set; }
            public string Packaging { get; set; }
            public CgNoticeSource Source { get; set; }
            /// <summary>
            /// 业务类型名称
            /// </summary>
            public string SourceName { get; set; }
            public CgNoticeType NoticeType { get; set; }
            public DateTime? AppointTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 转运使用
            /// </remarks>
            public string TransferID { get; set; }
            public string Driver { get; set; }
            /// <summary>
            /// 中港报关司机
            /// </summary>
            public string chcdDriver { get; set; }
            /// <summary>
            /// 具体承运商司机
            /// </summary>
            public string wldDriver { get; set; }

            public string wldTakingPhone { get; set; }
            
            public string CarNumberID { get; set; }
            public string CarNumber1 { get; set; }

            /// <summary>
            /// 中港报关车牌1
            /// </summary>
            public string chcdCarNumber1 { get; set; }
            /// <summary>
            /// 中港报关车牌2
            /// </summary>
            public string chcdCarNumber2 { get; set; }

            /// <summary>
            /// 具体承运商车牌
            /// </summary>
            public string wldCarNumber1 { get; set; }

            public CgLoadingExcuteStauts? LoadingExcuteStatus { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 经过商议暂时增加，等与库房前端对通后，我们可以没有这个传递与处理了。
            /// </remarks>
            public string[] Files { get; set; }

            /// <summary>
            /// 收件地址
            /// </summary>
            public string CoeAddress { get; set; }
            /// <summary>
            /// 提货证件
            /// </summary>
            public string IDNumber { get; set; }
            /// <summary>
            /// 证件类型
            /// </summary>
            public IDType? IDType { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string IDTypeName { get; set; }
            /// <summary>
            /// 提货联系人
            /// </summary>
            public string TakingContact { get; set; }
            /// <summary>
            /// 联系电话
            /// </summary>
            public string TakingPhone { get; set; }

            /// <summary>
            /// 快递类型
            /// </summary>
            public int? Extype { get; set; }
            /// <summary>
            /// 快递支付方式
            /// </summary>

            public int? ExPayType { get; set; }
            /// <summary>
            /// 总货值
            /// </summary>

            public decimal? chgTotalPrice { get; set; }

            /// <summary>
            /// 收货人姓名
            /// </summary>
            public string coeContact { get; set; }
            /// <summary>
            /// 收货电话
            /// </summary>

            public string coePhone { get; set; }

            /// <summary>
            /// 送货人
            /// </summary>
            public string corContact { get; set; }
            /// <summary>
            /// 送货电话
            /// </summary>
            public string corPhone { get; set; }

            public string corAddress { get; set; }

            public string corCompany { get; set; }

            /// <summary>
            ///总件数
            /// </summary>
            public int? TotalParts { get; set; }
            /// <summary>
            /// 总重量
            /// </summary>
            public decimal? TotalWeight { get; set; }
            /// <summary>

            public string Condition { get; set; }

            /// <summary>
            /// 运输批次
            /// </summary>
            public string LotNumber { get; set; }

            /// <summary>
            /// 客服人员名称
            /// </summary>
            public string Merchandiser { get; set; }

        }

        private class InsNotices
        {
            public List<InsNotice> Notices;
        }

        private class InsNotice
        {
            public string ShelveID { get; set; }

            public string BoxCode { get; set; }

            public string TinyOrderID { get; set; }

            public string OrderID { get; set; }
        }
        #endregion
    }
}
