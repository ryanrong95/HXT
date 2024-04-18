using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExitNoticeView : UniqueView<Models.ExitNotice, ScCustomsReponsitory>
    {
        public ExitNoticeView()
        {
        }

        internal ExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.ExitNotice> GetIQueryable()
        {
            var exitDeliverView = new ExitDeliverView(this.Reponsitory);
            //var decHeadView = new DecHeadsView(this.Reponsitory);
            var clientsView = new ClientsView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);
            return from exitNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on exitNotice.OrderID equals order.ID
                   join client in clientsView on order.ClientID equals client.ID
                   join admin in adminView on exitNotice.AdminID equals admin.ID
                   join exitDeliver in exitDeliverView on exitNotice.ID equals exitDeliver.ExitNoticeID into exitDelivers
                   from exitDeliver in exitDelivers.DefaultIfEmpty()
                   where exitNotice.Status == (int)Enums.Status.Normal
                   select new Models.ExitNotice
                   {
                       ID = exitNotice.ID,
                       OrderId = order.ID,
                       Order = new Order
                       {
                           ID = order.ID,
                           Client = client
                       },
                       Admin = admin,
                       WarehouseType = (Enums.WarehouseType)exitNotice.WarehouseType,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)exitNotice.ExitNoticeStatus,
                       ExitType = (Enums.ExitType)exitNotice.ExitType,
                       Status = (Enums.Status)exitNotice.Status,
                       IsPrint = (Enums.IsPrint)exitNotice.IsPrint,
                       CreateDate = exitNotice.CreateDate,
                       UpdateDate = exitNotice.UpdateDate,
                       Summary = exitNotice.Summary,
                       ExitDeliver = exitDeliver,
                   };
        }
    }

    /// <summary>
    /// 香港出库通知
    /// </summary>
    public class HKExitNoticeView : UniqueView<Models.HKExitNotice, ScCustomsReponsitory>
    {
        public HKExitNoticeView()
        {
        }

        internal HKExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.HKExitNotice> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var outputWayBillView = new WayBillManifestView(this.Reponsitory);
            var decHeadView = new DecHeadsView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);
            return from exitNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on exitNotice.OrderID equals order.ID
                   join client in clientsView on order.ClientID equals client.ID
                   join admin in adminView on exitNotice.AdminID equals admin.ID
                   join decHead in decHeadView on exitNotice.DecHeadID equals decHead.ID
                   join outputwaybill in outputWayBillView on decHead.BillNo equals outputwaybill.ID into outputwaybills
                   from outputwaybill in outputwaybills.DefaultIfEmpty()
                   where exitNotice.Status == (int)Enums.Status.Normal && exitNotice.WarehouseType == (int)Enums.WarehouseType.HongKong
                   select new Models.HKExitNotice
                   {
                       ID = exitNotice.ID,
                       Order = new Order
                       {
                           ID = order.ID,
                           Type = (Enums.OrderType)order.Type,
                           Client = client,
                       },
                       Admin = admin,
                       DecHead = decHead,
                       OutputWayBill = outputwaybill,
                       WarehouseType = (Enums.WarehouseType)exitNotice.WarehouseType,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)exitNotice.ExitNoticeStatus,
                       ExitType = (Enums.ExitType)exitNotice.ExitType,
                       Status = (Enums.Status)exitNotice.Status,
                       CreateDate = exitNotice.CreateDate,
                       UpdateDate = exitNotice.UpdateDate,
                       Summary = exitNotice.Summary,
                   };
        }

        /// <summary>
        /// 按运输批次号出库
        /// </summary>
        /// <param name="VoyageNo"></param>
        public void OutStockByVoyageNo(string VoyageNo)
        {
            //先查出该运输批次下所有的香港出库通知和通知项
            var hkExitNotices = this.Where(en => en.DecHead.VoyNo == VoyageNo).OrderBy(en => en.ID).ToArray();
            //比对报关单和出库通知数据
            var decHeads = new Views.DecHeadsView().Where(dh => dh.VoyNo == VoyageNo && dh.CusDecStatus != "04").ToArray();
            foreach (var decHead in decHeads)
            {
                int count = hkExitNotices.Where(en => en.DecHead.ID == decHead.ID).Count();
                if (count == 0)
                    throw new Exception("该运输批次下的报关单(单号：" + decHead.ID + " 合同号：" + decHead.ContrNo + ")尚未生成出库通知，不能出库，请仔细核对");
                if (count > 1)
                    throw new Exception("该运输批次下的报关单(单号：" + decHead.ID + " 合同号：" + decHead.ContrNo + ")生成了" + count + "个出库通知，数据异常不能出库，请仔细核对");
            }

            //排除通过报关辅助工具自动出库的通知
            hkExitNotices = hkExitNotices.Where(en => en.ExitNoticeStatus != ExitNoticeStatus.Exited).ToArray();
            if (hkExitNotices == null || hkExitNotices.Count() == 0)
            {
                return;
            }

            var hkExitNoticeIds = hkExitNotices.Select(en => en.ID).ToArray();
            var hkItems = new Views.HKExitNoticeItemView().Where(eni => hkExitNoticeIds.Contains(eni.ExitNoticeID))
                                                          .OrderBy(eni => eni.ExitNoticeID).ThenBy(eni => eni.ID).ToArray();
            var hkItemIds = hkItems.Select(i => i.ID).ToArray();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                System.Diagnostics.Stopwatch watch1 = new System.Diagnostics.Stopwatch();
                watch1.Start();

                //更新ExitNoticeItems表状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        ExitNoticeStatus = ExitNoticeStatus.Exited
                    }, item => hkItemIds.Contains(item.ID));

                //更新ExitNotice主表的状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        ExitNoticeStatus = ExitNoticeStatus.Exited
                    }, item => hkExitNoticeIds.Contains(item.ID));

                watch1.Stop();
                TimeSpan span1 = watch1.Elapsed;
            }

            Task.Run(() =>
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //生成深圳仓库的入库通知
                    var szEntryNotices = hkExitNotices.Select(item => new Layer.Data.Sqls.ScCustoms.EntryNotices
                    {
                        ID = "SZ" + Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice),
                        OrderID = item.Order.ID,
                        WarehouseType = (int)WarehouseType.ShenZhen,
                        DecHeadID = item.DecHead.ID,
                        ClientCode = item.Order.Client.ClientCode,
                        SortingRequire = (int)SortingRequire.Packed,
                        EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal
                    }).ToArray();
                    reponsitory.Insert(szEntryNotices);

                    //生成深圳入库通知项
                    var szEntryNoticeItems = hkItems.Select(item => new Layer.Data.Sqls.ScCustoms.EntryNoticeItems()
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem),
                        EntryNoticeID = szEntryNotices.SingleOrDefault(en => en.DecHeadID == item.DecList.DeclarationID).ID,
                        DecListID = item.DecList.ID,
                        IsSpotCheck = false,
                        EntryNoticeStatus = (int)EntryNoticeStatus.UnBoxed,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal,
                    }).ToArray();
                    reponsitory.Insert(szEntryNoticeItems);

                    //生成深圳分拣结果
                    var szSortings = hkItems.Select(item => new Layer.Data.Sqls.ScCustoms.Sortings()
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting),
                        OrderID = item.DecList.OrderID,
                        OrderItemID = item.DecList.OrderItemID,
                        EntryNoticeItemID = szEntryNoticeItems.FirstOrDefault(eni => eni.DecListID == item.DecList.ID).ID,
                        //ProductID = item.DecList.DeclarationNoticeItem.Sorting.Product.ID,
                        Quantity = item.DecList.GQty,
                        BoxIndex = item.DecList.CaseNo,
                        NetWeight = item.DecList.NetWt == null ? 0M : (decimal)item.DecList.NetWt,
                        GrossWeight = item.DecList.GrossWt == null ? 0M : (decimal)item.DecList.GrossWt,
                        DecStatus = (int)SortingDecStatus.Yes,
                        AdminID = hkExitNotices.FirstOrDefault().Admin.ID,
                        WrapType = hkExitNotices.FirstOrDefault(en => en.DecHead.ID == item.DecList.DeclarationID).DecHead.WrapType,
                        WarehouseType = (int)WarehouseType.ShenZhen,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal,
                    }).ToArray();
                    reponsitory.Insert(szSortings);

                    //TODO: 香港库房下架后期设计好后，再修改
                    var storages = new HKStoreStorageView(reponsitory);
                    var storageIds = (from storage in storages.AsEnumerable()
                                      join item in hkItems on storage.Sorting.ID equals item.DecList.DeclarationNoticeItem.Sorting.ID
                                      select storage.ID).ToArray();
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(new
                    {
                        Status = Enums.Status.Delete
                    }, s => storageIds.Contains(s.ID));

                    foreach (var hkExitNotice in hkExitNotices)
                    {
                        hkExitNotice.OnHKExited();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 出库单-我的跟单
    /// </summary>
    public class SZExitNoticeView : UniqueView<Models.SZExitNotice, ScCustomsReponsitory>
    {
        public SZExitNoticeView()
        {
        }

        internal SZExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.SZExitNotice> GetIQueryable()
        {
            var exitNoticeView = new ExitNoticeView(this.Reponsitory);
            return from exitNotice in exitNoticeView
                   where exitNotice.WarehouseType == Enums.WarehouseType.ShenZhen
                   select new Models.SZExitNotice
                   {
                       ID = exitNotice.ID,
                       Order = exitNotice.Order,
                       Admin = exitNotice.Admin,
                       WarehouseType = exitNotice.WarehouseType,
                       ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                       ExitType = exitNotice.ExitType,
                       Status = exitNotice.Status,
                       CreateDate = exitNotice.CreateDate,
                       UpdateDate = exitNotice.UpdateDate,
                       Summary = exitNotice.Summary,
                       ExitDeliver = exitNotice.ExitDeliver,
                       IsPrint = exitNotice.IsPrint
                   };
        }
    }

    public class SZExitNoticeUnExitedView : UniqueView<Models.SZExitNotice, ScCustomsReponsitory>
    {
        public SZExitNoticeUnExitedView()
        {
        }

        internal SZExitNoticeUnExitedView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.SZExitNotice> GetIQueryable()
        {
            var exitNoticeView = new ExitNoticeView(this.Reponsitory);
            var decHeadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.IsSuccess);
            return from exitNotice in exitNoticeView
                   join decHead in decHeadView on exitNotice.OrderId equals decHead.OrderID
                   join voyage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>() on decHead.VoyNo equals voyage.ID
                   where exitNotice.WarehouseType == Enums.WarehouseType.ShenZhen && voyage.CutStatus != (int)Enums.CutStatus.UnCutting
                   && (exitNotice.ExitNoticeStatus == Enums.ExitNoticeStatus.UnExited || exitNotice.ExitNoticeStatus == Enums.ExitNoticeStatus.Exiting)
                   select new Models.SZExitNotice
                   {
                       ID = exitNotice.ID,
                       Order = exitNotice.Order,
                       Admin = exitNotice.Admin,
                       WarehouseType = exitNotice.WarehouseType,
                       ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                       ExitType = exitNotice.ExitType,
                       Status = exitNotice.Status,
                       CreateDate = exitNotice.CreateDate,
                       UpdateDate = exitNotice.UpdateDate,
                       Summary = exitNotice.Summary,
                       ExitDeliver = exitNotice.ExitDeliver,
                       IsPrint = exitNotice.IsPrint
                   };
        }
    }


    public class MianOrderExitNoticeView : UniqueView<Models.ExitNotice, ScCustomsReponsitory>
    {
        public MianOrderExitNoticeView()
        {
        }

        internal MianOrderExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.ExitNotice> GetIQueryable()
        {
            var exitDeliverView = new ExitDeliverView(this.Reponsitory);
            //var decHeadView = new DecHeadsView(this.Reponsitory);
            var clientsView = new ClientsView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);
            return from exitNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on exitNotice.OrderID equals order.ID
                   join client in clientsView on order.ClientID equals client.ID
                   join admin in adminView on exitNotice.AdminID equals admin.ID
                   join exitDeliver in exitDeliverView on exitNotice.ID equals exitDeliver.ExitNoticeID into exitDelivers
                   from exitDeliver in exitDelivers.DefaultIfEmpty()
                   where exitNotice.Status == (int)Enums.Status.Normal
                   select new Models.ExitNotice
                   {
                       ID = exitNotice.ID,
                       OrderId = order.ID,
                       Order = new Order
                       {
                           ID = order.ID,
                           Client = client
                       },
                       Admin = admin,
                       WarehouseType = (Enums.WarehouseType)exitNotice.WarehouseType,
                       ExitNoticeStatus = (Enums.ExitNoticeStatus)exitNotice.ExitNoticeStatus,
                       ExitType = (Enums.ExitType)exitNotice.ExitType,
                       Status = (Enums.Status)exitNotice.Status,
                       IsPrint = (Enums.IsPrint)exitNotice.IsPrint,
                       CreateDate = exitNotice.CreateDate,
                       UpdateDate = exitNotice.UpdateDate,
                       Summary = exitNotice.Summary,
                       ExitDeliver = exitDeliver,
                   };
        }
    }

    public class SZMianExitNoticeView : UniqueView<Models.SZExitNotice, ScCustomsReponsitory>
    {
        public SZMianExitNoticeView()
        {
        }

        internal SZMianExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.SZExitNotice> GetIQueryable()
        {
            var exitNoticeView = new MianOrderExitNoticeView(this.Reponsitory);
            return from exitNotice in exitNoticeView
                   where exitNotice.WarehouseType == Enums.WarehouseType.ShenZhen
                   select new Models.SZExitNotice
                   {
                       ID = exitNotice.ID,
                       Order = exitNotice.Order,
                       Admin = exitNotice.Admin,
                       WarehouseType = exitNotice.WarehouseType,
                       ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                       ExitType = exitNotice.ExitType,
                       Status = exitNotice.Status,
                       CreateDate = exitNotice.CreateDate,
                       UpdateDate = exitNotice.UpdateDate,
                       Summary = exitNotice.Summary,
                       ExitDeliver = exitNotice.ExitDeliver,
                       IsPrint = exitNotice.IsPrint
                   };
        }
    }

    public class SubOrderExitQtyView : UniqueView<Models.SZExitNoticeItem, ScCustomsReponsitory>
    {
        public string OrderID { get; set; }

        public SubOrderExitQtyView(string orderID)
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<Models.SZExitNoticeItem> GetIQueryable()
        {
            var result = from exitNoticeItems in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                         join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on exitNoticeItems.SortingID equals sorting.ID
                         join orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on sorting.OrderItemID equals orderitem.ID
                         where orderitem.OrderID == this.OrderID && sorting.WarehouseType == (int)WarehouseType.ShenZhen && exitNoticeItems.ExitNoticeStatus == (int)ExitNoticeStatus.Exited
                         select new SZExitNoticeItem
                         {
                             Quantity = exitNoticeItems.Quantity,
                         };

            return result;

        }
    }

    public class CenterSZExitNoticeView : UniqueView<Models.SZExitNotice, ScCustomsReponsitory>
    {
        public CenterSZExitNoticeView()
        {
        }

        internal CenterSZExitNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 出库通知View
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.SZExitNotice> GetIQueryable()
        {
            //var exitNoticeView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SZOutputWaybillTopView>();
            var clientsView = new ClientsView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);

            var data = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSzOutputWaybillsTopView>()
                       join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on c.OrderID equals d.ID
                       join e in clientsView on d.ClientID equals e.ID
                       join f in adminView on d.AdminID equals f.ID
                       select new Models.SZExitNotice
                       {
                           ID = c.WaybillID,
                           Order = new Order
                           {
                               ID = c.OrderID,
                               Client = e
                           },
                           Admin = f,
                           WarehouseType = WarehouseType.ShenZhen,
                           //ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                           CenterExeStatus = (CgPickingExcuteStatus)c.IsModify,
                           CenterExitType = (WaybillType)c.WaybillType,
                           CenterPackNo = c.Quantity.Value,
                           //ExitType = exitNotice.ExitType,
                           Status = Status.Normal,
                           CreateDate = c.CreateDate,
                           UpdateDate = c.CreateDate,                         
                           IsPrint = c.IsModify.Value == (int)CgPickingExcuteStatus.Completed ? IsPrint.Printed : IsPrint.UnPrint
                       };

            return data;

       
        }
    }
}
