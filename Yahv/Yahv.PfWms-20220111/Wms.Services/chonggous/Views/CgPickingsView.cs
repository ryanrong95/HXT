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
using Kdn.Library;
using Wms.Services.chonggous.Models;
using Wms.Services.Enums;

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
    public class CgPickingsView : QueryView<object, PvWmsRepository>
    {
        #region 构造器
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CgPickingsView()
        {
        }

        /// <summary>
        /// 有参构造函数，外部调用使用
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        protected CgPickingsView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 有参构造函数，条件查询使用
        /// </summary>
        /// <param name="reponsitory">数据库实例</param>
        /// <param name="iQueryable">查询结果集</param>
        protected CgPickingsView(PvWmsRepository reponsitory, IQueryable<object> iQueryable) : base(reponsitory, iQueryable)
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

            //var wayRequirementsView = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WayRequirementsTopView>()
            //                          select new
            //                          {
            //                              item.ID,
            //                              DelivaryOpportunity = item.DelivaryOpportunity
            //                          };
            //var statisticsview = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.VouchersStatisticsViewForWms>()
            //                     where item.Catalog == CatalogConsts.仓储服务费 && item.Subject == SubjectConsts.代收货款
            //                     select new
            //                     {
            //                         item.OrderID,
            //                         item.LeftPrice,
            //                         item.RightPrice
            //                     };



            var payingWaybillView = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OutPayWaybillsTopView>()
                                    where waybill.ReceiveStatus == 1
                                    select waybill.ID;

            //var payingWaybillView = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
            //                        join r in wayRequirementsView on waybill.wbID equals r.ID
            //                        join s in statisticsview on waybill.OrderID equals s.OrderID into ss 
            //                        where r.DelivaryOpportunity == (int)DelivaryOpportunity.PaymentBeforeDelivery 
            //                            && s.LeftPrice > s.RightPrice
            //                        select waybill.wbID;


            var linqs = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                            //join paying in payingWaybillView on waybill.wbID equals paying
                        join _client in clientView on waybill.wbEnterCode equals _client.EnterCode into clients
                        from client in clients.DefaultIfEmpty()
                        join _carrier in carriersTopView on waybill.wbCarrierID equals _carrier.ID into carriers
                        from carrier in carriers.DefaultIfEmpty()
                        join _supplier in wsnSuppliersTopView on new { EnterCode = waybill.wbEnterCode, SupplierName = waybill.wbSupplier } equals new { EnterCode = _supplier.EnterCode, SupplierName = _supplier.RealEnterpriseName } into Suppliers
                        from supplier in Suppliers.DefaultIfEmpty()
                        where !payingWaybillView.Contains(waybill.wbID)


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
                            ModifyDate = waybill.wbModifyDate,
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
                            AppointTime = waybill.AppointTime.HasValue ? waybill.AppointTime.Value : waybill.wldTakingDate,
                            TransferID = waybill.wbTransferID,
                            wldDriver = waybill.wldDriver,
                            wldTakingPhone = waybill.wldTakingPhone,
                            chcdDriver = waybill.chcdDriver,
                            chcdCarNumber1 = waybill.chcdCarNumber1,
                            chcdCarNumber2 = waybill.chcdCarNumber2,
                            wldCarNumber1 = waybill.wldCarNumber1,
                            LoadingExcuteStatus = (CgLoadingExcuteStauts?)waybill.loadExcuteStatus,
                            CuttingOrderStatus = (CgCuttingOrderStatus?)waybill.CuttingOrderStatus,
                            CoeAddress = waybill.coeAddress,
                            TakingContact = waybill.corContact,
                            IDNumber = waybill.coeIDNumber,
                            IDType = (IDType?)waybill.coeIDType,
                            TakingPhone = waybill.corPhone,
                            TakingDate = waybill.wldTakingDate,

                            Extype = waybill.ExType,
                            ExPayType = waybill.ExPayType,
                            ThirdPartyCardNo = waybill.ThirdPartyCardNo,

                            chgTotalPrice = waybill.chgTotalPrice,
                            ClientID = client == null ? null : client.ID,
                            ClientName = client == null ? null : client.Name,
                            coeCompany = waybill.coeCompany,
                            coeContact = waybill.coeContact,
                            coePhone = waybill.coePhone,
                            corContact = waybill.corContact,
                            corCompany = waybill.corCompany,
                            corPhone = waybill.corPhone,
                            corAddress = waybill.corAddress,
                            TotalWeight = waybill.wbTotalWeight,//总重量
                            TotalParts = waybill.wbTotalParts,
                            Condition = waybill.wbCondition,
                            LotNumber = waybill.chcdLotNumber,
                            //LotNumber = waybill.chcdLotNumber, //运输批次
                            Summary = waybill.wbSummary,
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
            var waybills = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).OrderByDescending(x => x.ModifyDate).ToArray();

            var waybillIds = waybills.Select(item => item.ID).ToArray();            
            var waybill_tinyOrderIDs = from input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                       join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on input.ID equals notice.InputID
                                       where waybillIds.Contains(notice.WaybillID)                
                                       select new
                                       {
                                           ID = notice.WaybillID,
                                           TinyOrderID = input.TinyOrderID,
                                       };

            var waybill_tinyOrderIDsEnum = waybill_tinyOrderIDs.ToArray();

            var ordersId = waybills.Select(item => item.OrderID).Distinct();
            List<string> orderList = new List<string>();
            foreach (var orderId in ordersId)
            {
                orderList.AddRange(orderId.Split(','));
            }
            orderList = orderList.Distinct().ToList();

            Dictionary<string,List<string>> tinyOrderDic = new Dictionary<string, List<string>>();
            List<string> tinyOrderList = new List<string>();            
            foreach (var tinyorder in waybill_tinyOrderIDsEnum)
            {
                if (tinyOrderDic.ContainsKey(tinyorder.ID))
                {
                    tinyOrderDic[tinyorder.ID].Add(tinyorder.TinyOrderID);
                }
                else
                {
                    tinyOrderDic.Add(tinyorder.ID, new List<string>() { tinyorder.TinyOrderID });
                }
                
                tinyOrderList.Add(tinyorder.TinyOrderID);
            }
            tinyOrderList = tinyOrderList.Distinct().ToList();

            var merchandiserTopView = from merchandiser in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderMerchandiserTopView>()
                                      where orderList.Contains(merchandiser.OrderID)
                                      select new
                                      {
                                          merchandiser.OrderID,
                                          merchandiser.RealName,
                                      };

            var trackers = merchandiserTopView.Distinct().ToArray();

            var lotNumberView = from lotNumber in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PvWmsOrderVoyageTopView>()
                                where tinyOrderList.Contains(lotNumber.TinyOrderID)
                                select new
                                {
                                    lotNumber.LotNumber,
                                    lotNumber.OrderID,
                                    lotNumber.TinyOrderID,
                                };
            var ienum_lotnumber = lotNumberView.Distinct().ToArray();

            var result = from waybill in waybills                             
                             //join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.ID equals notice.WaybillID into notices
                             //from notice in notices.DefaultIfEmpty()
                             //join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID into  g
                             //from  output in g.DefaultIfEmpty()                             
                         join lotNumber in ienum_lotnumber on waybill.OrderID equals lotNumber.OrderID into lotNumbers
                         select new
                         {

                             ID = waybill.ID,
                             CreateDate = waybill.CreateDate,
                             ModifyDate = waybill.ModifyDate,
                             EnterCode = waybill.EnterCode,
                             ClientName = waybill.ClientName,
                             Supplier = waybill.Supplier,
                             waybill.SupplierName,
                             waybill.SupplierGrade,
                             Packaging = string.IsNullOrEmpty(waybill.Packaging) ? "" : ((Package)(int.Parse(waybill.Packaging))).GetDescription(),
                             ExcuteStatus = waybill.ExcuteStatus,
                             ExcuteStatusDescription = waybill.ExcuteStatus.GetDescription(),
                             Type = waybill.Type,
                             WaybillTypeDescription = waybill.Type.GetDescription(),
                             Code = waybill.CarrierID == "6B0094F31840E2BDB62AE19557DF5B13" ? "" : waybill.Code,
                             CarrierID = waybill.CarrierID,
                             CarrierName = waybill.CarrierID == "6B0094F31840E2BDB62AE19557DF5B13" ? "" : waybill.CarrierName,
                             ConsignorID = waybill.ConsignorID,
                             Place = waybill.Place,
                             ConsignorPlace = waybill.ConsignorPlace,
                             //OrderID = waybill.OrderID,
                             OrderID = string.Join(",", tinyOrderDic[waybill.ID].Distinct().ToList()),
                             TinyOrderID = string.Join(",", tinyOrderDic[waybill.ID].Distinct().ToList()),
                             Source = waybill.Source,
                             SourceDescription = waybill.Source.GetDescription(),
                             NoticeType = waybill.NoticeType,
                             AppointTime = waybill.AppointTime,
                             TransferID = waybill.TransferID,
                             //Driver = waybill.Driver,
                             //CarNumber1 = waybill.CarNumber1,
                             LoadingExcuteStatus = waybill.LoadingExcuteStatus,                             
                             LotNumber = waybill.Source == CgNoticeSource.AgentBreakCustomsForIns ? waybill.LotNumber : string.Join(",", lotNumbers.Where(item => tinyOrderDic[waybill.ID].Contains(item.TinyOrderID)).Select(item => item.LotNumber).Distinct().ToArray()),
                             Merchandiser = trackers.FirstOrDefault(item => waybill.OrderID.IndexOf(item.OrderID) >= 0)?.RealName
                         };

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = result.ToArray(),
            };
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
            //var overDue = false; 
            var wareHouseID = noticeView.Select(item => item.WareHouseID).Distinct().ToArray().FirstOrDefault();
            //通知IDs
            var noticeids = noticeView.Select(item => item.ID).ToArray();
            ///查询跟单信息
            var client = new Yahv.Services.Views.WsClientsTopView<PvWmsRepository>(this.Reponsitory).FirstOrDefault(x => x.EnterCode == waybill.EnterCode);
            string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            var RealID = string.Join("", companyid, client?.ID).MD5();
            var map = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.MapsTrackerTopView>().FirstOrDefault(x => x.RealID == RealID && x.Type == 76);
            var traker = map == null ? null : new Yahv.Services.Views.AdminsAll<PvWmsRepository>(this.Reponsitory).FirstOrDefault(x => x.ID == map.AdminID);
            ///只显示已上架的
            var storageView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>();
            var inputView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>();
            #region 通知数据

            //获取通知的数据和已拣货数据
            var noticesView = from notice in noticeView
                              join storage in storageView on notice.StorageID equals storage.ID
                              join input in inputView on storage.InputID equals input.ID
                              join product in productview on notice.ProductID equals product.ID
                              join _pWeight in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>() on product.PartNumber equals _pWeight.PartNumber into pWeights
                              from pWeight in pWeights.DefaultIfEmpty()
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
                                  InputCreateDate = input.CreateDate,
                                  StorageCreateDate = storage.CreateDate,
                                  InputOrderID = input.OrderID,
                                  InputTinyOrderID = input.TinyOrderID,
                                  pWeight.AVGWeight,
                                  Product = new
                                  {
                                      product.PartNumber,
                                      product.Manufacturer,
                                  },
                                  Output = new
                                  {
                                      output.ID,
                                      output.OrderID,
                                      output.TinyOrderID,
                                      output.ItemID,
                                      output.Price,
                                      output.Currency,

                                  },

                                  #endregion

                              };
            //已拣货数据 
            //理论上能够出库的只能是流水库的数据
            var pickingview = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Where(item => noticeids.Contains(item.NoticeID));
            var ienum_pickings = pickingview.ToArray();

            var ienum_notices = from notice in noticesView.ToArray()
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
                                    notice.Product,
                                    notice.CustomsName,
                                    Output = notice.Output,
                                    Currency = notice.Output.Currency,
                                    notice.InputCreateDate,
                                    notice.StorageCreateDate,
                                    notice.InputOrderID,
                                    notice.InputTinyOrderID,

                                    notice.StorageID,
                                    notice.InputID,
                                    notice.BoxCode,
                                    notice.AVGWeight,
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
            /*
            [WaybillID]
            [NoticeID]
            [StorageID]
            [InputID]
            [WsOrderID]
            */

            var storagesID = ienum_notices.Select(item => item.StorageID).Where(item => item != null).Distinct();
            var inputsID = ienum_notices.Select(item => item.InputID).Where(item => item != null).Distinct();
            var tinyOrdersID = ienum_notices.Select(item => item.InputTinyOrderID).Where(item => item != null).Distinct();
            var suppliers = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                            join _supplier in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.wsnSuppliersTopView>() on storage.Supplier equals _supplier.RealEnterpriseName into supplierWithGrade
                            from supplier in supplierWithGrade.DefaultIfEmpty()
                            where storagesID.Contains(storage.ID) && supplier.EnterCode == waybill.EnterCode
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

            #region 获取随货文件
            var files = (from file in new Yahv.Services.Views.CenterFilesTopView()
                         where waybill.ID == file.WaybillID
                            || noticeids.Contains(file.NoticeID)
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

            string carrierName = waybill.CarrierName;

            string extype = null;
            if ((waybill.Type == WaybillType.InternationalExpress || waybill.Type == WaybillType.LocalExpress) && carrierName != null)
            {
                if (carrierName == "顺丰")
                {
                    SfExpType sf = new SfExpType();
                    extype = waybill.Extype.HasValue ? sf[waybill.Extype.Value] : null;
                }
                if (carrierName == "跨越速运")
                {
                    KysyExpType ky = new KysyExpType();
                    extype = waybill.Extype.HasValue ? ky[waybill.Extype.Value] : null;
                }
            }

            string exPayType = null;
            if (waybill.ExPayType.HasValue)
            {
                exPayType = ((Kdn.Library.PayType)waybill.ExPayType).GetDescription();
            }

            var merchandiserTopView = from merchandiser in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderMerchandiserTopView>()
                                      where merchandiser.OrderID == waybill.OrderID
                                      select new
                                      {
                                          merchandiser.OrderID,
                                          merchandiser.RealName,
                                      };

            var trackers = merchandiserTopView.FirstOrDefault();            

            var lotNumberView = from lotNumber in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PvWmsOrderVoyageTopView>()
                                where tinyOrdersID.Contains(lotNumber.TinyOrderID)
                                select lotNumber.LotNumber;            

            var lotNumbers = string.Join(",", lotNumberView.Distinct().ToArray());

            //乔霞测试
            //var orderRequirements = new WayRequirementsView().OrderTests; //.SingleOrDefault(item => item.ID == waybill.ID);
            //var checkRequirements = new WayRequirementsView().CheckTests;

            //真实
            var requirementsView = new WayRequirementsView().SingleOrDefault(item => item.ID == waybill.ID);

            var requirementIds = requirementsView?.Order?.Select(item => item.ID).ToArray();

            var files1 = (requirementIds == null || requirementIds.Count() == 0) ? null : (from file in new Yahv.Services.Views.CenterFilesTopView()
                                                                                           where requirementIds.Contains(file.WsOrderID)
                                                                                           select new CenterFileDescription
                                                                                           {
                                                                                               ID = file.ID,
                                                                                               WaybillID = file.WaybillID,
                                                                                               WsOrderID = file.WsOrderID,
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

            var requirements = requirementsView?.Order?.Select(item => (OrderRequirement)item);

            var ienum_requirements = requirements?.Select(item => new
            {
                item.ID,
                TypeName = item.Type.GetDescription(),
                item.Name,
                item.Requirement,
                item.Quantity,
                item.CreateDate,
                item.TotalPrice,
                File = files1.Where(f => f.WsOrderID == item.ID).SingleOrDefault()
            });
                        
            var clientLsEndDateView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientLSEndDateTopView>().FirstOrDefault(item => item.EnterCode == waybill.EnterCode);
            
            var tempStockView = from tempStock in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TWaybills>().Where(item => item.ForOrderID != null)
                                select new
                                {
                                    tempStock.ForOrderID,
                                    tempStock.CreateDate,
                                    tempStock.CompleteDate,
                                };
            var ienum_tempStocks = tempStockView.ToArray();

            bool IsTempStock = false;
            DateTime FirstTempDate = DateTime.Now;

            foreach (var item in ienum_notices)
            {
                DateTime itemTempDate = DateTime.Now;
                if (ienum_tempStocks.Any(t => t.ForOrderID == item.InputOrderID || t.ForOrderID == item.InputTinyOrderID))
                {
                    IsTempStock = true;
                    itemTempDate = ienum_tempStocks.FirstOrDefault(t => t.ForOrderID == item.InputOrderID || t.ForOrderID == item.InputTinyOrderID).CreateDate;
                }
                else
                {
                    // itemTempDate = item.InputCreateDate; 解决通知发的比较早，而导致的入库时间比较早，产生库房收取费用的问题
                    itemTempDate = item.StorageCreateDate;
                }

                FirstTempDate = itemTempDate < FirstTempDate ? itemTempDate : FirstTempDate;
            }


            var Waybill = new
            {
                waybill.ID,
                waybill.CreateDate,
                waybill.ModifyDate,
                waybill.EnterCode,
                waybill.ClientID,
                waybill.ClientName,
                Supplier_bak = waybill.Supplier,
                Supplier = group_suppliers?.Select(item => new
                {
                    item.Supplier,
                    item.SupplierName,
                    item.SupplierGrade,
                }),
                Packaging = waybill.Packaging == null ? "" : ((Package)(int.Parse(waybill.Packaging))).GetDescription(),
                waybill.ExcuteStatus,
                waybill.Type,
                Code = waybill.CarrierID == "6B0094F31840E2BDB62AE19557DF5B13" ? "" : waybill.Code,
                waybill.CarrierID,
                CarrierName = waybill.CarrierID == "6B0094F31840E2BDB62AE19557DF5B13" ? "" : waybill.CarrierName,
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
                waybill.TakingDate,
                waybill.Extype,
                ExtypeDes = extype,
                waybill.ExPayType,                
                ExPayTypeDes = exPayType,
                waybill.ThirdPartyCardNo,
                chgTotalPrice = totalPrice,
                chgTotalCurrency = ((Yahv.Underly.Currency)(totalPriceCurrency.Count() == 0 ? 0 : totalPriceCurrency.First())).GetCurrency().ShortName,
                waybill.coePhone,
                waybill.coeContact,
                waybill.CoeAddress,
                waybill.coeCompany,
                waybill.corContact,
                waybill.corPhone,
                LotNumber = lotNumbers,
                Merchandiser = trackers?.RealName,
                total = creditView == null ? 0 : creditView.Where(item => item.Business == business).ToArray()?.Sum(item => item.Total),
                totalDebt = creditView == null ? 0 : creditView.Where(item => item.Business == business).ToArray()?.Sum(item => item.Cost),
                overDue,
                Condition = waybill.Condition.JsonTo<WayCondition>(),
                Summary = waybill.Summary,
                //跟单信息
                Tracker = new
                {
                    traker?.ID,
                    traker?.RealName,
                    traker?.SelCode
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
                TotalParts = CgSzSortingsView.GetTotalPart(ienum_notices.Where(item => item.BoxCode != null).Select(item => item.BoxCode.ToUpper().Trim()).Distinct()),//总件数
                waybill.TotalWeight, //总重量
                OrderIDCount = ienum_notices.Select(notice => notice.Output.OrderID).Distinct().ToArray().Count(),
                //OrderRequirements = orderRequirements.Select(item => (OrderRequirementForShow)item),
                //DelivaryOpportunity = checkRequirements.DelivaryOpportunity,
                //OrderRequirementsReal = orderRequirements.Select(item => (OrderRequirementForShow)item),
                //OrderRequirements = requirementsView?.Order?.Select(item => (OrderRequirementForShow)item),
                OrderRequirements = ienum_requirements?.ToArray(),
                //DelivaryOpportunityReal = checkRequirements.DelivaryOpportunity,
                //DelivaryOpportunity = requirementsView == null ? null : ((CheckRequirementForShow)requirementsView.Check).DelivaryOpportunity,
                DelivaryOpportunity = requirementsView == null ? null : requirementsView.DelivaryOpportunity.HasValue ? requirementsView.DelivaryOpportunity.GetDescription() : null,                
                IsClientLs = clientLsEndDateView != null ? true : false,
                LsEndDate = clientLsEndDateView != null ? clientLsEndDateView.EndDate : (DateTime?)null,
                IsTempStock = IsTempStock,
                FirstTempDate = FirstTempDate,
            };

            return new
            {
                Waybill = Waybill,
                Notices = ienum_notices.Select(notice => new
                {
                    notice.ID,
                    notice.WaybillID,
                    notice.OutputID,
                    notice.DateCode,
                    notice.Quantity,
                    notice.Origin,
                    OriginName = ((Origin)Enum.Parse(typeof(Origin), string.IsNullOrEmpty(notice.Origin) ? nameof(Origin.Unknown) : notice.Origin)).GetDescription(),
                    notice.Source,
                    notice.CreateDate,
                    notice.Type,
                    notice.Weight,
                    notice.NetWeight,
                    AVGWeight = notice.AVGWeight.HasValue ? notice.AVGWeight.Value : 0,
                    notice.Volume,
                    ShelveID = notice.ShelveID?.Substring(3),
                    notice.Summary,
                    notice.Product,
                    Imagefiles = file_Imagefile,
                    Pickings = notice.Pickings,
                    PickedQuantity = notice.PickedQuantity,//已出库数量
                    LeftQuantity = notice.LeftQuantity,
                    CurrentQuantity = notice.CurrentQuantity,
                    Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                    BoxCode = notice.BoxCode, //乔霞的硬要求
                    TinyOrderID = notice.Output.TinyOrderID  //乔霞的硬要求

                }).OrderBy(item => item.BoxCode).ToArray(),
            };

        }

        /// <summary>
        /// 根据运单,产品型号获取产品清单
        /// </summary>
        /// <param name="WaybillID">运单号</param>
        /// <param name="key">产品型号，品牌</param>
        /// <returns></returns>
        public object GetDetailNoticeByID(string WaybillID, string key)
        {
            var noticeview = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == WaybillID);
            var productview = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);
            //var productview1 = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>();

            //根据条件过滤数据
            var notices = from notice in noticeview
                          join product in productview on notice.ProductID equals product.ID
                          join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                          where product.Manufacturer.Contains(key) || product.PartNumber.Contains(key)
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
                              notice.ShelveID,
                              notice.Summary,
                              Product = new
                              {
                                  product.PartNumber,
                                  product.Manufacturer,
                                  product.PackageCase,
                                  product.Packaging,
                              },
                              Output = new
                              {
                                  output.ID,
                                  output.OrderID,
                                  output.TinyOrderID,
                                  output.ItemID,
                              },

                              #endregion
                          };

            var noticesId = noticeview.Select(item => item.ID).ToArray();

            var pickingview = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().
                Where(item => noticesId.Contains(item.NoticeID));

            //Origin? origin = Origin.NAM;

            var data = from notice in notices.ToArray()
                       join picking in pickingview.ToArray() on notice.ID equals picking.NoticeID into pickings
                       select new
                       {
                           #region 视图

                           notice.ID,
                           notice.WaybillID,
                           notice.OutputID,
                           notice.DateCode,
                           notice.Quantity,
                           //在sql 视图中也有解决办法
                           //在ienum视图中也有解决办法
                           Origin = (Origin)Enum.Parse(typeof(Origin), notice.Origin ?? nameof(Origin.Unknown)),
                           //Origin1 = Enum.TryParse()(Origin?)notice.Origin ?? (Origin)Enum.Parse(typeof(Origin), notice.Origin),
                           //Origind = ((Origin)Enum.Parse(typeof(Origin), notice.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                           notice.Source,
                           notice.Type,
                           notice.Weight,
                           notice.NetWeight,
                           notice.Volume,
                           notice.ShelveID,
                           notice.Summary,
                           notice.Product,
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
                           PickedQuantity = pickings.Sum(s => s.Quantity),
                           LeftQuantity = notice.Quantity - pickings.Sum(s => s.Quantity),
                           CurrentQuantity = notice.Quantity - pickings.Sum(s => s.Quantity),
                           Conditions = notice.Conditions.JsonTo<NoticeCondition>(),

                           #endregion
                       };
            return data;
        }

        #region 查询条件搜索
        /// <summary>
        /// 根据运单号，订单号搜索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CgPickingsView SearchByID(string id)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var waybillIDs = from waybill in waybillView
                             where waybill.ID.Contains(id) || waybill.Code.Contains(id) || (waybill.LotNumber.Contains(id)) || waybill.EnterCode.Contains(id)
                             select waybill.ID;

            var waybillIDs2 = from input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                              join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on input.ID equals notice.InputID
                              where input.TinyOrderID == id
                              select notice.WaybillID;                              
                              
            var waybillIDs_enum = waybillIDs.Distinct().ToArray().Concat(waybillIDs2.Distinct().ToArray());

            var linq = from waybill in waybillView
                       where waybillIDs_enum.Contains(waybill.ID)
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }


        /// <summary>
        /// 根据客户搜索
        /// </summary>
        /// <param name="Client">客户名称</param>
        /// <returns></returns>
        public CgPickingsView SearchByClient(string Client)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var clientEnterCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>().Where(item => item.Name.Contains(Client)).Select(item => item.EnterCode);

            var linq = from waybill in waybillView
                       join entercode in clientEnterCodes on waybill.EnterCode equals entercode
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }


        /// <summary>
        /// 根据partNumber搜索
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public CgPickingsView SearchByPartNumber(string partNumber)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq_waybillIds = from product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                                  join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on product.ID equals notice.ProductID
                                  where product.PartNumber.Contains(partNumber) && notice.WareHouseID.StartsWith(wareHouseID)
                                  orderby notice.WaybillID descending
                                  select notice.WaybillID;

            var takes = linq_waybillIds.Distinct().Take(5000);

            var linq = from waybill in waybillView
                       join id in takes on waybill.ID equals id
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }


        /// <summary>
        /// 根据业务来源搜索
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CgPickingsView SearchBySource(int sources)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            //  var includes = sources.Select(item =>(int)item).FirstOrDefault();

            var linq = from waybill in waybillView
                       where (int)waybill.Source == sources
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运输批次搜索
        /// </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns></returns>
        public CgPickingsView SearchByLotNumber(string lotnumber)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = from waybill in waybillView
                       where waybill.LotNumber == lotnumber
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据开始时间搜索
        /// </summary>
        /// <param name="startdate"></param>
        /// <returns></returns>
        public CgPickingsView SearchByStartDate(DateTime startdate)
        {
            var linq = from waybill in this.IQueryable.Cast<MyWaybill>()
                       where waybill.CreateDate >= startdate
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据截止时间搜索
        /// </summary>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public CgPickingsView SearchByEndDate(DateTime enddate)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = from waybill in waybillView
                       where waybill.CreateDate < enddate.AddDays(1)
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        string wareHouseID;

        /// <summary>
        /// 根据库房ID搜索
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public CgPickingsView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyWaybill>();

            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgPickingsView(this.Reponsitory, waybillView);
            }

            Expression<Func<MyWaybill, bool>> express = null;
            if (wareHouseID.StartsWith(nameof(WhSettings.HK)))
            {
                express = waybill => (waybill.NoticeType == CgNoticeType.Out && (waybill.Source == CgNoticeSource.Transfer || waybill.Source == CgNoticeSource.AgentSend));
            }

            if (wareHouseID.StartsWith(nameof(WhSettings.SZ)))
            {
                //express = waybill => waybill.NoticeType == CgNoticeType.Out;
                express = waybill => (waybill.NoticeType == CgNoticeType.Out && waybill.Source == CgNoticeSource.AgentBreakCustoms) || (waybill.NoticeType == CgNoticeType.Out && waybill.Source == CgNoticeSource.AgentBreakCustomsForIns && waybill.CuttingOrderStatus.HasValue && waybill.CuttingOrderStatus == CgCuttingOrderStatus.Completed);
            }

            if (express == null)
            {
                throw new NotImplementedException("不可思议的错误！");
            }

            waybillView = waybillView.Where(express);

            var linq_waybillIDs = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  where notice.WareHouseID.StartsWith(this.wareHouseID)
                                  select notice.WaybillID;

            var ienum_waybillIDs = linq_waybillIDs.Distinct();/*.Take(1000)*/

            var linq = from waybill in waybillView
                       join id in ienum_waybillIDs on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        #region 注释
        /// <summary>
        /// 根据运单执行状态搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public CgPickingsView SearchByStatus(params CgPickingExcuteStatus[] status)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where status.Contains(waybill.ExcuteStatus)
                       select waybill;

            var view = new CgPickingsView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }
        #endregion

        #endregion


        #region 拣货出库操作
        /// <summary>
        /// 拣货完成
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>
        /// 捡货完成，这里其实等待出库
        /// </remarks>
        public void Completed(JToken token)
        {
            //可以调用部分拣货的方法
            //目前出库就是出库，点击完成后，只是判断状态。
            //如果是全都出库了，就修改状态：CgLoadingExcuteStauts
            //扣除库存,原则上能出库的一定是流水库
            this.PartialPicking(token);
        }

        /// <summary>
        /// 部分拣货
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>
        /// 如果是正规的拣货操作需要通过扫码完成拣货，尤其是部分拣货
        /// 因此，本方法应该是部分拣货
        /// </remarks>
        public void PartialPicking(JToken token)
        {
            //扣除库存,原则上能出库的一定是流水库
            //复核 生成   Pickings
            //例如：扫描或是用肉眼看 这个产品是要出库的  ，点击XX出库操作的时候，要生成Pickings 数据.

            //运单处理
            var adminid = token["AdminID"].Value<string>();
            var Waybill = token["Waybill"];
            var orderID = Waybill["OrderID"].Value<string>();
            var waybilltop = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().
                SingleOrDefault(item => item.wbID == Waybill["ID"].Value<string>());
            var source = (CgNoticeSource)waybilltop.Source;
            var warehouseID = Waybill["WarehouseID"].Value<string>();

            var realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminid).RealName;
            var waybillid = Waybill["ID"].Value<string>();
            string driver = Waybill["Driver"].Value<string>();
            string driverName = Waybill["Driver"]?.Value<string>();
            string carNumber = Waybill["CarNumber1"].Value<string>();
            string carrierID = Waybill["CarrierID"].Value<string>();
            string driverPhone = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.DriversTopView>().SingleOrDefault(item => item.EnterpriseID == carrierID && item.Name == driver)?.Mobile;
            string carrierName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CarriersTopView>().SingleOrDefault(item => item.ID == carrierID)?.Name;
            string code = Waybill["Code"]?.Value<string>();
            string codeValue = Waybill["Code"]?.Value<string>();
            List<CgLogs_Operator> logsOperatorList = new List<CgLogs_Operator>();
            var waybillType = Waybill["Type"]?.Value<int?>();
            if (warehouseID.StartsWith("SZ") && Waybill.HasValues && waybillType.Value == (int)WaybillType.LocalExpress)
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new Exception("请先点击一键打印,运单打印,生成运单ID, 然后再进行出库操作.");
                }
            }

            //去除出库时对订单状态的判断，由跟单员管理端去判断
            //if ((CgNoticeSource)waybilltop.Source == CgNoticeSource.AgentSend || (CgNoticeSource)waybilltop.Source == CgNoticeSource.Transfer)
            //{
            //    if (orderstatus != OrderPaymentStatus.Paid)
            //    {
            //        throw new Exception("订单未完成支付, 不能完成出库操作!");
            //    }                
            //} 

            if (warehouseID.StartsWith("SZ"))
            {
                var Pickings = token["Pickings"];

                var noticeIds = Pickings.Select(item => item["NoticeID"].Value<string>());
                var storageView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on notice.StorageID equals storage.ID
                                  where noticeIds.Contains(notice.ID) && storage.WareHouseID.StartsWith("SZ") && storage.Type == 200
                                  select new
                                  {
                                      NoticeID = notice.ID,
                                      StorageID = storage.ID,
                                      storage.WareHouseID,
                                      storage.ShelveID,
                                  };
                var ienum_storages = storageView.ToArray();

                if (ienum_storages.Any(item => string.IsNullOrEmpty(item.ShelveID)))
                {
                    throw new Exception("请完成上架操作后再进行深圳外单出库操作!");
                }
            }

            //处理拣货数据出库,更新流水库数据
            var pickings = this.OnEnterPickings(token);

            var picking_View = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillid)
                               join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID
                               join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on notice.StorageID equals storage.ID
                               join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
                               select new
                               {
                                   notice,
                                   product,
                                   picking,
                                   storage
                               };
            var pickings_enum = picking_View.Select(item => item.picking).ToList();

            foreach (var picking in pickings_enum)
            {
                var single = picking_View.Single(item => item.picking.ID == picking.ID);
                CgLogs_Operator logsOperator = new CgLogs_Operator();
                logsOperator.MainID = waybillid;
                logsOperator.Type = LogOperatorType.Insert;
                logsOperator.Conduct = "拣货";
                logsOperator.CreatorID = adminid;
                logsOperator.CreateDate = DateTime.Now;
                logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 拣货, 型号: {single.product.PartNumber}, 品牌: {single.product.Manufacturer}, 产地: {single.storage.Origin} 数量: {single.picking.Quantity}";
                logsOperatorList.Add(logsOperator);
            }

            code = !string.IsNullOrEmpty(code) ? (",运单ID: " + code) : "";
            driver = !string.IsNullOrEmpty(driver) ? (", 司机: " + driver) : "";
            carNumber = !string.IsNullOrEmpty(carNumber) ? (", 车牌号: " + carNumber) : "";
            carrierName = !string.IsNullOrEmpty(carrierName) ? (", 承运商: " + carrierName) : "";
            CgLogs_Operator logsWaybillOperator = new CgLogs_Operator();
            logsWaybillOperator.MainID = waybillid;
            logsWaybillOperator.Type = LogOperatorType.Update;
            logsWaybillOperator.Conduct = "拣货";
            logsWaybillOperator.CreatorID = adminid;
            logsWaybillOperator.CreateDate = DateTime.Now;
            logsWaybillOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Update.GetDescription()} 拣货, WaybillID: {waybillid} {code}{carrierName}{driver}{carNumber}";
            logsOperatorList.Add(logsWaybillOperator);            

            //处理运单状态
            var ExcuteStatus = this.GetExcuteStatus(Waybill);
            //修改运单数据
            this.EnterWaybill(Waybill, adminid, ExcuteStatus);


            if (waybilltop.NoticeType != (int)CgNoticeType.Boxing)
            {
                //出库完成后，流水库数量更新为0

                var tPickings = from item in pickings_enum.Select(item => new
                {
                    item.StorageID,
                    item.Quantity,
                }).ToArray()
                                group item by item.StorageID into groups
                                select new
                                {
                                    StorageID = groups.Key,
                                    Quantity = groups.Sum(item => item.Quantity)
                                };

                var storagesid = tPickings.Select(item => item.StorageID);
                var storages = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => storagesid.Contains(item.ID)).ToArray();

                StringBuilder builder = new StringBuilder();
                Layers.Data.Sqls.PvWms.Storages tstorages;
                foreach (var picking in tPickings)
                {
                    var storage = storages.Single(item => item.ID == picking.StorageID);
                    var value = storage.Quantity - picking.Quantity;
                    if (value < 0)
                    {
                        CgLogs_Operator updateStorageError = new CgLogs_Operator();
                        updateStorageError.MainID = waybillid;
                        updateStorageError.Type = LogOperatorType.Update;
                        updateStorageError.Conduct = "拣货更新库存Error";
                        updateStorageError.CreatorID = adminid;
                        updateStorageError.CreateDate = DateTime.Now;
                        updateStorageError.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Update.GetDescription()} 拣货更新库存Error, WaybillID: {waybillid}, storage.Quantity:{storage.Quantity}, picking.Quantity: {picking.Quantity}, value: {value}";
                        logsOperatorList.Add(updateStorageError);

                        throw new NotImplementedException("不能实现这样的逻辑：扣库存为负数！");
                    }

                    builder.AppendFormat($@"update [{nameof(Layers.Data.Sqls.PvWms.Storages)}]
                        set [{nameof(tstorages.Quantity)}] = {value}
                    where [{nameof(tstorages.ID)}] = '{picking.StorageID}';
                    ");
                    //this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
                    //{
                    //    Quantity = value,
                    //}, item => item.ID == picking.StorageID);
                }
                // 确保不在同一个会话里
                using (var r = new PvWmsRepository())
                {
                   r.Command(builder.ToString());
                }              

                CgLogs_Operator updateStorage = new CgLogs_Operator();
                updateStorage.MainID = waybillid;
                updateStorage.Type = LogOperatorType.Update;
                updateStorage.Conduct = "拣货更新库存";
                updateStorage.CreatorID = adminid;
                updateStorage.CreateDate = DateTime.Now;
                updateStorage.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Update.GetDescription()} 拣货更新库存, WaybillID: {waybillid}, 更新库存语句:{builder.ToString()}";
                logsOperatorList.Add(updateStorage);
            }

            if (logsOperatorList.Count() > 0)
            {
                foreach (var log in logsOperatorList)
                {
                    log.Enter(this.Reponsitory);
                }
            }
            // 如果出库完成则更新订单出库状态
            if (ExcuteStatus == CgPickingExcuteStatus.Completed)
            {
                if (string.IsNullOrEmpty(waybilltop.OrderID))
                {
                    //如果出库运单的OrderID为空
                    var waybillId = waybilltop.wbID;
                    var outputids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillId)
                        .Select(item => item.OutputID).ToArray();
                    var orderids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(item => outputids.Contains(item.ID))
                        .Select(item => item.OrderID).Distinct().ToArray();
                    foreach (var id in orderids)
                    {
                        UpdateOrderStatus(source, (CgNoticeType)waybilltop.NoticeType, id, adminid);
                        if (warehouseID.StartsWith("SZ") && waybillType.Value != (int)WaybillType.PickUp )
                        {
                            //代报关出库向客户发送深圳仓库已出货消息
                            PushMsg pushMsg = new PushMsg(1, (int)SpotName.DSZSend, id, codeValue, driverName, driverPhone);
                            pushMsg.push();
                        }
                    }
                }
                else
                {
                    UpdateOrderStatus(source, (CgNoticeType)waybilltop.NoticeType, waybilltop.OrderID, adminid);

                    if (warehouseID.StartsWith("SZ") && waybillType.Value != (int)WaybillType.PickUp )
                    {
                        //代报关深圳出库向客户发送深圳仓库已出货消息
                        PushMsg pushMsg = new PushMsg(1, (int)SpotName.DSZSend, waybilltop.OrderID, codeValue, driverName, driverPhone);
                        pushMsg.push();
                    }
                }

                if (warehouseID.StartsWith("HK") && waybillType.Value != (int)WaybillType.PickUp && (waybilltop.Source == (int)CgNoticeSource.AgentSend || waybilltop.Source == (int)CgNoticeSource.Transfer))
                {
                    //代仓储出库向客户发送已出货消息
                    PushMsg pushMsg = new PushMsg(2, (int)SpotName.HKSend, waybilltop.OrderID, codeValue, driverName, driverPhone);
                    pushMsg.push();                    
                }

                // 香港出库中的代发货, 代转运, 以及深圳出库时, 当点击完成分拣时把对应的 ConfirmReceiptStatus 修改为100
                if (warehouseID.StartsWith("SZ") || waybilltop.Source == (int)CgNoticeSource.AgentSend || waybilltop.Source == (int)CgNoticeSource.Transfer)
                {
                    using (var pvcenterReponsitory = new PvCenterReponsitory())
                    {
                        pvcenterReponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                        {
                            ConfirmReceiptStatus = 100,
                        }, item => item.ID == waybilltop.wbID);
                    }
                }
            }


            #region _bak

            //if (waybilltop.NoticeType != (int)CgNoticeType.Boxing)
            //{
            //    //出库完成后，流水库数量更新为0
            //    var storageids = pickings.Select(item => item.StorageID).Distinct().ToArray();

            //    foreach (var item in pickings)
            //    {


            //        this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Storages>(new
            //        {
            //            Quantity = 0M,
            //        }, item => storageids.Contains(item.ID));
            //    }
            //}

            #endregion

            //装箱全部完成后,转报关单生成申报日志
            //if (ExcuteStatus == CgPickingExcuteStatus.Completed && waybilltop.NoticeType == (int)CgNoticeType.Boxing
            //    && waybilltop.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
            //{
            //    this.OnEnterDeclareLog(waybilltop, adminid, pickings);
            //}


            // 深圳库房出库成功后调用芯达通的接口,判断一下如果是深圳出库 就调用
            if (Waybill["WarehouseID"].Value<string>().Contains("SZ"))
            {
                string[] orderids = null;
                if (string.IsNullOrEmpty(waybilltop.OrderID))
                {
                    //如果出库运单的OrderID为空
                    var waybillId = waybilltop.wbID;
                    var outputids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillId)
                        .Select(item => item.OutputID).ToArray();
                    orderids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(item => outputids.Contains(item.ID))
                        .Select(item => item.OrderID).Distinct().ToArray();
                }
                else
                {
                    orderids = new string[] { waybilltop.OrderID };                    
                }

                var result = Yahv.Utils.Http.ApiHelper.Current.JPost(Wms.Services.FromType.XdtSZShiped.GetDescription(), new SZToXDT { AdminID = adminid, WaybillID = Waybill["ID"].Value<string>(), OrdersID = orderids });
                var resultJson = result.JsonTo<JMessage>();
                if (!resultJson.success)
                {
                    throw new Exception("调用芯达通 深圳出库接口失败," + resultJson.data);
                }
            }
        }


        /// <summary>
        /// 生成申报日志
        /// </summary>
        /// <param name="Waybill">运单</param>
        /// <param name="Pickings">拣货数据</param>
        private void OnEnterDeclareLog(Layers.Data.Sqls.PvWms.WaybillsTopView Waybill, string AdminID, Layers.Data.Sqls.PvWms.Pickings[] Pickings)
        {
            //过滤箱号为空的
            Pickings = Pickings.Where(item => !string.IsNullOrWhiteSpace(item.BoxCode)).ToArray();

            var linq = (from output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().ToArray()
                        join picking in Pickings on output.ID equals picking.OutputID
                        select new
                        {
                            output.InputID,
                            output.TinyOrderID,
                            output.ItemID,
                            picking.NoticeID,
                            picking.ID,
                            picking.OutputID,
                            picking.BoxCode,
                            picking.StorageID,
                            picking.Quantity,
                            picking.Weight,
                            picking.NetWeight,
                            picking.Volume,
                        }).ToArray();

            //生成申报报告
            foreach (var output in linq)
            {
                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().Any(item => item.TinyOrderID == output.TinyOrderID))
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Declare
                    {
                        ID = PKeySigner.Pick(PkeyType.LogsDeclare),
                        TinyOrderID = output.TinyOrderID,
                        //WaybillID = Waybill.wbID,
                        EnterCode = Waybill.wbEnterCode,
                        //BoxCode = output.BoxCode,
                        AdminID = AdminID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Enums.TinyOrderDeclareStatus.Boxed,
                    });
                }

                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.OrderItemID == output.ItemID && item.StorageID == output.StorageID))
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_DeclareItem
                    {
                        ID = PKeySigner.Pick(PkeyType.LogsDeclareItem),
                        TinyOrderID = output.TinyOrderID,
                        OrderItemID = output.ItemID,
                        StorageID = output.StorageID, //noticeEntity.notice.StorageID,
                        Quantity = output.Quantity,
                        AdminID = AdminID,
                        OutputID = output.OutputID,// 装箱通知中的OutputID,
                        BoxCode = output.BoxCode,
                        Weight = output.Weight,
                        NetWeight = output.NetWeight,
                        Volume = output.Volume,
                    });
                }




            }
        }


        /// <summary>
        /// picking数据持久化
        /// </summary>
        /// <param name="Pickings">拣货数据</param>
        /// <param name="token"></param>
        private Layers.Data.Sqls.PvWms.Pickings[] OnEnterPickings(JToken token)
        {
            var Waybill = token["Waybill"];
            var Pickings = token["Pickings"];
            string WareHouseID = Waybill["WarehouseID"].Value<string>();

            int total = Pickings.Count();
            var ids = PKeySigner.Series(PkeyType.Pickings,total);

            var linq = Pickings.Select((item , index ) =>
            {               
                var OutputID = item["OutputID"].Value<string>();
                var noticeID = item["NoticeID"].Value<string>();
                string StorageID = null;
                var notice = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().SingleOrDefault(n => n.ID == noticeID);

                //获取销项信息对应的进项ID
                var InputID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().SingleOrDefault(a => a.ID == OutputID).InputID;
                var Quantity = item["Quantity"].Value<decimal>();

                if (notice != null && (notice.Source == (int)(CgNoticeSource.AgentBreakCustoms) || notice.Source == (int)(CgNoticeSource.AgentBreakCustomsForIns) || notice.Source == (int)(CgNoticeSource.AgentCustomsFromStorage)))
                {
                    StorageID = notice.StorageID;
                }
                else
                {
                    StorageID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().
                    Where(x => x.WareHouseID.Contains(WareHouseID)).
                   FirstOrDefault(a => a.InputID == InputID && a.Type == (int)CgStoragesType.Flows && a.Quantity == Quantity).ID;
                }
                
                //依据inputID + Quantity 进行关联
                //var StorageID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().
                //    Where(x => x.WareHouseID.Contains(WareHouseID)).
                //    FirstOrDefault(a => a.InputID == InputID && a.Type == (int)CgStoragesType.Flows).ID;

                return new Layers.Data.Sqls.PvWms.Pickings
                {
                    ID = ids[index] ,// PKeySigner.Pick(PkeyType.Pickings),
                    StorageID = StorageID,
                    OutputID = OutputID,
                    NoticeID = item["NoticeID"].Value<string>(),
                    BoxCode = item["BoxCode"].Value<string>(),
                    Quantity = Quantity,
                    AdminID = token["AdminID"].Value<string>(),
                    CreateDate = DateTime.Now,
                    Weight = item["Weight"]?.Value<decimal?>(),
                    NetWeight = item["NetWeight"]?.Value<decimal?>(),
                    Volume = item["Volume"]?.Value<decimal?>(),
                    Summary = Waybill["Summary"]?.Value<string>(),
                };

            }).ToArray();

            this.Reponsitory.Insert(linq);

            //更新库存图片,当前高会航已经把所有上传文件地方都做了更新
            #region 更新库存图片
            /*
            using (var centerreponsitory = new PvCenterReponsitory())
            {
                foreach (var picking in linq)
                {
                    // 更新Files信息
                    var files = Pickings["Files"].Values<string>();
                    if (files.Count() > 0)
                    {
                        centerreponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            NoticeID = picking.NoticeID,
                            StorageID = picking.StorageID,
                        }, item => files.Contains(item.ID));
                    }
                    
                }
            }
            */
            #endregion

            //更新Picking的文件
            //using (var centerreponsitory = new PvCenterReponsitory())
            //{
            //    foreach (var picking in linq)
            //    {
            //        // 更新Files信息
            //        var files = Waybill["Files"].Values<string>();
            //        if (files.Count() > 0)
            //        {
            //            centerreponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
            //            {
            //                PickingID = picking.ID
            //            }, item => files.Contains(item.ID));
            //        }
            //    }
            //}

            return linq;
        }

        /// <summary>
        /// 出库状态获取
        /// </summary>
        /// <param name="WaybillID"></param>
        private CgPickingExcuteStatus GetExcuteStatus(JToken Waybill)
        {
            var excuteStatus = (CgPickingExcuteStatus?)Waybill["ExcuteStatus"]?.Value<int>();
            var waybillid = Waybill["ID"].Value<string>();
            //查询当前运单的通知数量和所有分拣数量            

            var linq = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillid)
                       join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID into pickingss
                       select new
                       {
                           notice,
                           pickingss
                       };

            var ienum_linq = linq.ToArray().Select(item => new
            {
                NoticeQuantity = item.notice.Quantity,
                PickingQuantity = item.pickingss.ToArray().Sum(s => s.Quantity)
            });

            if (ienum_linq.All(item => item.PickingQuantity == 0))
            {
                excuteStatus = CgPickingExcuteStatus.Picking;
            }
            else if (ienum_linq.Any(item => item.NoticeQuantity > item.PickingQuantity))
            {
                excuteStatus = CgPickingExcuteStatus.PartialShiped;
            }
            else
            {
                excuteStatus = CgPickingExcuteStatus.Completed;
            }
            return excuteStatus.Value;
        }

        /// <summary>
        /// 修改订单
        /// </summary>
        /// <remarks>
        /// 送货上门的
        /// </remarks>
        private void EnterWaybill(JToken Waybill, string AdminID, CgPickingExcuteStatus excuteStatus)
        {
            //可以修改，承运商、司机、车牌
            using (var reponsitory = new PvCenterReponsitory())
            {
                var waybillid = Waybill["ID"].Value<string>();
                var Type = Waybill["Type"].Value<int>();
                var wareHouseID = Waybill["WarehouseID"].Value<string>();
                //入库是香港库房的话，更新送货日期为出库时间
                if (wareHouseID.StartsWith("HK"))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ExcuteStatus = (int)excuteStatus,
                        CarrierID = Waybill["CarrierID"]?.Value<string>(),
                        ModifyDate = DateTime.Now,
                        ModifierID = AdminID,
                        Summary = Waybill["Summary"]?.Value<string>(),
                        Code = Waybill["Code"]?.Value<string>(),
                        Type = Waybill["Type"]?.Value<int?>() ?? Type,
                        AppointTime = DateTime.Now
                    }, item => item.ID == waybillid);
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        ExcuteStatus = (int)excuteStatus,
                        CarrierID = Waybill["CarrierID"]?.Value<string>(),
                        ModifyDate = DateTime.Now,
                        ModifierID = AdminID,
                        Summary = Waybill["Summary"]?.Value<string>(),
                        Code = Waybill["Code"]?.Value<string>(),
                        Type = Waybill["Type"]?.Value<int?>() ?? Type,
                    }, item => item.ID == waybillid);


                }

                var exType = Waybill["ExType"]?.Value<int>();
                var exPayType = Waybill["ExPayType"]?.Value<int>();
                if (exType.HasValue && exPayType.HasValue)
                {
                    ///快递方式
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.WayExpress>(new
                    {
                        ExType = exType.Value,
                        ExPayType = exPayType.Value,
                    }, item => item.ID == waybillid);
                }

                //客户自提,更新提货状态,送货上门,由库房决定司机和车牌号
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Any(item => item.ID == waybillid))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                    {
                        ID = waybillid,
                        CarNumber1 = Waybill["CarNumber1"]?.Value<string>(),
                        Driver = Waybill["Driver"]?.Value<string>(),
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        CreatorID = AdminID,
                        ModifierID = AdminID,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                    {
                        Driver = Waybill["Driver"]?.Value<string>(),
                        CarNumber1 = Waybill["CarNumber1"]?.Value<string>(),
                        ExcuteStatus = Waybill["LoadingExcuteStatus"]?.Value<int?>(),
                    }, item => item.ID == waybillid);



                }
                //更新ConsignorPlace   先 注释 弄清楚后再修改

                /*根据董建新要求, 入库出库不修改运单中的收发货人
                string consignorID = Waybill["ConsignorID"]?.Value<string>();
                string consignorPlace = Waybill["ConsignorPlace"]?.Value<string>();

                if (!string.IsNullOrWhiteSpace(consignorID) && !string.IsNullOrWhiteSpace(consignorPlace))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.WayParters>(new
                    {
                        Place = consignorPlace,
                    }, item => item.ID == consignorID);
                }
                */

                //更新Files信息，当前上传文件逻辑高会航已经处理过
                #region 更新Files信息
                /*
                var files = Waybill["Files"].Values<string>();
                if (files.Count() > 0)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                    {
                        WaybillID = Waybill["ID"].Value<string>(),
                        WsOrderID = Waybill["OrderID"].Value<string>(),
                    }, item => files.Contains(item.ID));
                }
                */
                #endregion

                #region 运单状态日志记录
                UpdateLogWaybillStatus(waybillid, excuteStatus, AdminID);
                #endregion
            }
        }

        #endregion        

        #region  转报关

        public void SortingCompleted(JToken token)
        {
            //处理拣货数据出库,更新流水库数据
            var pickings = this.OnEnterPickings(token);

            //运单处理
            var adminid = token["AdminID"].Value<string>();
            var Waybill = token["Waybill"];
            var waybillid = Waybill["ID"].Value<string>();

            List<CgLogs_Operator> logsOperatorList = new List<CgLogs_Operator>();

            var realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminid).RealName;

            var picking_View = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillid)
                               join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID
                               join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on notice.StorageID equals storage.ID
                               join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on notice.ProductID equals product.ID
                               select new
                               {
                                   notice,
                                   product,
                                   picking,
                                   storage
                               };


            foreach (var picking in pickings)
            {
                var single = picking_View.Single(item => item.picking.ID == picking.ID);
                CgLogs_Operator logsOperator = new CgLogs_Operator();
                logsOperator.MainID = waybillid;
                logsOperator.Type = LogOperatorType.Insert;
                logsOperator.Conduct = "拣货";
                logsOperator.CreatorID = adminid;
                logsOperator.CreateDate = DateTime.Now;
                logsOperator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {LogOperatorType.Insert.GetDescription()} 拣货, 型号: {single.product.PartNumber}, 品牌: {single.product.Manufacturer}, 产地: {single.storage.Origin} 数量: {single.picking.Quantity}";
                logsOperatorList.Add(logsOperator);
            }

            if (logsOperatorList.Count() > 0)
            {
                foreach (var log in logsOperatorList)
                {
                    log.Enter(this.Reponsitory);
                }
            }
            //var ExcuteStatus = this.GetExcuteStatus(Waybill);

            //查询当前运单的通知数量和所有分拣数量
            #region 处理运单状态
            var excuteStatus = (CgPickingExcuteStatus?)Waybill["ExcuteStatus"]?.Value<int>();

            var linq = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillid)
                       join picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID into pickingss
                       select new
                       {
                           notice,
                           pickingss
                       };

            var ienum_linq = linq.ToArray().Select(item => new
            {
                NoticeQuantity = item.notice.Quantity,
                PickingQuantity = item.pickingss.ToArray().Sum(s => s.Quantity)
            });

            if (ienum_linq.All(item => item.PickingQuantity == 0))
            {
                excuteStatus = CgPickingExcuteStatus.Picking;
            }
            else if (ienum_linq.Any(item => item.NoticeQuantity > item.PickingQuantity))
            {
                excuteStatus = CgPickingExcuteStatus.PartialShiped;
            }
            else
            {
                excuteStatus = CgPickingExcuteStatus.Completed;
            }
            var ExcuteStatus = excuteStatus.Value;
            #endregion
            //修改运单数据
            this.EnterWaybill(Waybill, adminid, ExcuteStatus);

            var waybilltop = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().
       SingleOrDefault(item => item.wbID == Waybill["ID"].Value<string>());

            // 更新运单累加状态
            UpdateLogWaybillStatus(Waybill["ID"].Value<string>(), excuteStatus.Value, adminid);

            //装箱全部完成后,转报关单生成申报日志
            if (ExcuteStatus == CgPickingExcuteStatus.Completed && waybilltop.NoticeType == (int)CgNoticeType.Boxing
                && waybilltop.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
            {
                var waybillID = Waybill["ID"].Value<string>();
                var pickingView = from picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                                  join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  on new { NoticeID = picking.NoticeID, OutputID = picking.OutputID } equals new { NoticeID = notice.ID, OutputID = notice.OutputID }
                                  where notice.WaybillID == waybillID && notice.Type == (int)CgNoticeType.Boxing
                                  select picking;
                var ienum_pickings = pickingView.ToArray();

                this.OnEnterDeclareLog(waybilltop, adminid, ienum_pickings);

                //更新订单状态
                UpdateOrderStatus((CgNoticeSource)waybilltop.Source, (CgNoticeType)waybilltop.NoticeType, waybilltop.OrderID, adminid);
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

        /// <summary>
        /// 更新运单的Code
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="code"></param>
        public void UpdateWaybillCode(string waybillID, string code)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Code = code,
                }, item => item.ID == waybillID);
            }
        }

        /// <summary>
        /// 更新深圳出库运单的快递方式，（只能是顺丰，或者跨越速运）
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="shipperCode">快递公司编码SF 或者 KYSY</param>
        /// <param name="exType"></param>
        /// <param name="exPayType"></param>
        public void UpdateWaybillExpress(string waybillID, string shipperCode, int? exType, int? exPayType, string thirdPartyCardNo)
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                var carrierID = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.CarriersTopView>().Single(item => item.Code == shipperCode).ID;

                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    CarrierID = carrierID,
                    Code = string.Empty,
                }, item => item.ID == waybillID);

                if (reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayExpress>().Any(item => item.ID == waybillID))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.WayExpress>(new
                    {
                        ExType = exType,
                        ExPayType = exPayType,
                        ThirdPartyCardNo = thirdPartyCardNo,
                    }, item => item.ID == waybillID);
                }
                else
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayExpress
                    {
                        ID = waybillID,
                        ExType = exType,
                        ExPayType = exPayType,
                        ThirdPartyCardNo = thirdPartyCardNo,
                    });
                }
            }
        }

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
                    if (noticeSource == CgNoticeSource.AgentSend || noticeSource == CgNoticeSource.Transfer)
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

        /// <summary>
        /// 修改Waybill的Code
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="code"></param>
        /// <param name="enterCode"></param>
        public void ModifyWaybillCode(string waybillID, string code)
        {
            //当前去验证合单时，不再该客户下是否已经有对应的运单号
            //而是直接验证: 一，运单号存在，二是，已存在运单号的地址，和现有的运单的地址相同就可以
            var waybillsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();

            var waybill = waybillsView.Single(item => item.wbID == waybillID);
            var newWaybill = waybillsView.FirstOrDefault(item => item.wbCode == code);

            if (newWaybill != null && newWaybill.coeAddress == waybill.coeAddress)
            {
                using (var pvcenterResponsitory = new PvCenterReponsitory())
                {
                    pvcenterResponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        Code = code,
                    }, item => item.ID == waybillID);
                }
            }
            else
            {
                throw new Exception($"运单号{code}不存在或该运单号对应的收货地址与现有运单的收货地址不同!");
            }
        }

        /// <summary>
        /// 用于库房修改出库通知 地址、联系人、电话
        /// </summary>
        /// <param name="address"></param>
        /// <param name="contact"></param>
        /// <param name="phone"></param>
        public void ModifyWaybillConsigneeInfo(string waybillID, string address, string name, string mobile)
        {
            var waybill = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == waybillID);

            var coeID = waybill.coeID;
            var coeCompany = waybill.coeCompany;
            var coeAddress = waybill.coeAddress;
            var coeContact = waybill.coeContact;
            var coePhone = waybill.coePhone;
            var coeZipcode = waybill.coeZipcode;
            var coeEmail = waybill.coeEmail;
            var coeIDType = waybill.coeIDType;
            var coeIDNumber = waybill.coeIDNumber;

            var WayParter = new Layers.Data.Sqls.PvCenter.WayParters()
            {
                Company = waybill.coeCompany,
                Place = waybill.coePlace,
                Address = address,
                Contact = name,
                Phone = mobile,
                Zipcode = waybill.coeZipcode,
                Email = waybill.coeEmail,
                CreateDate = DateTime.Now,
                IDType = waybill.coeIDType,
                IDNumber = waybill.coeIDNumber,
            };
            WayParter.ID = string.Concat(WayParter.Company, WayParter.Place, WayParter.Address, WayParter.Contact,
                WayParter.Phone, WayParter.Zipcode, WayParter.Email).MD5();

            using (var pvcenterResponsitory = new PvCenterReponsitory())
            {
                if (!pvcenterResponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == WayParter.ID))
                {
                    pvcenterResponsitory.Insert(WayParter);
                }

                pvcenterResponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ConsigneeID = WayParter.ID,
                }, item => item.ID == waybillID);
            }            
        }
        #endregion

        #region Helper Class
        /// <summary>
        /// 符合Picking视图头部定义的内部类
        /// </summary>
        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime? ModifyDate { get; set; }
            public string EnterCode { get; set; }
            public string ClientID { get; set; }
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
            public List<string> TinyOrderID { get; set; }
            //public string TinyOrderID { get; set; }
            public string Packaging { get; set; }
            public string Summary { get; set; }
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
            /// <summary>
            /// 具体承运商司机联系电话
            /// </summary>
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

            public CgCuttingOrderStatus? CuttingOrderStatus { get; set; }
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
            /// 自提时间
            /// </summary>
            public DateTime? TakingDate { get; set; }

            /// <summary>
            /// 快递类型
            /// </summary>
            public int? Extype { get; set; }
            /// <summary>
            /// 快递支付方式
            /// </summary>

            public int? ExPayType { get; set; }
            /// <summary>
            /// 第三方月结卡号
            /// </summary>
            public string ThirdPartyCardNo { get; set; }
            /// <summary>
            /// 总货值
            /// </summary>

            public decimal? chgTotalPrice { get; set; }

            /// <summary>
            /// 收货公司
            /// </summary>
            public string coeCompany { get; set; }

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
        #endregion
    }
}
