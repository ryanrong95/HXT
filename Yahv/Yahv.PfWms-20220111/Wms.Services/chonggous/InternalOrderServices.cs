using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous
{
    public class InternalOrderServices
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InternalOrderServices()
        {

        }

        /// <summary>
        /// 华芯通内单（香港入库数据）
        /// </summary>
        /// <remarks>需要修改</remarks>
        public void Enter(PvWsOrderInsApiModel model)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                //连接华芯通
                var order = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrdersTopView>()
                   .Where(item => item.ID == model.VastOrderID).FirstOrDefault();


                var items = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.OrderItemsTopView>()
                    .Where(item => item.OrderID == model.VastOrderID).ToList();

                if (order == null)
                {
                    throw new Exception("订单" + model.VastOrderID + "不存在");
                }

                using (var trans = reponsitory.OpenTransaction())
                {
                    #region 生成Inputs数据
                    var inputs = items.Select(entity => new Layers.Data.Sqls.PvWms.Inputs
                    {
                        ID = entity.ID,//InputID用内单数据的ItemID生成
                        Code = entity.ID,
                        OrderID = entity.OrderID,
                        TinyOrderID = entity.TinyOrderID,
                        ItemID = entity.ID,
                        ClientID = order.ClientID,
                        ProductID = entity.ProductID,
                        UnitPrice = entity.UnitPrice,
                        PayeeID = order.PayeeID,
                        Currency = order.inCurrency,
                        ThirdID = Yahv.Services.WhSettings.HK[model.HKWareHouseID].Enterprise.ID,//（万路通或畅运）
                        OriginID = string.Empty,
                        SalerID = string.Empty,
                        PurchaserID = string.Empty,
                        TrackerID = order.CreatorID,
                        CreateDate = DateTime.Now,
                    }).ToArray();

                    #endregion

                    #region 生成Notices数据
                    var notices = (from item in model.Items
                                   join input in inputs on item.ID equals input.ID
                                   select new Layers.Data.Sqls.PvWms.Notices()
                                   {
                                       //ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                       InputID = input.ID,
                                       ProductID = input.ProductID,
                                       DateCode = item.DateCode,
                                       Quantity = item.Quantity,
                                       Origin = item.Origin,
                                       BoxCode = item.PackNo,
                                       Weight = item.GrossWeight,
                                       NetWeight = item.NetWeight,
                                       Type = (int)CgNoticeType.Enter,
                                       OutputID = null,
                                       WareHouseID = model.HKWareHouseID,
                                       WaybillID = order.InWayBillID,
                                       Supplier = model.OrderConsignee.ClientSupplierName,
                                       Conditions = new NoticeCondition().Json(),
                                       CreateDate = DateTime.Now,
                                       Status = (int)GeneralStatus.Normal,
                                       Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                                       Target = (int)NoticesTarget.Default,
                                       Volume = 0M,
                                       ShelveID = null,
                                       BoxingSpecs = null,
                                   }).ToArray();

                    var noticeIds = reponsitory.Series(PkeyType.Notices, notices.Length);
                    notices = notices.Select((item, index) =>
                   {
                       item.ID = noticeIds[index];
                       return item;
                   }).ToArray();


                    #endregion

                    #region 生成Sortings数据
                    var sortingsIds = reponsitory.Series(PkeyType.Sortings, notices.Length);
                    var sortings = notices.Select((entity, index) => new Layers.Data.Sqls.PvWms.Sortings()
                    {
                        //ID = reponsitory.Pick(Wms.Services.PkeyType.Sortings),
                        ID = sortingsIds[index],
                        InputID = entity.InputID,
                        NoticeID = entity.ID,
                        WaybillID = model.WayBillID,
                        Quantity = entity.Quantity,
                        BoxCode = entity.BoxCode,
                        Weight = entity.Weight,
                        NetWeight = entity.NetWeight,
                        AdminID = Npc.Robot.Obtain(),
                        CreateDate = DateTime.Now,
                        Volume = 0M,
                    }).ToArray();
                    #endregion

                    #region 生成Storages数据

                    var storagesIds = reponsitory.Series(PkeyType.Storages, notices.Length);
                    var storages = (from sorting in sortings
                                    join notice in notices on sorting.NoticeID equals notice.ID
                                    select new Layers.Data.Sqls.PvWms.Storages()
                                    {
                                        //ID = reponsitory.Pick(PkeyType.Storages),
                                        InputID = sorting.InputID,
                                        SortingID = sorting.ID,
                                        Total = sorting.Quantity,
                                        Quantity = sorting.Quantity,
                                        ProductID = notice.ProductID,
                                        Origin = notice.Origin,
                                        ShelveID = null,//先做可空
                                        Supplier = notice.Supplier,
                                        DateCode = notice.DateCode,
                                        Type = (int)CgStoragesType.Flows,
                                        WareHouseID = model.HKWareHouseID,
                                        IsLock = false,
                                        CreateDate = DateTime.Now,
                                        Status = (int)GeneralStatus.Normal,
                                        Summary = string.Empty,
                                    }).ToArray();
                    storages = storages.Select((item, index) =>
                    {
                        item.ID = storagesIds[index];
                        return item;
                    }).ToArray();

                    var log_storages = notices.Select((entity, index) => new Layers.Data.Sqls.PvWms.Logs_Storage()
                    {
                        ID = Guid.NewGuid().ToString(),
                        IsCurrent = true,
                        StorageID = storagesIds[index],
                        Summary = null,
                        AdminID = Npc.Robot.Obtain(),
                        CreateDate = DateTime.Now,
                        BoxCode = entity.BoxCode,
                        Weight = entity.Weight.HasValue ? entity.Weight.Value : 0,
                    }).ToArray();
                    #endregion


                    //连接 Waybill

                    #region 生成Logs_Declare数据
                    var TinyOrderIDs = model.Items.Select(item => item.TinyOrderID).Distinct().ToArray();
                    var waybill = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == order.InWayBillID);


                    //优化
                    //疑问？这些都是新的订单项目，为什么会提前生成 申报？
                    //可能是我考虑不周

                    var list_logsDeclares = new Layers.Data.Sqls.PvWms.Logs_Declare[0];
                    var existsTinyOrderIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().
                        Where(item => TinyOrderIDs.Contains(item.TinyOrderID)).Select(item => item.TinyOrderID).ToArray();
                    var tiniesID = TinyOrderIDs.Except(existsTinyOrderIDs).ToArray();

                    if (tiniesID.Length > 0)
                    {
                        string[] logsDeclareID = reponsitory.Series(PkeyType.LogsDeclare, tiniesID.Length);
                        list_logsDeclares = tiniesID.Select((item, index) => new Layers.Data.Sqls.PvWms.Logs_Declare
                        {
                            ID = logsDeclareID[index],
                            TinyOrderID = item,
                            EnterCode = waybill.wbEnterCode,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            AdminID = order.CreatorID,
                            Status = (int)Enums.TinyOrderDeclareStatus.Declaring,
                        }).ToArray();
                    }
                    #endregion

                    #region 生成Logs_DeclareItem数据


                    var declareItemsID = reponsitory.Series(PkeyType.LogsDeclareItem, storages.Length);

                    var logs_DeclareItems = (from storage in storages
                                             join sorting in sortings on storage.SortingID equals sorting.ID
                                             join input in inputs on sorting.InputID equals input.ID
                                             select new Layers.Data.Sqls.PvWms.Logs_DeclareItem()
                                             {
                                                 //ID = reponsitory.Pick(PkeyType.LogsDeclareItem),
                                                 TinyOrderID = input.TinyOrderID,
                                                 OrderItemID = input.ItemID,
                                                 StorageID = storage.ID,
                                                 Quantity = storage.Quantity,
                                                 BoxCode = sorting.BoxCode,
                                                 AdminID = Npc.Robot.Obtain(),
                                                 Weight = sorting.Weight,
                                                 NetWeight = sorting.NetWeight,
                                                 Volume = sorting.Volume,
                                                 Status = 1,
                                             }).ToArray();

                    logs_DeclareItems = logs_DeclareItems.Select((item, index) =>
                    {
                        item.ID = declareItemsID[index];
                        return item;
                    }).ToArray();

                    #endregion

                    //Task task = new Task(() =>
                    //{
                    //插入数据
                    //reponsitory.Insert(inputs);
                    //reponsitory.Insert(notices);
                    //reponsitory.Insert(sortings);
                    //reponsitory.Insert(storages);
                    //reponsitory.Insert(log_storages);
                    //reponsitory.Insert(list_logsDeclares);
                    //reponsitory.Insert(logs_DeclareItems);

                    var tSql = reponsitory.TSql;
                    tSql.InserSync(inputs);
                    tSql.InserSync(notices.ToArray());
                    tSql.InserSync(sortings);
                    tSql.InserSync(storages);
                    tSql.InserSync(log_storages);
                    tSql.InserSync(list_logsDeclares);
                    tSql.InserSync(logs_DeclareItems);
                    //});
                    //task.Start();

                    #region 申请报关

                    List<CgDeclareApplyItem> list = logs_DeclareItems.Select(item => new CgDeclareApplyItem
                    {
                        TinyOrderID = item.TinyOrderID,
                        AdminID = order.CreatorID,
                        DeclareID = item.ID,
                    }).ToList();

                    CgDeclareApply declareApply = new CgDeclareApply();
                    declareApply.Items = list;
                    var result = Yahv.Utils.Http.ApiHelper.Current.PostData(FromType.CustomApply.GetDescription(), declareApply);
                    var message = result.JsonTo<JMessage>();
                    if (message.code != 200)
                    {
                        throw new Exception("通知华芯通申请报关失败：" + message.data);
                    }
                    #endregion

                    //Task.WaitAll(task);

                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// 根据订单ID,删除所有库房数据
        /// </summary>
        /// <param name="OrderID"></param>
        public void AbandonByOrderID(string OrderID)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                var inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => item.OrderID == OrderID);
                var inputIDs = inputs.Select(item => item.ID).ToList();
                if (inputIDs.Count > 0)
                {
                    //删除数据
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => inputIDs.Contains(item.InputID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_Declare>(item => item.TinyOrderID.Contains(OrderID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => item.TinyOrderID.Contains(OrderID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => inputIDs.Contains(item.InputID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => inputIDs.Contains(item.InputID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Inputs>(item => inputIDs.Contains(item.ID));
                }
            }
        }

        /// <summary>
        /// 根据订单项ID,删除某项数据
        /// </summary>
        /// <param name="OrderID"></param>
        public void AbandonByOrderItemID(string OrderItemID)
        {
            using (var reponsitory = new PvWmsRepository())
            {
                var inputs = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => item.ItemID == OrderItemID);
                var inputIDs = inputs.Select(item => item.ID).ToList();
                if (inputIDs.Count > 0)
                {
                    //删除数据
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Storages>(item => inputIDs.Contains(item.InputID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Logs_DeclareItem>(item => item.OrderItemID == OrderItemID);
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Sortings>(item => inputIDs.Contains(item.InputID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Notices>(item => inputIDs.Contains(item.InputID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWms.Inputs>(item => inputIDs.Contains(item.ID));
                }
            }
        }

        /// <summary>
        /// 华芯通数据贯通接口
        /// </summary>
        public void XDTOrderEnter()
        {
            using (var reponsitory = new PvWmsRepository())
            {
                var orders = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsOrdersTopView>()
                    .Where(t => t.Type == (int)OrderType.Declare && t.MainStatus != (int)CgOrderStatus.取消).ToList();
                var orderItems = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsOrderItemsTopView>();
                var waybills = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>();
                //缺少时间视图
                foreach (var order in orders)
                {
                    //订单项
                    var items = orderItems.Where(item => item.OrderID == order.ID).ToList();
                    //运单数据
                    var HK_In_Waybill = waybills.Where(item => item.wbID == order.InWayBillID).FirstOrDefault();
                    var HK_Out_Waybill = waybills.Where(item => item.chcdLotNumber == order.VoyNo && item.NoticeType == (int)CgNoticeType.Out).FirstOrDefault();
                    var SZ_In_Waybill = waybills.Where(item => item.chcdLotNumber == order.VoyNo && item.NoticeType == (int)CgNoticeType.Enter).FirstOrDefault();
                    var SZ_Out_Waybill = waybills.Where(item => item.wbID == order.OutWayBillID).FirstOrDefault();

                    #region 生成香港Inputs数据
                    var inputs = items.Select(entity => new Layers.Data.Sqls.PvWms.Inputs
                    {
                        ID = entity.ID,//InputID用内单数据的ItemID生成
                        Code = entity.ID,
                        OrderID = entity.OrderID,
                        TinyOrderID = entity.TinyOrderID,
                        ItemID = entity.ID,
                        ClientID = order.ClientID,
                        ProductID = entity.ProductID,
                        UnitPrice = entity.UnitPrice,
                        PayeeID = order.PayeeID,
                        Currency = order.inCurrency,
                        ThirdID = Yahv.Services.WhSettings.HK["HK01"].Enterprise.ID,//（万路通或畅运）
                        OriginID = string.Empty,
                        SalerID = string.Empty,
                        PurchaserID = string.Empty,
                        TrackerID = order.CreatorID,
                        CreateDate = order.CreateDate,
                    }).ToArray();

                    #endregion

                    #region 生成香港入库通知
                    var notices = (from item in items
                                   join input in inputs on item.ID equals input.ItemID
                                   select new Layers.Data.Sqls.PvWms.Notices()
                                   {
                                       ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                       InputID = input.ID,
                                       ProductID = input.ProductID,
                                       DateCode = item.DateCode,
                                       Quantity = item.Quantity,
                                       Origin = item.Origin,
                                       CustomsName = item.CustomName,
                                       BoxCode = item.BoxCode,
                                       Weight = item.GrossWeight,
                                       NetWeight = item.NetWeight,
                                       Type = (int)CgNoticeType.Enter,
                                       OutputID = null,
                                       WareHouseID = Yahv.Services.WhSettings.HK["HK01"].ID,
                                       WaybillID = order.InWayBillID,
                                       Supplier = HK_In_Waybill.wbSupplier,
                                       Conditions = new NoticeCondition().Json(),
                                       CreateDate = order.CreateDate,
                                       Status = (int)GeneralStatus.Normal,
                                       Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                                       Target = (int)NoticesTarget.Default,
                                       Volume = 0M,
                                       ShelveID = null,
                                       BoxingSpecs = null,
                                   }).ToArray();

                    #endregion

                    #region 生成香港Sortings数据 
                    var sortings = (from notice in notices
                                    join item in items on notice.InputID equals item.ID
                                    select new Layers.Data.Sqls.PvWms.Sortings()
                                    {
                                        ID = Layers.Data.PKeySigner.Pick(PkeyType.Sortings),
                                        InputID = notice.InputID,
                                        NoticeID = notice.ID,
                                        WaybillID = HK_In_Waybill.wbID,
                                        Quantity = notice.Quantity,
                                        BoxCode = notice.BoxCode,
                                        Weight = notice.Weight,
                                        NetWeight = notice.NetWeight,
                                        AdminID = Npc.Robot.Obtain(),
                                        CreateDate = item.hkSortingDate ?? order.CreateDate,
                                        Volume = 0M,
                                    }).ToArray();
                    #endregion

                    #region 生成香港Storages数据
                    var storages = (from sorting in sortings
                                    join notice in notices on sorting.NoticeID equals notice.ID
                                    select new Layers.Data.Sqls.PvWms.Storages()
                                    {
                                        ID = Layers.Data.PKeySigner.Pick(PkeyType.Storages),
                                        InputID = sorting.InputID,
                                        SortingID = sorting.ID,
                                        Total = sorting.Quantity,
                                        Quantity = 0m,//应该给0
                                        ProductID = notice.ProductID,
                                        Origin = notice.Origin,
                                        ShelveID = null,//先做可空
                                        Supplier = notice.Supplier,
                                        DateCode = notice.DateCode,
                                        Type = (int)CgStoragesType.Flows,
                                        WareHouseID = notice.WareHouseID,
                                        IsLock = false,
                                        CreateDate = sorting.CreateDate,
                                        Status = (int)GeneralStatus.Normal,
                                        Summary = string.Empty,
                                        CustomsName = notice.CustomsName,
                                    }).ToArray();
                    #endregion

                    #region 生成Logs_Declare数据
                    var TinyOrderIDs = items.Select(item => item.TinyOrderID).Distinct();
                    var waybill = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == order.InWayBillID);

                    var logs_Declares = new List<Layers.Data.Sqls.PvWms.Logs_Declare>();
                    foreach (var tinyId in TinyOrderIDs)
                    {
                        bool exist = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().Any(item => item.TinyOrderID == tinyId);
                        if (!exist)
                        {
                            var logs_Declare = new Layers.Data.Sqls.PvWms.Logs_Declare()
                            {
                                ID = Layers.Data.PKeySigner.Pick(PkeyType.LogsDeclare),
                                TinyOrderID = tinyId,
                                EnterCode = waybill.wbEnterCode,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                AdminID = order.CreatorID,
                                Status = (int)Enums.TinyOrderDeclareStatus.Declaring,
                            };
                            logs_Declares.Add(logs_Declare);
                        }
                    }
                    #endregion

                    #region 生成Logs_DeclareItem数据

                    var logs_DeclareItems = (from storage in storages
                                             join sorting in sortings on storage.SortingID equals sorting.ID
                                             join input in inputs on sorting.InputID equals input.ID
                                             select new Layers.Data.Sqls.PvWms.Logs_DeclareItem()
                                             {
                                                 ID = Layers.Data.PKeySigner.Pick(PkeyType.LogsDeclareItem),
                                                 TinyOrderID = input.TinyOrderID,
                                                 OrderItemID = input.ItemID,
                                                 StorageID = storage.ID,
                                                 Quantity = storage.Quantity,
                                                 BoxCode = sorting.BoxCode,
                                                 AdminID = Npc.Robot.Obtain(),
                                                 Weight = sorting.Weight,
                                                 NetWeight = sorting.NetWeight,
                                                 Volume = sorting.Volume,
                                                 Status = 1,
                                             }).ToArray();
                    #endregion

                    #region 生成香港Outputs数据
                    var hk_Outputs = (from input in inputs
                                      join item in items on input.ItemID equals item.ID
                                      select new Layers.Data.Sqls.PvWms.Outputs
                                      {
                                          ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                                          InputID = input.ID,
                                          OrderID = input.OrderID,
                                          TinyOrderID = input.TinyOrderID,
                                          ItemID = input.ItemID,
                                          OwnerID = string.Empty,
                                          PurchaserID = string.Empty,
                                          Currency = input.Currency,
                                          Price = ((decimal)input.UnitPrice * 1.002m),
                                          CreateDate = item.hkExitDate ?? order.CreateDate,
                                          ReviewerID = string.Empty,
                                          TrackerID = order.CreatorID,
                                      }).ToArray();
                    #endregion

                    #region 生成香港Notices出库通知
                    var hk_OutNotices = (from output in hk_Outputs
                                         join input in inputs on output.InputID equals input.ID
                                         join sorting in sortings on input.ID equals sorting.InputID
                                         join storage in storages on input.ID equals storage.InputID
                                         select new Layers.Data.Sqls.PvWms.Notices()
                                         {
                                             ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                             Type = (int)CgNoticeType.Out,
                                             WareHouseID = Yahv.Services.WhSettings.HK["HK01"].ID,
                                             WaybillID = HK_Out_Waybill?.wbID ?? "",
                                             InputID = input.ID,
                                             OutputID = output.ID,
                                             ProductID = storage.ProductID,
                                             DateCode = storage.DateCode,
                                             Origin = storage.Origin,
                                             CustomsName = storage.CustomsName,
                                             Quantity = (decimal)storage.Total,
                                             Conditions = new NoticeCondition().Json(),
                                             Status = (int)NoticesStatus.Completed,
                                             Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                                             Target = (int)NoticesTarget.Default,
                                             Weight = sorting.Weight,
                                             NetWeight = sorting.NetWeight,
                                             Volume = 0m,
                                             BoxCode = sorting.BoxCode,
                                             ShelveID = storage.ShelveID,
                                             BoxingSpecs = null,
                                             CreateDate = output.CreateDate,
                                             StorageID = storage.ID,
                                         }).ToArray();
                    #endregion

                    #region 生成香港的Picking数据
                    var hk_Pickings = (from notice in hk_OutNotices
                                       join output in hk_Outputs on notice.OutputID equals output.ID
                                       select new Layers.Data.Sqls.PvWms.Pickings
                                       {
                                           ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                                           StorageID = notice.StorageID,
                                           NoticeID = notice.ID,
                                           OutputID = notice.OutputID,
                                           BoxCode = notice.BoxCode,
                                           Quantity = notice.Quantity,
                                           AdminID = Npc.Robot.Obtain(),
                                           CreateDate = notice.CreateDate,
                                           Weight = notice.Weight,
                                           NetWeight = notice.NetWeight,
                                           Volume = notice.Volume,
                                       }).ToArray();
                    #endregion

                    #region 生成深圳Inputs数据
                    var sz_Inputs = (from output in hk_Outputs
                                     join input in inputs on output.InputID equals input.ID
                                     select new Layers.Data.Sqls.PvWms.Inputs
                                     {
                                         ID = Layers.Data.PKeySigner.Pick(PkeyType.Inputs),
                                         Code = string.Empty,
                                         OrderID = input.OrderID,
                                         TinyOrderID = input.TinyOrderID,
                                         ItemID = input.ItemID,
                                         ClientID = input.ClientID,
                                         PayeeID = order.PayeeID,
                                         ThirdID = Yahv.Services.WhSettings.SZ["SZ01"].Enterprise.ID,//（华芯通）
                                         ProductID = input.ProductID,
                                         UnitPrice = output.Price,//TODO:需要更新
                                         Currency = (int)Currency.CNY,
                                         OriginID = string.Empty,
                                         SalerID = string.Empty,
                                         PurchaserID = string.Empty,
                                         TrackerID = order.CreatorID,
                                         CreateDate = output.CreateDate,
                                     }).ToArray();

                    #endregion

                    #region 生成深圳Notices入库通知
                    var sz_Notices = (from item in items
                                      join input in sz_Inputs on item.ID equals input.ItemID
                                      select new Layers.Data.Sqls.PvWms.Notices()
                                      {
                                          ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                          InputID = input.ID,
                                          ProductID = input.ProductID,
                                          DateCode = item.DateCode,
                                          Quantity = item.Quantity,
                                          Origin = item.Origin,
                                          CustomsName = item.CustomName,
                                          BoxCode = item.BoxCode,
                                          Weight = item.GrossWeight,
                                          NetWeight = item.NetWeight,
                                          Type = (int)CgNoticeType.Enter,
                                          OutputID = null,
                                          WareHouseID = Yahv.Services.WhSettings.SZ["SZ01"].ID,//华芯通
                                          WaybillID = SZ_In_Waybill?.wbID ?? "",
                                          Supplier = HK_In_Waybill.wbSupplier,
                                          Conditions = new NoticeCondition().Json(),
                                          CreateDate = input.CreateDate,
                                          Status = (int)GeneralStatus.Normal,
                                          Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                                          Target = (int)NoticesTarget.Default,
                                          Volume = 0M,
                                          ShelveID = null,
                                          BoxingSpecs = null,
                                      }).ToArray();

                    #endregion

                    #region 生成深圳Sortings数据
                    var sz_Sortings = (from notice in sz_Notices
                                       join input in sz_Inputs on notice.InputID equals input.ID
                                       join item in items on input.ItemID equals item.ID
                                       select new Layers.Data.Sqls.PvWms.Sortings
                                       {
                                           ID = Layers.Data.PKeySigner.Pick(PkeyType.Sortings),
                                           InputID = notice.InputID,
                                           NoticeID = notice.ID,
                                           WaybillID = SZ_In_Waybill?.wbID ?? "",
                                           Quantity = notice.Quantity,
                                           BoxCode = notice.BoxCode,
                                           Weight = notice.Weight,
                                           NetWeight = notice.NetWeight,
                                           Volume = 0M,
                                           AdminID = Npc.Robot.Obtain(),
                                           CreateDate = item.szSortingDate ?? order.CreateDate,
                                       }).ToArray();

                    #endregion

                    #region 生成深圳Storages数据
                    var sz_Storages = (from sorting in sz_Sortings
                                       join notice in sz_Notices on sorting.NoticeID equals notice.ID
                                       select new Layers.Data.Sqls.PvWms.Storages()
                                       {
                                           ID = Layers.Data.PKeySigner.Pick(PkeyType.Storages),
                                           InputID = sorting.InputID,
                                           SortingID = sorting.ID,
                                           Total = sorting.Quantity,
                                           Quantity = 0m,//应该给0
                                           ProductID = notice.ProductID,
                                           Origin = notice.Origin,
                                           CustomsName = notice.CustomsName,
                                           ShelveID = null,//先做可空
                                           Supplier = notice.Supplier,
                                           DateCode = notice.DateCode,
                                           Type = (int)CgStoragesType.Flows,
                                           WareHouseID = notice.WareHouseID,
                                           IsLock = false,
                                           CreateDate = sorting.CreateDate,
                                           Status = (int)GeneralStatus.Normal,
                                           Summary = string.Empty,
                                       }).ToArray();
                    #endregion

                    #region 生成深圳Outputs数据
                    var sz_Outputs = (from input in sz_Inputs
                                      join item in items on input.ItemID equals item.ID
                                      select new Layers.Data.Sqls.PvWms.Outputs
                                      {
                                          ID = Layers.Data.PKeySigner.Pick(PkeyType.Outputs),
                                          InputID = input.ID,
                                          OrderID = input.OrderID,
                                          TinyOrderID = input.TinyOrderID,
                                          ItemID = input.ItemID,
                                          OwnerID = string.Empty,
                                          PurchaserID = string.Empty,
                                          Currency = input.Currency,
                                          Price = input.UnitPrice,//TODO:需要跟新
                                          CreateDate = item.szExitDate ?? order.CreateDate,
                                          ReviewerID = string.Empty,
                                          TrackerID = order.CreatorID,
                                      }).ToArray();
                    #endregion

                    #region 生成深圳Notices出库通知
                    var sz_OutNotices = (from output in sz_Outputs
                                         join input in sz_Inputs on output.InputID equals input.ID
                                         join sorting in sz_Sortings on input.ID equals sorting.InputID
                                         join storage in sz_Storages on input.ID equals storage.InputID
                                         select new Layers.Data.Sqls.PvWms.Notices()
                                         {
                                             ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices),
                                             Type = (int)CgNoticeType.Out,
                                             WareHouseID = Yahv.Services.WhSettings.SZ["SZ01"].ID,
                                             WaybillID = SZ_Out_Waybill?.wbID ?? "",
                                             InputID = input.ID,
                                             OutputID = output.ID,
                                             ProductID = storage.ProductID,
                                             DateCode = storage.DateCode,
                                             Origin = storage.Origin,
                                             CustomsName = storage.CustomsName,
                                             Quantity = (decimal)storage.Total,
                                             Conditions = new NoticeCondition().Json(),
                                             Status = (int)NoticesStatus.Completed,
                                             Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                                             Target = (int)NoticesTarget.Default,
                                             Weight = sorting.Weight,
                                             NetWeight = sorting.NetWeight,
                                             Volume = 0m,
                                             BoxCode = sorting.BoxCode,
                                             ShelveID = storage.ShelveID,
                                             BoxingSpecs = null,
                                             CreateDate = output.CreateDate,
                                             StorageID = storage.ID,
                                         }).ToArray();

                    #endregion

                    #region 生成深圳的Picking数据
                    var sz_Pickings = (from notice in sz_OutNotices
                                       join output in sz_Outputs on notice.OutputID equals output.ID
                                       select new Layers.Data.Sqls.PvWms.Pickings
                                       {
                                           ID = Layers.Data.PKeySigner.Pick(PkeyType.Pickings),
                                           StorageID = notice.StorageID,
                                           NoticeID = notice.ID,
                                           OutputID = notice.OutputID,
                                           BoxCode = notice.BoxCode,
                                           Quantity = notice.Quantity,
                                           AdminID = Npc.Robot.Obtain(),
                                           CreateDate = notice.CreateDate,
                                           Weight = notice.Weight,
                                           NetWeight = notice.NetWeight,
                                           Volume = notice.Volume,
                                       }).ToArray();
                    #endregion

                    AllData data = new AllData
                    {
                        HK_Inputs = inputs,
                        HK_EnterNotices = notices,
                        HK_Sortings = sortings,
                        HK_Storages = storages,
                        logs_Declares = logs_Declares.ToArray(),
                        Logs_DeclareItem = logs_DeclareItems,
                        HK_Outputs = hk_Outputs,
                        HK_OutNotices = hk_OutNotices,
                        HK_Pickings = hk_Pickings,

                        SZ_Inputs = sz_Inputs,
                        SZ_EnterNotices = sz_Notices,
                        SZ_Sortings = sz_Sortings,
                        SZ_Storages = sz_Storages,
                        SZ_Outputs = sz_Outputs,
                        SZ_OutNotices = sz_OutNotices,
                        SZ_Pickings = sz_Pickings,
                    };
                    //用线程池执行无参数方法
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(data.Save));
                }
            }
        }

        private class AllData
        {
            public Layers.Data.Sqls.PvWms.Inputs[] HK_Inputs { get; set; }
            public Layers.Data.Sqls.PvWms.Notices[] HK_EnterNotices { get; set; }
            public Layers.Data.Sqls.PvWms.Sortings[] HK_Sortings { get; set; }
            public Layers.Data.Sqls.PvWms.Storages[] HK_Storages { get; set; }
            public Layers.Data.Sqls.PvWms.Logs_Declare[] logs_Declares { get; set; }
            public Layers.Data.Sqls.PvWms.Logs_DeclareItem[] Logs_DeclareItem { get; set; }
            public Layers.Data.Sqls.PvWms.Outputs[] HK_Outputs { get; set; }
            public Layers.Data.Sqls.PvWms.Notices[] HK_OutNotices { get; set; }
            public Layers.Data.Sqls.PvWms.Pickings[] HK_Pickings { get; set; }

            public Layers.Data.Sqls.PvWms.Inputs[] SZ_Inputs { get; set; }
            public Layers.Data.Sqls.PvWms.Notices[] SZ_EnterNotices { get; set; }
            public Layers.Data.Sqls.PvWms.Sortings[] SZ_Sortings { get; set; }
            public Layers.Data.Sqls.PvWms.Storages[] SZ_Storages { get; set; }
            public Layers.Data.Sqls.PvWms.Outputs[] SZ_Outputs { get; set; }
            public Layers.Data.Sqls.PvWms.Notices[] SZ_OutNotices { get; set; }
            public Layers.Data.Sqls.PvWms.Pickings[] SZ_Pickings { get; set; }

            public void Save(object state)
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    reponsitory.Insert(HK_Inputs);
                    reponsitory.Insert(HK_EnterNotices);
                    reponsitory.Insert(HK_Sortings);
                    reponsitory.Insert(HK_Storages);
                    reponsitory.Insert(logs_Declares);
                    reponsitory.Insert(Logs_DeclareItem);
                    reponsitory.Insert(HK_Outputs);
                    reponsitory.Insert(HK_OutNotices);
                    reponsitory.Insert(HK_Pickings);

                    reponsitory.Insert(SZ_Inputs);
                    reponsitory.Insert(SZ_EnterNotices);
                    reponsitory.Insert(SZ_Sortings);
                    reponsitory.Insert(SZ_Storages);
                    reponsitory.Insert(SZ_Outputs);
                    reponsitory.Insert(SZ_OutNotices);
                    reponsitory.Insert(SZ_Pickings);
                }
            }
        }
    }
}
