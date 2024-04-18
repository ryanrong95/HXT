using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.XdtData.Import.Connections;
using Yahv.XdtData.Import.Enums;
using Yahv.XdtData.Import.Extends;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 库房数据导入
    /// </summary>
    public sealed class PvWmsDataService : IDataService
    {
        private string[] xdtMainOrderIDs;
        private Dictionary<string, string> mapsWaybill;

        public PvWmsDataService(Dictionary<string, string> mapsWaybill, params string[] mainOrderIDs)
        {
            this.xdtMainOrderIDs = mainOrderIDs;
            this.mapsWaybill = mapsWaybill;
        }

        public IDataService Query()
        {
            throw new NotImplementedException();
        }

        public IDataService Encapsule()
        {
            throw new NotImplementedException();
        }

        public void Enter()
        {
            using (var reponsitory = new PvWmsRepository())
            {
                var orders = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsOrdersTopView>()
                    .Where(t => xdtMainOrderIDs.Contains(t.ID)).ToArray();
                var orderItems = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsOrderItemsTopView>()
                    .Where(item => xdtMainOrderIDs.Contains(item.OrderID)).ToArray();
                var orderItemIDs = orderItems.Select(item => item.ID).ToArray();
                var waybills = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                        .Where(item => orders.Select(o => o.InWayBillID).ToArray().Contains(item.wbID) ||
                                       orders.Select(o => o.VoyNo).ToArray().Contains(item.chcdLotNumber)).ToArray();
                var szOutItems = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgPvWmsSzOutItemsTopView>()
                    .Where(item => orderItemIDs.Contains(item.OrderItemID)).ToArray();

                foreach (var order in orders)
                {
                    //订单项
                    var items = orderItems.Where(item => item.OrderID == order.ID).ToArray();
                    //运单数据
                    var HK_In_Waybill = waybills.FirstOrDefault(item => item.wbID == order.InWayBillID);
                    var HK_Out_Waybill = waybills.FirstOrDefault(item => item.chcdLotNumber == order.VoyNo && item.NoticeType == (int)CgNoticeType.Out);
                    var SZ_In_Waybill = waybills.FirstOrDefault(item => item.chcdLotNumber == order.VoyNo && item.NoticeType == (int)CgNoticeType.Enter);

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

                    var serirs = PkeyType.Notices.Pick(items.Length);
                    var notices = (from item in items
                                   join input in inputs on item.ID equals input.ItemID
                                   select new Layers.Data.Sqls.PvWms.Notices()
                                   {
                                       ID = null,
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
                                   }).Select((item, index) => new Layers.Data.Sqls.PvWms.Notices()
                                   {
                                       ID = serirs[index],
                                       InputID = item.InputID,
                                       ProductID = item.ProductID,
                                       //DateCode = item.DateCode,
                                       Quantity = item.Quantity,
                                       Origin = item.Origin,
                                       CustomsName = item.CustomsName,
                                       BoxCode = item.BoxCode,
                                       Weight = item.Weight,
                                       NetWeight = item.NetWeight,
                                       Type = item.Type,
                                       OutputID = item.OutputID,
                                       WareHouseID = item.WareHouseID,
                                       WaybillID = item.WaybillID,
                                       Supplier = item.Supplier,
                                       Conditions = item.Conditions,
                                       CreateDate = item.CreateDate,
                                       Status = item.Status,
                                       Source = item.Source,
                                       Target = item.Target,
                                       Volume = item.Volume,
                                       ShelveID = item.ShelveID,
                                       BoxingSpecs = item.BoxingSpecs,
                                   }).ToArray();

                    #endregion

                    #region 生成香港Sortings数据 

                    serirs = PkeyType.Sortings.Pick(items.Length);
                    var sortings = (from notice in notices
                                    join item in items on notice.InputID equals item.ID
                                    select new Layers.Data.Sqls.PvWms.Sortings
                                    {
                                        ID = null,
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
                                    }).Select((item, index) => new Layers.Data.Sqls.PvWms.Sortings
                                    {
                                        ID = serirs[index],
                                        InputID = item.InputID,
                                        NoticeID = item.NoticeID,
                                        WaybillID = HK_In_Waybill.wbID,
                                        Quantity = item.Quantity,
                                        BoxCode = item.BoxCode,
                                        Weight = item.Weight,
                                        NetWeight = item.NetWeight,
                                        AdminID = item.AdminID,
                                        CreateDate = item.CreateDate,
                                        Volume = item.Volume,
                                    }).ToArray();

                    #endregion

                    #region 生成香港Storages数据

                    serirs = PkeyType.Storages.Pick(items.Length);
                    var storages = (from sorting in sortings
                                    join notice in notices on sorting.NoticeID equals notice.ID
                                    select new Layers.Data.Sqls.PvWms.Storages()
                                    {
                                        ID = null,
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
                                    }).Select((item, index) => new Layers.Data.Sqls.PvWms.Storages()
                                    {
                                        ID = serirs[index],
                                        InputID = item.InputID,
                                        SortingID = item.SortingID,
                                        Total = item.Total,
                                        Quantity = item.Quantity,//应该给0
                                        ProductID = item.ProductID,
                                        Origin = item.Origin,
                                        ShelveID = item.ShelveID,//先做可空
                                        Supplier = item.Supplier,
                                        //DateCode = item.DateCode,
                                        Type = item.Type,
                                        WareHouseID = item.WareHouseID,
                                        IsLock = item.IsLock,
                                        CreateDate = item.CreateDate,
                                        Status = item.Status,
                                        Summary = item.Summary,
                                        CustomsName = item.CustomsName,
                                    }).ToArray();

                    #endregion

                    #region 生成Logs_Declare数据

                    var TinyOrderIDs = items.Select(item => item.TinyOrderID).Distinct().ToArray();
                    var waybill = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Single(item => item.wbID == order.InWayBillID);
                    serirs = PkeyType.LogsDeclare.Pick(TinyOrderIDs.Length);
                    var logs_Declares = new List<Layers.Data.Sqls.PvWms.Logs_Declare>();
                    for (int index = 0; index < TinyOrderIDs.Length; index++)
                    {
                        string tinyId = TinyOrderIDs[index];
                        bool exist = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Declare>().Any(item => item.TinyOrderID == tinyId);
                        if (!exist)
                        {
                            var logs_Declare = new Layers.Data.Sqls.PvWms.Logs_Declare()
                            {
                                ID = serirs[index],
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

                    serirs = PkeyType.LogsDeclareItem.Pick(items.Length);
                    var logs_DeclareItems = (from storage in storages
                                             join sorting in sortings on storage.SortingID equals sorting.ID
                                             join input in inputs on sorting.InputID equals input.ID
                                             select new Layers.Data.Sqls.PvWms.Logs_DeclareItem()
                                             {
                                                 ID = null,
                                                 TinyOrderID = input.TinyOrderID,
                                                 OrderItemID = input.ItemID,
                                                 StorageID = storage.ID,
                                                 Quantity = storage.Total.GetValueOrDefault(),
                                                 BoxCode = sorting.BoxCode,
                                                 AdminID = Npc.Robot.Obtain(),
                                                 Weight = sorting.Weight,
                                                 NetWeight = sorting.NetWeight,
                                                 Volume = sorting.Volume,
                                             }).Select((item, index) => new Layers.Data.Sqls.PvWms.Logs_DeclareItem()
                                             {
                                                 ID = serirs[index],
                                                 TinyOrderID = item.TinyOrderID,
                                                 OrderItemID = item.OrderItemID,
                                                 StorageID = item.StorageID,
                                                 Quantity = item.Quantity,
                                                 BoxCode = item.BoxCode,
                                                 AdminID = item.AdminID,
                                                 Weight = item.Weight,
                                                 NetWeight = item.NetWeight,
                                                 Volume = item.Volume,
                                             }).ToArray();

                    #endregion

                    #region 生成香港Outputs数据

                    serirs = PkeyType.Outputs.Pick(items.Length);
                    var hk_Outputs = (from input in inputs
                                      join item in items on input.ItemID equals item.ID
                                      select new Layers.Data.Sqls.PvWms.Outputs
                                      {
                                          ID = null,
                                          InputID = input.ID,
                                          OrderID = input.OrderID,
                                          TinyOrderID = input.TinyOrderID,
                                          ItemID = input.ItemID,
                                          OwnerID = string.Empty,
                                          PurchaserID = string.Empty,
                                          Currency = input.Currency,
                                          Price = ((decimal)input.UnitPrice * 1.002m),
                                          CreateDate = item.hkExitDate ?? item.declareDate ?? order.CreateDate,
                                          ReviewerID = string.Empty,
                                          TrackerID = order.CreatorID,
                                      }).Select((item, index) => new Layers.Data.Sqls.PvWms.Outputs()
                                      {
                                          ID = serirs[index],
                                          InputID = item.InputID,
                                          OrderID = item.OrderID,
                                          TinyOrderID = item.TinyOrderID,
                                          ItemID = item.ItemID,
                                          OwnerID = item.OwnerID,
                                          PurchaserID = item.PurchaserID,
                                          Currency = item.Currency,
                                          Price = item.Price,
                                          CreateDate = item.CreateDate,
                                          ReviewerID = item.ReviewerID,
                                          TrackerID = item.TrackerID,
                                      }).ToArray();

                    #endregion

                    #region 生成香港Notices出库通知

                    serirs = PkeyType.Notices.Pick(items.Length);
                    var hk_OutNotices = (from output in hk_Outputs
                                         join input in inputs on output.InputID equals input.ID
                                         join sorting in sortings on input.ID equals sorting.InputID
                                         join storage in storages on input.ID equals storage.InputID
                                         select new Layers.Data.Sqls.PvWms.Notices()
                                         {
                                             ID = null,
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
                                         }).Select((item, index) => new Layers.Data.Sqls.PvWms.Notices()
                                         {
                                             ID = serirs[index],
                                             Type = item.Type,
                                             WareHouseID = item.WareHouseID,
                                             WaybillID = item.WaybillID,
                                             InputID = item.InputID,
                                             OutputID = item.OutputID,
                                             ProductID = item.ProductID,
                                             //DateCode = item.DateCode,
                                             Origin = item.Origin,
                                             CustomsName = item.CustomsName,
                                             Quantity = item.Quantity,
                                             Conditions = item.Conditions,
                                             Status = item.Status,
                                             Source = item.Source,
                                             Target = item.Target,
                                             Weight = item.Weight,
                                             NetWeight = item.NetWeight,
                                             Volume = item.Volume,
                                             BoxCode = item.BoxCode,
                                             ShelveID = item.ShelveID,
                                             BoxingSpecs = item.BoxingSpecs,
                                             CreateDate = item.CreateDate,
                                             StorageID = item.StorageID,
                                         }).ToArray();

                    #endregion

                    #region 生成香港的Picking数据

                    serirs = PkeyType.Pickings.Pick(items.Length);
                    var hk_Pickings = (from notice in hk_OutNotices
                                       join output in hk_Outputs on notice.OutputID equals output.ID
                                       select new Layers.Data.Sqls.PvWms.Pickings
                                       {
                                           ID = null,
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
                                       }).Select((item, index) => new Layers.Data.Sqls.PvWms.Pickings()
                                       {
                                           ID = serirs[index],
                                           StorageID = item.StorageID,
                                           NoticeID = item.NoticeID,
                                           OutputID = item.OutputID,
                                           BoxCode = item.BoxCode,
                                           Quantity = item.Quantity,
                                           AdminID = item.AdminID,
                                           CreateDate = item.CreateDate,
                                           Weight = item.Weight,
                                           NetWeight = item.NetWeight,
                                           Volume = item.Volume,
                                       }).ToArray();

                    #endregion

                    #region 生成深圳Inputs数据

                    serirs = PkeyType.Inputs.Pick(items.Length);
                    var sz_Inputs = (from output in hk_Outputs
                                     join input in inputs on output.InputID equals input.ID
                                     select new Layers.Data.Sqls.PvWms.Inputs
                                     {
                                         ID = null,
                                         Code = string.Empty,
                                         OrderID = input.OrderID,
                                         TinyOrderID = input.TinyOrderID,
                                         ItemID = input.ItemID,
                                         ClientID = input.ClientID,
                                         PayeeID = order.PayeeID,
                                         ThirdID = Yahv.Services.WhSettings.SZ["SZ01"].Enterprise.ID,//（芯达通）
                                         ProductID = input.ProductID,
                                         UnitPrice = output.Price,//TODO:需要更新
                                         Currency = (int)Currency.CNY,
                                         OriginID = string.Empty,
                                         SalerID = string.Empty,
                                         PurchaserID = string.Empty,
                                         TrackerID = order.CreatorID,
                                         CreateDate = output.CreateDate,
                                     }).Select((item, index) => new Layers.Data.Sqls.PvWms.Inputs()
                                     {
                                         ID = serirs[index],
                                         Code = item.Code,
                                         OrderID = item.OrderID,
                                         TinyOrderID = item.TinyOrderID,
                                         ItemID = item.ItemID,
                                         ClientID = item.ClientID,
                                         PayeeID = item.PayeeID,
                                         ThirdID = item.ThirdID,
                                         ProductID = item.ProductID,
                                         UnitPrice = item.UnitPrice,
                                         Currency = item.Currency,
                                         OriginID = item.OriginID,
                                         SalerID = item.SalerID,
                                         PurchaserID = item.PurchaserID,
                                         TrackerID = item.TrackerID,
                                         CreateDate = item.CreateDate,
                                     }).ToArray();

                    #endregion

                    #region 生成深圳Notices入库通知

                    serirs = PkeyType.Notices.Pick(items.Length);
                    var sz_Notices = (from item in items
                                      join input in sz_Inputs on item.ID equals input.ItemID
                                      select new Layers.Data.Sqls.PvWms.Notices()
                                      {
                                          ID = null,
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
                                          WareHouseID = Yahv.Services.WhSettings.SZ["SZ01"].ID,//芯达通
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
                                      }).Select((item, index) => new Layers.Data.Sqls.PvWms.Notices()
                                      {
                                          ID = serirs[index],
                                          InputID = item.InputID,
                                          ProductID = item.ProductID,
                                          //DateCode = item.DateCode,
                                          Quantity = item.Quantity,
                                          Origin = item.Origin,
                                          CustomsName = item.CustomsName,
                                          BoxCode = item.BoxCode,
                                          Weight = item.Weight,
                                          NetWeight = item.NetWeight,
                                          Type = item.Type,
                                          OutputID = item.OutputID,
                                          WareHouseID = item.WareHouseID,
                                          WaybillID = item.WaybillID,
                                          Supplier = item.Supplier,
                                          Conditions = item.Conditions,
                                          CreateDate = item.CreateDate,
                                          Status = item.Status,
                                          Source = item.Source,
                                          Target = item.Target,
                                          Volume = item.Volume,
                                          ShelveID = item.ShelveID,
                                          BoxingSpecs = item.BoxingSpecs,
                                      }).ToArray();

                    #endregion

                    #region 生成深圳Sortings数据

                    serirs = PkeyType.Sortings.Pick(items.Length);
                    var sz_Sortings = (from notice in sz_Notices
                                       join input in sz_Inputs on notice.InputID equals input.ID
                                       join item in items on input.ItemID equals item.ID
                                       select new Layers.Data.Sqls.PvWms.Sortings
                                       {
                                           ID = null,
                                           InputID = notice.InputID,
                                           NoticeID = notice.ID,
                                           WaybillID = SZ_In_Waybill?.wbID ?? "",
                                           Quantity = notice.Quantity,
                                           BoxCode = notice.BoxCode,
                                           Weight = notice.Weight,
                                           NetWeight = notice.NetWeight,
                                           Volume = 0M,
                                           AdminID = Npc.Robot.Obtain(),
                                           CreateDate = item.szSortingDate ?? item.declareDate ?? order.CreateDate,
                                       }).Select((item, index) => new Layers.Data.Sqls.PvWms.Sortings()
                                       {
                                           ID = serirs[index],
                                           InputID = item.InputID,
                                           NoticeID = item.NoticeID,
                                           WaybillID = item.WaybillID,
                                           Quantity = item.Quantity,
                                           BoxCode = item.BoxCode,
                                           Weight = item.Weight,
                                           NetWeight = item.NetWeight,
                                           Volume = item.Volume,
                                           AdminID = item.AdminID,
                                           CreateDate = item.CreateDate,
                                       }).ToArray();

                    #endregion

                    #region 生成深圳Storages数据

                    serirs = PkeyType.Storages.Pick(items.Length);
                    var sz_Storages = (from sorting in sz_Sortings
                                       join notice in sz_Notices on sorting.NoticeID equals notice.ID
                                       select new Layers.Data.Sqls.PvWms.Storages()
                                       {
                                           ID = null,
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
                                       }).Select((item, index) => new Layers.Data.Sqls.PvWms.Storages()
                                       {
                                           ID = serirs[index],
                                           InputID = item.InputID,
                                           SortingID = item.SortingID,
                                           Total = item.Total,
                                           Quantity = item.Quantity,
                                           ProductID = item.ProductID,
                                           Origin = item.Origin,
                                           CustomsName = item.CustomsName,
                                           ShelveID = item.ShelveID,
                                           Supplier = item.Supplier,
                                           //DateCode = item.DateCode,
                                           Type = item.Type,
                                           WareHouseID = item.WareHouseID,
                                           IsLock = item.IsLock,
                                           CreateDate = item.CreateDate,
                                           Status = item.Status,
                                           Summary = item.Summary,
                                       }).ToArray();

                    #endregion

                    #region 生成深圳Outputs数据

                    serirs = PkeyType.Outputs.Pick(items.Length);
                    var sz_Outputs = (from input in sz_Inputs
                                      join item in items on input.ItemID equals item.ID
                                      select new Layers.Data.Sqls.PvWms.Outputs
                                      {
                                          ID = null,
                                          InputID = input.ID,
                                          OrderID = input.OrderID,
                                          TinyOrderID = input.TinyOrderID,
                                          ItemID = input.ItemID,
                                          OwnerID = string.Empty,
                                          PurchaserID = string.Empty,
                                          Currency = input.Currency,
                                          Price = input.UnitPrice,//TODO:需要跟新
                                          CreateDate = item.szExitDate ?? item.declareDate ?? order.CreateDate,
                                          ReviewerID = string.Empty,
                                          TrackerID = order.CreatorID,
                                      }).Select((item, index) => new Layers.Data.Sqls.PvWms.Outputs()
                                      {
                                          ID = serirs[index],
                                          InputID = item.InputID,
                                          OrderID = item.OrderID,
                                          TinyOrderID = item.TinyOrderID,
                                          ItemID = item.ItemID,
                                          OwnerID = item.OwnerID,
                                          PurchaserID = item.PurchaserID,
                                          Currency = item.Currency,
                                          Price = item.Price,
                                          CreateDate = item.CreateDate,
                                          ReviewerID = item.ReviewerID,
                                          TrackerID = item.TrackerID,
                                      }).ToArray();
                    #endregion

                    #region 生成深圳Notices出库通知

                    serirs = PkeyType.Notices.Pick(items.Length);
                    var sz_OutNotices = (from output in sz_Outputs
                                         join input in sz_Inputs on output.InputID equals input.ID
                                         join sorting in sz_Sortings on input.ID equals sorting.InputID
                                         join storage in sz_Storages on input.ID equals storage.InputID
                                         select new Layers.Data.Sqls.PvWms.Notices()
                                         {
                                             ID = null,
                                             Type = (int)CgNoticeType.Out,
                                             WareHouseID = Yahv.Services.WhSettings.SZ["SZ01"].ID,
                                             WaybillID = GetWaybillID(input.ItemID, mapsWaybill, order.ID, szOutItems),
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
                                         }).Select((item, index) => new Layers.Data.Sqls.PvWms.Notices()
                                         {
                                             ID = serirs[index],
                                             Type = item.Type,
                                             WareHouseID = item.WareHouseID,
                                             WaybillID = item.WaybillID,
                                             InputID = item.InputID,
                                             OutputID = item.OutputID,
                                             ProductID = item.ProductID,
                                             //DateCode = item.DateCode,
                                             Origin = item.Origin,
                                             CustomsName = item.CustomsName,
                                             Quantity = item.Quantity,
                                             Conditions = item.Conditions,
                                             Status = item.Status,
                                             Source = item.Source,
                                             Target = item.Target,
                                             Weight = item.Weight,
                                             NetWeight = item.NetWeight,
                                             Volume = item.Volume,
                                             BoxCode = item.BoxCode,
                                             ShelveID = item.ShelveID,
                                             BoxingSpecs = item.BoxingSpecs,
                                             CreateDate = item.CreateDate,
                                             StorageID = item.StorageID,
                                         }).ToArray();
                    #endregion

                    #region 生成深圳的Picking数据

                    serirs = PkeyType.Pickings.Pick(items.Length);
                    var sz_Pickings = (from notice in sz_OutNotices
                                       join output in sz_Outputs on notice.OutputID equals output.ID
                                       select new Layers.Data.Sqls.PvWms.Pickings
                                       {
                                           ID = null,
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
                                       }).Select((item, index) => new Layers.Data.Sqls.PvWms.Pickings()
                                       {
                                           ID = serirs[index],
                                           StorageID = item.StorageID,
                                           NoticeID = item.NoticeID,
                                           OutputID = item.OutputID,
                                           BoxCode = item.BoxCode,
                                           Quantity = item.Quantity,
                                           AdminID = item.AdminID,
                                           CreateDate = item.CreateDate,
                                           Weight = item.Weight,
                                           NetWeight = item.NetWeight,
                                           Volume = item.Volume,
                                       }).ToArray();

                    #endregion

                    AllData data = new AllData
                    {
                        XdtMainOrderID = order.ID,
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

                    data.Save();

                    //用线程池执行无参数方法
                    //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(data.Save));
                }
            }
        }

        private string GetWaybillID(string itemID, Dictionary<string, string> mapsWaybill, string orderID, Layers.Data.Sqls.PvWms.CgPvWmsSzOutItemsTopView[] szOutItems)
        {
            var outItem = szOutItems.FirstOrDefault(i => i.OrderItemID == itemID);
            if (outItem == null)
            {
                var mapWaybill = mapsWaybill.FirstOrDefault(m => m.Key == orderID);
                if (!default(KeyValuePair<string, string>).Equals(mapWaybill))
                    return mapWaybill.Value;
            }
            else
            {
                var mapWaybill = mapsWaybill.FirstOrDefault(m => m.Key == outItem.ExitNoticeID);
                if (!default(KeyValuePair<string, string>).Equals(mapWaybill))
                    return mapWaybill.Value;
            }

            return string.Empty;
        }

        private class AllData
        {
            public string XdtMainOrderID { get; set; }

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

            public void Save(object state = null)
            {
                using (var conn = ConnManager.Current.PvWms)
                {
                    conn.BulkInsert(HK_Inputs);
                    conn.BulkInsert(HK_EnterNotices);
                    conn.BulkInsert(HK_Sortings);
                    conn.BulkInsert(HK_Storages);
                    conn.BulkInsert(logs_Declares);
                    conn.BulkInsert(Logs_DeclareItem);
                    conn.BulkInsert(HK_Outputs);
                    conn.BulkInsert(HK_OutNotices);
                    conn.BulkInsert(HK_Pickings);

                    conn.BulkInsert(SZ_Inputs);
                    conn.BulkInsert(SZ_EnterNotices);
                    conn.BulkInsert(SZ_Sortings);
                    conn.BulkInsert(SZ_Storages);
                    conn.BulkInsert(SZ_Outputs);
                    conn.BulkInsert(SZ_OutNotices);
                    conn.BulkInsert(SZ_Pickings);
                }

                Services.UpdateService.Current.AddMainOrderIDs(XdtMainOrderID);
            }
        }
    }
}
