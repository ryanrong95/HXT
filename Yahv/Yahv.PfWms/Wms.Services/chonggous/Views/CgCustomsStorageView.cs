using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Wms.Services.Enums;
using Yahv.Linq;
using Yahv.Payments;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Views
{
    public class CgCustomsStorageView : QueryView<object, PvWmsRepository>
    {
        #region 构造器
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public CgCustomsStorageView()
        {
        }

        /// <summary>
        /// 有参构造函数, 外部调用使用
        /// </summary>
        /// <param name="reponsitory"></param>
        protected CgCustomsStorageView(PvWmsRepository reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 有参构造函数, 条件查询使用
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="iQueryable"></param>
        protected CgCustomsStorageView(PvWmsRepository reponsitory, IQueryable<object> iQueryable): base(reponsitory, iQueryable)
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
            
            var linqs = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                        join _client in clientView on waybill.wbEnterCode equals _client.EnterCode into clients
                        from client in clients.DefaultIfEmpty()
                        join _carrier in carriersTopView on waybill.wbCarrierID equals _carrier.ID into carriers
                        from carrier in carriers.DefaultIfEmpty()
                        orderby waybill.wbCreateDate descending
                        select new MyWaybill
                        {
                            ID = waybill.wbID,
                            CreateDate = waybill.wbCreateDate,
                            ModifyDate = waybill.wbModifyDate,
                            EnterCode = waybill.wbEnterCode,
                            Packaging = waybill.wbPackaging,
                            ExcuteStatus = (CgPickingExcuteStatus)waybill.wbExcuteStatus,
                            Type = (WaybillType)waybill.wbType,
                            Code = waybill.wbCode,
                            CarrierID = waybill.wbCarrierID,
                            CarrierName = carrier != null ? carrier.Name : null,
                            OrderID = waybill.OrderID,
                            Source = (CgNoticeSource)waybill.Source,
                            NoticeType = (CgNoticeType)waybill.NoticeType,
                            TransferID = waybill.wbTransferID,
                            LoadingExcuteStatus = (CgLoadingExcuteStauts?)waybill.loadExcuteStatus,
                            CuttingOrderStatus = (CgCuttingOrderStatus?)waybill.CuttingOrderStatus,
                            chgTotalPrice = waybill.chgTotalPrice,
                            ClientName = client == null ? null : client.Name,                            
                            TotalWeight = waybill.wbTotalWeight,//总重量
                            TotalParts = waybill.wbTotalParts,
                            Condition = waybill.wbCondition,
                            LotNumber = null,
                            
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

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_waybills = iquery.ToArray().OrderByDescending(item => item.ModifyDate);

            var waybillIds = ienum_waybills.Select(item => item.ID).Distinct();
            var orderIds = ienum_waybills.Select(item => item.OrderID).Distinct();

            var merchandiserTopView = from merchandiser in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderMerchandiserTopView>()
                                      where orderIds.Contains(merchandiser.OrderID)
                                      select new
                                      {
                                          merchandiser.OrderID,
                                          merchandiser.RealName,
                                      };
            var trackers = merchandiserTopView.Distinct().ToArray();

            var result = from waybill in ienum_waybills
                         select new
                         {
                             ID = waybill.ID,
                             CreateDate = waybill.CreateDate,
                             ModifyDate = waybill.ModifyDate,
                             EnterCode = waybill.EnterCode,
                             ClientName = waybill.ClientName,                             
                             Packaging = string.IsNullOrEmpty(waybill.Packaging) ? "" : ((Package)(int.Parse(waybill.Packaging))).GetDescription(),
                             ExcuteStatus = waybill.ExcuteStatus,
                             ExcuteStatusDescription = waybill.ExcuteStatus.GetDescription(),
                             Type = waybill.Type,
                             WaybillTypeDescription = waybill.Type.GetDescription(),
                             Code = waybill.Code,
                             CarrierID = waybill.CarrierID,
                             CarrierName = waybill.CarrierName,
                             OrderID = waybill.OrderID,
                             Source = waybill.Source,
                             SourceDescription = waybill.Source.GetDescription(),
                             NoticeType = waybill.NoticeType,
                             TransferID = waybill.TransferID,
                             LoadingExcuteStatus = waybill.LoadingExcuteStatus,
                             Merchandiser = trackers.SingleOrDefault(item => item.OrderID == waybill.OrderID)?.RealName
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
        /// 获取详情数据
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public object GetDetail(string waybillID)
        {
            var waybill = this.IQueryable.Cast<MyWaybill>().SingleOrDefault(item => item.ID == waybillID);
            var productview = new Yahv.Services.Views.ProductsTopView<PvWmsRepository>(this.Reponsitory);
            var noticeView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillID);
            var orderItem = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderItemsTopView>().Where(item => item.OrderID == waybill.OrderID).ToArray();// 取报关总货值
            var totalPriceInstance = orderItem.FirstOrDefault();

            var orderInfo = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsOrdersTopView>().SingleOrDefault(item => item.ID == waybill.OrderID);
            var creditView = orderInfo == null ? null : this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CreditsStatisticsView>().Where(item => item.Currency == 1 && item.Payer == orderInfo.ClientID && item.Payee == orderInfo.PayeeID);
            var isCustoms = waybill.Source == CgNoticeSource.AgentBreakCustoms || waybill.Source == CgNoticeSource.AgentBreakCustomsForIns || waybill.Source == CgNoticeSource.AgentCustomsFromStorage;
            var business = isCustoms ? ConductConsts.代报关 : ConductConsts.代仓储;
            var overDue = orderInfo == null ? true : PaymentManager.Npc[orderInfo.ClientID, orderInfo.PayeeID][business].DebtTerm[DateTime.Now].IsOverdue;
            var wareHouseID = noticeView.Select(item => item.WareHouseID).Distinct().ToArray().FirstOrDefault();
            //通知IDs
            var noticeids = noticeView.Select(item => item.ID).ToArray();
            //查询跟单信息
            var merchandisers = from merchandiser in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderMerchandiserTopView>()
                           where merchandiser.OrderID == waybill.OrderID
                           select new
                           {
                               merchandiser.RealName,
                               merchandiser.OrderID,
                           };

            var trackers = merchandisers.Distinct().ToArray();

            #region 获取随货文件
            var files = (from file in new Yahv.Services.Views.CenterFilesTopView()
                         where waybill.ID == file.WaybillID
                            || noticeids.Contains(file.NoticeID)
                         || file.WsOrderID == waybill.OrderID
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
            var file_delivergood = files.Where(item => item.Type == (int)FileType.DeliverGoods && item.WaybillID == waybillID).ToArray();
            //标签文件
            var file_Label = files.Where(item => item.Type == (int)FileType.Label).ToArray();

            var file_Imagefile = files.Where(item => item.Type == (int)FileType.StoragesPic).ToArray();
            var file_sendGoods = files.Where(item => item.Type == (int)FileType.SendGoods && item.WaybillID == waybillID).ToArray();

            #endregion
            // 获取通知的数据,以及拣货数据
            var noticesView = from notice in noticeView
                              join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                             on notice.StorageID equals storage.ID
                              join input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                              on storage.InputID equals input.ID
                              join output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                              on notice.OutputID equals output.ID
                              join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                              on notice.ProductID equals product.ID
                              join _pWeight in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>()
                              on product.PartNumber equals _pWeight.PartNumber into pWeights
                              from pWeight in pWeights.DefaultIfEmpty()
                              where notice.Type == (int)CgNoticeType.Boxing
                              select new
                              {
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
                                  notice.ID,
                                  notice.WaybillID,
                                  notice.OutputID,
                                  notice.Origin,
                                  notice.Quantity,
                                  Source = (NoticeSource)notice.Source,
                                  Type = (CgNoticeType)notice.Type,
                                  notice.Weight,
                                  notice.NetWeight,
                                  notice.Volume,
                                  pWeight.AVGWeight,
                                  ShelveID = storage.ShelveID,
                                  StorageInputID = storage.InputID,
                                  notice.Summary,
                                  notice.CreateDate,
                                  notice.CustomsName,
                                  notice.Conditions,
                                  InputCreateDate = input.CreateDate,
                                  StorageCreateDate = storage.CreateDate,
                                  InputOrderID = input.OrderID,
                                  InputTinyOrderID = input.TinyOrderID,
                              };

            var pickingView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Where(item => noticeids.Contains(item.NoticeID));            
            var ienum_pickings = pickingView.ToArray();            

            var ienum_notices = from notice in noticesView.ToArray()
                                join picking in ienum_pickings on notice.ID equals picking.NoticeID into pickings                                
                                select new
                                {
                                    notice.ID,
                                    notice.WaybillID,
                                    notice.OutputID,
                                    notice.Quantity,
                                    notice.Origin,
                                    OriginName = ((Origin)Enum.Parse(typeof(Origin), string.IsNullOrEmpty(notice.Origin) ? nameof(Origin.Unknown) : notice.Origin)).GetDescription(),
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
                                    TinyOrderID = notice.Output.TinyOrderID,                                    
                                    Output = notice.Output,
                                    Curreny = notice.Output.Currency,
                                    notice.AVGWeight,
                                    notice.InputCreateDate,
                                    notice.StorageCreateDate,
                                    notice.InputOrderID,
                                    notice.InputTinyOrderID,                                    
                                    Imagefiles = file_Imagefile,
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
                                    Conditions = notice.Conditions.JsonTo<NoticeCondition>(),
                                    totalPrice = notice.Quantity * notice.Output.Price
                                };
            var totalPrice = ienum_notices.Sum(x => x.totalPrice);
            var totalPriceCurrency = ienum_notices.Select(x => x.Curreny).Distinct().ToArray();
            
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

            foreach(var item in ienum_notices)
            {
                DateTime itemTempDate = DateTime.Now;
                if (ienum_tempStocks.Any(t => t.ForOrderID == item.InputOrderID || t.ForOrderID == item.InputTinyOrderID))
                {
                    IsTempStock = true;
                    itemTempDate = ienum_tempStocks.FirstOrDefault(t => t.ForOrderID == item.InputOrderID || t.ForOrderID == item.InputTinyOrderID).CreateDate;
                }
                else
                {
                    //itemTempDate = item.InputCreateDate;  //解决由于通知发的比较早，而导致的收取入仓费的问题
                    itemTempDate = item.StorageCreateDate;
                }

                FirstTempDate = itemTempDate < FirstTempDate ? itemTempDate : FirstTempDate;
            }

            return new
            {
                Waybill = new
                {

                    waybill.ID,
                    waybill.CreateDate,
                    waybill.ModifyDate,
                    waybill.EnterCode,
                    waybill.ClientName,
                    Packaging = waybill.Packaging == null ? "" : ((Package)(int.Parse(waybill.Packaging))).GetDescription(),
                    waybill.ExcuteStatus,
                    waybill.Type,
                    waybill.Code,
                    waybill.CarrierID,
                    waybill.CarrierName,
                    waybill.Source,
                    waybill.NoticeType,
                    waybill.TransferID,
                    waybill.OrderID,
                    TypeName = waybill.Type.GetDescription(),
                    ExcuteStatusName = waybill.ExcuteStatus.GetDescription(),
                    Files = file_followgoods,
                    FeliverGoodFile = file_delivergood,
                    LableFile = file_Label,
                    SendGoodsFile = file_sendGoods,//送货文件                  
                    SourceName = ((CgNoticeSource)Enum.Parse(typeof(CgNoticeSource), waybill.Source.ToString())).GetDescription(),
                    chgTotalPrice = totalPrice,
                    chgTotalCurrency = ((Yahv.Underly.Currency)(totalPriceCurrency.Count() == 0 ? 0 : totalPriceCurrency.First())).GetCurrency().ShortName,
                    Merchandiser = trackers.SingleOrDefault(item => item.OrderID == waybill.OrderID)?.RealName,
                    total = creditView == null ? 0 : creditView.Where(item => item.Business == business).ToArray()?.Sum(item => item.Total),
                    totalDebt = creditView == null ? 0 : creditView.Where(item => item.Business == business).ToArray()?.Sum(item => item.Cost),
                    overDue,
                    Condition = waybill.Condition.JsonTo<WayCondition>(),
                    IsClientLs = clientLsEndDateView != null ? true : false,
                    LsEndDate = clientLsEndDateView != null ? clientLsEndDateView.EndDate : (DateTime?)null,
                    IsTempStock = IsTempStock,
                    FirstTempDate = FirstTempDate,
                },
                Notices = ienum_notices.ToArray(),
            };
        }

        /// <summary>
        /// 转报关分拣
        /// </summary>
        /// <param name="token"></param>
        public void PickingCompleted(JToken token)
        {
            //处理拣货数据出库,更新流水库数据
            var pickings = this.OnEnterPickings(token);

            // 更新Logs_Storage的箱号等信息
            foreach (var picking in pickings)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
                {
                    IsCurrent = false,
                }, item => item.StorageID == picking.StorageID && item.IsCurrent == true);

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Storage
                {
                    IsCurrent = true,
                    AdminID = picking.AdminID,
                    BoxCode = picking.BoxCode,
                    CreateDate = DateTime.Now,
                    ID = Guid.NewGuid().ToString(),
                    StorageID = picking.StorageID,
                    Summary = null,
                    Weight = picking.Weight.HasValue ? picking.Weight.Value : 0,
                });
            }

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

            //查询当前运单的通知数量和所有分拣数量
            var excuteStatus = CheckAndUpdateWaybillStatus(waybillid, adminid, this.Reponsitory);

            CheckWaybillExcuteStatusAndDeclare(waybillid, adminid, excuteStatus, this.Reponsitory);
        }

        #region Helper Method
        /// <summary>
        /// Picking数据持久化
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Layers.Data.Sqls.PvWms.Pickings[] OnEnterPickings(JToken token)
        {
            var Waybill = token["Waybill"];
            var Pickings = token["Pickings"];
            string WareHouseID = Waybill["WarehouseID"].Value<string>();

            var linq = Pickings.Select(item =>
            {
                var OutputID = item["OutputID"].Value<string>();
                //获取销项信息对应的进项ID
                var InputID = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().SingleOrDefault(a => a.ID == OutputID).InputID;
                var Quantity = item["Quantity"].Value<decimal>();
                var storageFlow = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().
                    Where(x => x.WareHouseID.Contains(WareHouseID)).
                   SingleOrDefault(a => a.InputID == InputID && a.Type == (int)CgStoragesType.Flows && a.Quantity == Quantity && a.Summary == OutputID);

                if (storageFlow == null)
                {
                    storageFlow = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().
                    Where(x => x.WareHouseID.Contains(WareHouseID)).
                   FirstOrDefault(a => a.InputID == InputID && a.Type == (int)CgStoragesType.Flows && a.Quantity == Quantity);
                }
                var StorageID = storageFlow.ID;

                return new Layers.Data.Sqls.PvWms.Pickings
                {
                    ID = PKeySigner.Pick(PkeyType.Pickings),
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
            return linq;
        }

        /// <summary>
        /// 修改运单状态
        /// </summary>
        /// <param name="Waybill"></param>
        /// <param name="AdminID"></param>
        /// <param name="excuteStatus"></param>
        public static void EnterWaybill(string waybillid, string AdminID, CgPickingExcuteStatus excuteStatus)
        {
            using (var pvcenterRepository = new PvCenterReponsitory())
            {
                pvcenterRepository.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ExcuteStatus = (int)excuteStatus,
                    ModifierID = AdminID,
                }, item => item.ID == waybillid);
            }
        }

        /// <summary>
        /// 检查并更新运单的执行状态
        /// </summary>
        /// <param name="waybillid"></param>
        /// <param name="adminID"></param>
        public static CgPickingExcuteStatus CheckAndUpdateWaybillStatus(string waybillid, string adminID, PvWmsRepository reponsitory)
        {
            //查询当前运单的通知数量和所有分拣数量
            #region 处理运单状态
            var excuteStatus = (CgPickingExcuteStatus)reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == waybillid).wbExcuteStatus;

            var linq = from notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybillid)
                       join picking in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>() on notice.ID equals picking.NoticeID into pickingss
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
            
            #endregion
            //修改运单数据
            EnterWaybill(waybillid, adminID, excuteStatus);

            return excuteStatus;
        }

        public static void CheckWaybillExcuteStatusAndDeclare(string waybillid, string adminid, CgPickingExcuteStatus excuteStatus,  PvWmsRepository reponsitory)
        {
            var waybilltop = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().
       SingleOrDefault(item => item.wbID == waybillid);

            // 更新运单累加状态
            UpdateLogWaybillStatus(waybillid, adminid, excuteStatus);

            //装箱全部完成后,转报关单生成申报日志
            if (excuteStatus == CgPickingExcuteStatus.Completed && waybilltop.NoticeType == (int)CgNoticeType.Boxing
                && waybilltop.Source == (int)CgNoticeSource.AgentCustomsFromStorage)
            {
                var pickingView = from picking in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                                  join notice in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  on new { NoticeID = picking.NoticeID, OutputID = picking.OutputID } equals new { NoticeID = notice.ID, OutputID = notice.OutputID }
                                  where notice.WaybillID == waybillid && notice.Type == (int)CgNoticeType.Boxing
                                  select picking;
                var ienum_pickings = pickingView.ToArray();

                OnEnterDeclareLog(waybilltop, adminid, ienum_pickings, reponsitory);

                //更新订单状态
                UpdateOrderStatus((CgNoticeSource)waybilltop.Source, (CgNoticeType)waybilltop.NoticeType, waybilltop.OrderID, adminid);
            }
        }
        /// <summary>
        /// 更新运单日志的累加状态
        /// </summary>
        /// <param name="waybillid"></param>
        /// <param name="AdminID"></param>
        /// <param name="excuteStatus"></param>
        public static void UpdateLogWaybillStatus(string waybillid, string adminID, CgPickingExcuteStatus excuteStatus)
        {
            using (var pvcenterRepository = new PvCenterReponsitory())
            {
                pvcenterRepository.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false,
                }, item => item.Type == (int) WaybillStatusType.ExecutionStatus && item.MainID == waybillid);

                pvcenterRepository.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = waybillid,
                    Type = (int)Yahv.Underly.Enums.WaybillStatusType.ExecutionStatus,//建议询问小辉
                    Status = (int)excuteStatus,//建议询问小辉
                    CreateDate = DateTime.Now,
                    CreatorID = adminID,
                    IsCurrent = true,
                });
            }
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="noticeSource"></param>
        /// <param name="noticeType"></param>
        /// <param name="waybillorderid"></param>
        /// <param name="adminID"></param>
        public static void UpdateOrderStatus(CgNoticeSource noticeSource, CgNoticeType noticeType, string waybillorderid, string adminID)
        {
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            {
                if (noticeType == CgNoticeType.Out)
                {
                    /* 根据新要求, 对于代报关 ,转报关, 已发货状态删除不再使用
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
                        Status = (int)CgOrderStatus.已发货,//建议询问小辉
                        CreateDate = DateTime.Now,
                        CreatorID = adminID,
                        IsCurrent = true,
                    });
                    */
                    var orderID = waybillorderid;
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
        /// 生成申报日志及申报项日志
        /// </summary>
        /// <param name="Waybill"></param>
        /// <param name="AdminID"></param>
        /// <param name="Pickings"></param>
        public static void OnEnterDeclareLog(Layers.Data.Sqls.PvWms.WaybillsTopView Waybill, string AdminID, Layers.Data.Sqls.PvWms.Pickings[] Pickings, PvWmsRepository reponsitory)
        {
            //过滤箱号为空的
            Pickings = Pickings.Where(item => !string.IsNullOrWhiteSpace(item.BoxCode)).ToArray();

            var linq = (from output in reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().ToArray()
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
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().Any(item => item.TinyOrderID == output.TinyOrderID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Declare
                    {
                        ID = PKeySigner.Pick(PkeyType.LogsDeclare),
                        TinyOrderID = output.TinyOrderID,                        
                        EnterCode = Waybill.wbEnterCode,
                        AdminID = AdminID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Enums.TinyOrderDeclareStatus.Boxed,
                    });
                }

                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.OrderItemID == output.ItemID && item.StorageID == output.StorageID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_DeclareItem
                    {
                        ID = PKeySigner.Pick(PkeyType.LogsDeclareItem),
                        TinyOrderID = output.TinyOrderID,
                        OrderItemID = output.ItemID,
                        StorageID = output.StorageID,
                        Quantity = output.Quantity,
                        AdminID = AdminID,
                        OutputID = output.OutputID,
                        BoxCode = output.BoxCode,
                        Weight = output.Weight,
                        NetWeight = output.NetWeight,
                        Volume = output.Volume,
                        Status = 0,
                    });
                }
            }
        }

        /// <summary>
        /// 封箱操作, 提供是否能进行封箱成功的判断
        /// </summary>
        /// <param name="waybillID"></param>
        /// <param name="adminID"></param>
        /// <param name="arry"></param>
        public void CloseBoxes(string waybillID, string adminID, string[] arry)
        {
            var waybill = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == waybillID);
            var excuteStatus = waybill.wbExcuteStatus;

            if (excuteStatus != (int)CgPickingExcuteStatus.Completed)
            {
                throw new Exception("转报关封箱，需要完成装运后才能进行封箱!");
            }

            var pickingView = from picking in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                              join notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                              on new { NoticeID = picking.NoticeID, OutputID = picking.OutputID } equals new { NoticeID = notice.ID, OutputID = notice.OutputID }
                              where notice.WaybillID == waybillID && notice.Type == (int)CgNoticeType.Boxing
                              select picking;
            var ienum_pickings = pickingView.ToArray();            

            OnEnterDeclareLog(waybill, adminID, ienum_pickings, this.Reponsitory);

            List<string> abnormalListPicking = new List<string>();
            List<string> normalListPicking = new List<string>(arry);
            using (var pvcenterReponsitory = new PvCenterReponsitory())
            using (var pvwmsReponsitory = new PvWmsRepository())
            {
                //历史分拣
                var linq_pickings = from picking in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                                   join notice in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on picking.NoticeID equals notice.ID                                   
                                   join output in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on picking.OutputID equals output.ID
                                   join storage in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on picking.StorageID equals storage.ID
                                   join product in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                                   where notice.WaybillID == waybillID
                                   select new
                                   {
                                       picking.NoticeID,
                                       product.Manufacturer,
                                       product.PartNumber,
                                       storage.Origin,
                                       picking.Quantity,
                                       output.TinyOrderID,
                                   };
                var ienums_pickings = linq_pickings.ToArray();

                //当前的全部通知
                var linq_notices = from notice in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                   join output in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                   on notice.OutputID equals output.ID
                                   join product in pvwmsReponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                                   on notice.ProductID equals product.ID
                                   where notice.WaybillID == waybillID
                                   select new
                                   {
                                       notice.ID,
                                       product.Manufacturer,
                                       product.PartNumber,
                                       notice.Origin,
                                       notice.Quantity,
                                       output.TinyOrderID,
                                   };
                var ienums_notices = linq_notices.ToArray();


                var linq_merger = from notice in ienums_notices
                                  join picking in ienums_pickings on new { NoticeID = notice.ID, PartNumber = notice.PartNumber, Manufacturer = notice.Manufacturer, Origin = notice.Origin, TinyOrderID = notice.TinyOrderID }
                                  equals new { NoticeID = picking.NoticeID, PartNumber = picking.PartNumber, Manufacturer = picking.Manufacturer, Origin = picking.Origin, TinyOrderID = picking.TinyOrderID } into _pickings
                                  select new
                                  {
                                      NoticeID = notice.ID,
                                      noticeQuantity = notice.Quantity,
                                      notice.PartNumber,
                                      notice.Manufacturer,
                                      notice.Origin,
                                      notice.TinyOrderID,
                                      pickingQuantity = _pickings.Sum(item => item.Quantity)
                                  };


                foreach (var tinyOrderID in arry)
                {
                    var isAbnormalForPicking = false;
                    if (ienums_pickings.Where(item => item.TinyOrderID == tinyOrderID).Any(item => item.NoticeID == null))
                    {
                        isAbnormalForPicking = true;                        
                    }

                    if (linq_merger.Where(item => item.TinyOrderID == tinyOrderID).Any(item => item.noticeQuantity != item.pickingQuantity))
                    {
                        isAbnormalForPicking = true;
                    }

                    if (isAbnormalForPicking)
                    {
                        abnormalListPicking.Add(tinyOrderID);
                        normalListPicking.Remove(tinyOrderID);
                    }
                }
            }

            // 处理分拣与通知不一致的情况
            if (abnormalListPicking.Count > 0)
            {
                var abnormalArryPicking = abnormalListPicking.ToArray();
                var pickingContent = string.Join(",", abnormalArryPicking);

                throw new Exception($"小订单{pickingContent}中包含与入库通知不一致的分拣,请处理后再进行封箱操作!");
            }

            // 检测WsOrderItemTopView 与 到货的实际情况是否完全一致
            var wsOrderItemTopViews = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WsOrderItemTopView>()
                                      select new
                                      {
                                          OrderItemID = entity.ItemID,
                                          entity.TinyOrderID,
                                          entity.Origin,
                                          PartNumber = entity.Model,
                                          entity.Manufacturer,
                                          entity.Quantity,
                                      };

            var logDeclareItemViews = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                                      join storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>() on entity.StorageID equals storage.ID
                                      join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                                      select new
                                      {
                                          entity.OrderItemID,
                                          entity.TinyOrderID,
                                          entity.Quantity,
                                          storage.Origin,
                                          product.Manufacturer,
                                          product.PartNumber,
                                      };            

            List<string> abnormalList = new List<string>();
            List<string> normalList = new List<string>(arry);

            // 申报项视图与芯达通订单项视图对比检查
            bool isAbnormal = false;
            foreach (var tinyOrderID in arry)
            {
                bool islogDeclareItemAbnormal = false;
                bool iswsOrderItemAbnormal = false;
                var wsOrderItemView = wsOrderItemTopViews.Where(item => item.TinyOrderID == tinyOrderID);
                var logDeclareItemView = logDeclareItemViews.Where(item => item.TinyOrderID == tinyOrderID);
                var logDeclareItemGroupView = from entity in logDeclareItemView
                                              group entity by new { entity.TinyOrderID, entity.OrderItemID, entity.PartNumber, entity.Manufacturer, entity.Origin } into entitygroup
                                              select new
                                              {
                                                  entitygroup.Key.TinyOrderID,
                                                  entitygroup.Key.OrderItemID,
                                                  entitygroup.Key.PartNumber,
                                                  entitygroup.Key.Manufacturer,
                                                  entitygroup.Key.Origin,
                                                  Quantity = entitygroup.Sum(item => item.Quantity),
                                              };                

                foreach (var logDeclareItem in logDeclareItemGroupView.ToArray())
                {
                    var wsOrderItem = wsOrderItemView.SingleOrDefault(item => item.OrderItemID == logDeclareItem.OrderItemID);
                    if (wsOrderItem == null || wsOrderItem.Manufacturer != logDeclareItem.Manufacturer || wsOrderItem.PartNumber != logDeclareItem.PartNumber || wsOrderItem.Origin != logDeclareItem.Origin || wsOrderItem.Quantity != logDeclareItem.Quantity)
                    {
                        islogDeclareItemAbnormal = true;
                        break;
                    }
                }

                foreach (var wsOrderItem in wsOrderItemView)
                {
                    var logDeclareItem = logDeclareItemGroupView.SingleOrDefault(item => item.OrderItemID == wsOrderItem.OrderItemID);
                    if (logDeclareItem == null || logDeclareItem.Manufacturer != wsOrderItem.Manufacturer || logDeclareItem.PartNumber != wsOrderItem.PartNumber || logDeclareItem.Origin != wsOrderItem.Origin || logDeclareItem.Quantity != wsOrderItem.Quantity)
                    {
                        iswsOrderItemAbnormal = true;
                        break;
                    }
                }

                if (islogDeclareItemAbnormal || iswsOrderItemAbnormal)
                {
                    isAbnormal = true;
                    abnormalList.Add(tinyOrderID);
                    normalList.Remove(tinyOrderID);
                }
            }

            #region 正常封箱成功的TinyOrderID, 进行待报关申请页面的显示
            if (normalList.Count > 0)
            {
                var newArry = normalList.ToArray();
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    Status = 1,
                }, item => newArry.Contains(item.TinyOrderID));
            }
            #endregion

            #region 封箱失败的TinyOrderID， 提示操作人
            if (isAbnormal)
            {
                var abnormalArry = abnormalList.ToArray();
                var noticeContent = string.Join(",", abnormalArry);
                throw new Exception($"小订单{noticeContent}的待申报项与芯达通订单项视图检测中，型号，品牌，产地，数量有不一致的内容，需要等跟单先处理到货异常");
            }
            #endregion
        }

        /// <summary>
        /// 删除对应的拣货历史
        /// </summary>
        /// <param name="pickingID"></param>
        /// <param name="storageID"></param>
        /// <param name="adminID"></param>
        public void DeletePicking(string pickingID, string storageID, string adminID)
        {
            //ToDo: 检查是否已经报关 declare的状态需要检查
            //仅仅删除Picking, 并不需要删除销项记录, 不需要删除库存, 不需要恢复流水库存, 
            //流水库存是在装箱通知时生成的,一直存在

            var linq_declare = from declare in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>()
                               join declareitem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>()
                               on declare.TinyOrderID equals declareitem.TinyOrderID
                               select new
                               {
                                   declare.TinyOrderID,
                                   declare.Status,
                                   declareitem.OrderItemID,
                                   declareitem.StorageID,
                                   declareitem.Quantity,
                                   declareitem.BoxCode,
                               };
            if (linq_declare.Any(item => item.StorageID == storageID && item.Status >= (int)TinyOrderDeclareStatus.Declaring))
            {
                throw new Exception($"拣货{pickingID}不能删除,该拣货装箱的库存已申报!");
            }

            var realName = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Single(item => item.ID == adminID).RealName;

            var view = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                       join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>() on storage.ProductID equals product.ID
                       where storage.ID == storageID
                       select new
                       {
                           storage.Quantity,
                           storage.Origin,
                           product.Manufacturer,
                           product.PartNumber,
                       };
            var single = view.FirstOrDefault();

            //var storage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Single(item => item.ID == storageID);
            //var product = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>().Single(item => item.ID == storage.ProductID);
            var picking = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Single(item => item.ID == pickingID);            
            var waybillid = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Single(item => item.ID == picking.NoticeID).WaybillID;
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => item.ID == pickingID);
            this.Reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => item.StorageID == storageID);

            //把删除的Picking对应的流水库的库存日志记录的IsCurrent状态改变即可
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
            {
                IsCurrent = false,
            }, item => item.IsCurrent == true && item.StorageID == storageID);

            CgLogs_Operator logs_operator = new CgLogs_Operator();
            logs_operator.Conduct = "拣货";
            logs_operator.CreatorID = adminID;
            logs_operator.Type = LogOperatorType.Delete;
            logs_operator.CreateDate = DateTime.Now;
            logs_operator.Content = $"{realName} 在 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 拣货 {LogOperatorType.Delete.GetDescription()} 型号:{single.PartNumber} 品牌:{single.Manufacturer} 产地:{single.Origin} 数量:{single.Quantity}";
            logs_operator.MainID = waybillid;
            logs_operator.Enter();

            //删除箱号
            CgBoxManage.Current.Delete(picking.BoxCode);

            // 检查删除拣货记录后的运单状态并更新
            CheckAndUpdateWaybillStatus(waybillid, adminID, this.Reponsitory);
        }

        /// <summary>
        /// 更新箱号
        /// </summary>
        /// <param name="storageID"></param>
        /// <param name="boxcode"></param>
        /// <param name="adminID"></param>
        public void ModifyBoxCode(string storageID, string boxcode, string adminID)
        {
            //ToDo:检查申报日志中是否已经申报
                        
            var customsStorageView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgCustomStoragePickingReportTopView>().SingleOrDefault(item => item.StorageID == storageID && item.StorageType == (int)CgStoragesType.Flows);

            if (customsStorageView == null)
            {
                throw new Exception($"不能更新该拣货历史的箱号, 没有找到库存{storageID}对应的拣货记录!");
            }

            if (customsStorageView.DelcareStatus.HasValue && customsStorageView.DelcareStatus.Value >= 30)
            {
                throw new Exception($"库存{storageID}对应的拣货记录的箱号不能修改,该拣货记录的库存已经申报!");
            }

            // 更新分拣中的箱号
            this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Pickings>(new
            {
                BoxCode = boxcode,
            }, item => item.ID == customsStorageView.PickingID);

            var storagelogs = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Storage>().SingleOrDefault(item => item.StorageID == storageID && item.IsCurrent == true);

            if (storagelogs != null)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_Storage>(new
                {
                    IsCurrent = false,
                }, item => item.StorageID == storageID && item.IsCurrent == true);

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Storage
                {
                    ID = Guid.NewGuid().ToString(),
                    AdminID = adminID,
                    BoxCode = boxcode,
                    CreateDate = DateTime.Now,
                    IsCurrent = true,
                    StorageID = storageID,
                    Summary = null,
                    Weight = storagelogs.Weight,
                });
            }

            // 如果申报项日志中有, 则更新BoxCode
            if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_DeclareItem>().Any(item => item.StorageID == storageID))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(new
                {
                    BoxCode = boxcode,
                }, item => item.StorageID == storageID);
            }
        }
        #endregion

        #region 查询条件搜索
        /// <summary>
        /// 根据型号搜索
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public CgCustomsStorageView SearchByPartNumber(string partNumber)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var linq_waybillIds = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                                  on notice.ProductID equals product.ID
                                  where product.PartNumber.Contains(partNumber) && notice.WareHouseID.StartsWith(wareHouseID)
                                  orderby notice.WaybillID descending
                                  select notice.WaybillID;
            var ienum_ids = linq_waybillIds.Distinct();
            var linq = from waybill in waybillView
                       join id in ienum_ids on waybill.ID equals id
                       select waybill;

            var view = new CgCustomsStorageView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据起止时间查询
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public CgCustomsStorageView SearchByDate(DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<MyWaybill, bool>> express = waybill => (startdate == null ? true : waybill.CreateDate >= startdate.Value)
            && (enddate == null ? true : waybill.CreateDate < enddate.Value.AddDays(1));

            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = waybillView.Where(express);

            var view = new CgCustomsStorageView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据运单号,订单号搜索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CgCustomsStorageView SearchByID(string id)
        {
            var waybillView = IQueryable.Cast<MyWaybill>();

            var linq = from waybill in waybillView
                       where waybill.ID.Contains(id) || waybill.Code.Contains(id) || waybill.OrderID.Contains(id)
                       orderby waybill.ID descending
                       select waybill;

            var view = new CgCustomsStorageView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据客户名搜索
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public CgCustomsStorageView SearchByClient(string client)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();

            var clientEnterCodes = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>().Where(item => item.Name.Contains(client)).Select(item => item.EnterCode);

            var linq = from waybill in waybillView
                       join entercode in clientEnterCodes on waybill.EnterCode equals entercode
                       select waybill;
            var view = new CgCustomsStorageView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }

        /// <summary>
        /// 根据运单执行状态搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public CgCustomsStorageView SearchByStatus(params CgPickingExcuteStatus[] status)
        {
            var waybillView = this.IQueryable.Cast<MyWaybill>();
            var linq = from waybill in waybillView
                       where status.Contains(waybill.ExcuteStatus)
                       select waybill;
            var view = new CgCustomsStorageView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        string wareHouseID;

        /// <summary>
        /// 根据库房ID搜索
        /// </summary>
        /// <param name="wareHouseID"></param>
        /// <returns></returns>
        public CgCustomsStorageView SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var waybillView = this.IQueryable.Cast<MyWaybill>();

            if (string.IsNullOrWhiteSpace(this.wareHouseID))
            {
                return new CgCustomsStorageView(this.Reponsitory, waybillView);
            }

            Expression<Func<MyWaybill, bool>> express = null;
            if (wareHouseID.StartsWith(nameof(WhSettings.HK)))
            {
                express = waybill => waybill.NoticeType == CgNoticeType.Boxing && waybill.Source == CgNoticeSource.AgentCustomsFromStorage;
            }

            waybillView = waybillView.Where(express);

            var linq_waybillIDs = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                  where notice.WareHouseID.StartsWith(this.wareHouseID)
                                  select notice.WaybillID;
            var ienum_waybillIDs = linq_waybillIDs.Distinct();
            var linq = from waybill in waybillView
                       join id in ienum_waybillIDs on waybill.ID equals id
                       orderby waybill.ID descending
                       select waybill;
            var view = new CgCustomsStorageView(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };
            return view;
        }
        #endregion

        #region Helper Class
        private class MyWaybill
        {
            public string ID { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime? ModifyDate { get; set; }
            public string EnterCode { get; set; }
            public string ClientName { get; set; }           
            public CgPickingExcuteStatus ExcuteStatus { get; set; }
            public WaybillType Type { get; set; }
            public string Code { get; set; }
            public string CarrierID { get; set; }
            public string CarrierName { get; set; }            
            public string OrderID { get; set; }
            public string Packaging { get; set; }
            public CgNoticeSource Source { get; set; }
            /// <summary>
            /// 业务类型名称
            /// </summary>
            public string SourceName { get; set; }
            public CgNoticeType NoticeType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// 转运使用
            /// </remarks>
            public string TransferID { get; set; }
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
            /// 总货值
            /// </summary>
            public decimal? chgTotalPrice { get; set; }            

            /// <summary>
            ///总件数
            /// </summary>
            public int? TotalParts { get; set; }
            /// <summary>
            /// 总重量
            /// </summary>
            public decimal? TotalWeight { get; set; }            

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
