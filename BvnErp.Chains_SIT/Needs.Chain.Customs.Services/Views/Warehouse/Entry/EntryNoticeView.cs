using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 入库通知视图
    /// </summary>
    public class EntryNoticeView : UniqueView<Models.EntryNotice, ScCustomsReponsitory>
    {
        public EntryNoticeView()
        {
        }

        internal EntryNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.EntryNotice> GetIQueryable()
        {
            //代理报关订单
            var orderView = new OrdersView(this.Reponsitory);

            return from entryNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                   join order in orderView on entryNotice.OrderID equals order.ID
                   where entryNotice.Status == (int)Enums.Status.Normal
                   orderby entryNotice.EntryNoticeStatus
                   select new Models.EntryNotice
                   {
                       ID = entryNotice.ID,
                       Order = order,
                       ClientCode = order.Client.ClientCode,
                       SortingRequire = (Enums.SortingRequire)entryNotice.SortingRequire,
                       WarehouseType = (Enums.WarehouseType)entryNotice.WarehouseType,
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)entryNotice.EntryNoticeStatus,
                       Status = (Enums.Status)entryNotice.Status,
                       CreateDate = entryNotice.CreateDate,
                       UpdateDate = entryNotice.UpdateDate,
                       Summary = entryNotice.Summary,
                   };
        }
    }

    /// <summary>
    /// 香港入库通知视图
    /// </summary>
    public class HKEntryNoticeView : UniqueView<Models.HKEntryNotice, ScCustomsReponsitory>
    {
        public HKEntryNoticeView()
        {
        }

        internal HKEntryNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKEntryNotice> GetIQueryable()
        {
            //入库通知
            var entryNoticeView = new EntryNoticeView(this.Reponsitory);
            var entryNoticeItemView = new HKEntryNoticeItemView(this.Reponsitory);

            return from entryNotice in entryNoticeView
                   join item in entryNoticeItemView on entryNotice.ID equals item.EntryNoticeID into items
                   where entryNotice.WarehouseType == Enums.WarehouseType.HongKong
                   select new Models.HKEntryNotice
                   {
                       ID = entryNotice.ID,
                       Order = entryNotice.Order,
                       ClientCode = entryNotice.ClientCode,
                       SortingRequire = entryNotice.SortingRequire,
                       WarehouseType = entryNotice.WarehouseType,
                       EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                       Status = entryNotice.Status,
                       CreateDate = entryNotice.CreateDate,
                       UpdateDate = entryNotice.UpdateDate,
                       Summary = entryNotice.Summary,
                       HKItems = items,
                   };
        }
    }

    /// <summary>
    /// 香港入库通知视图(不包含项)
    /// </summary>
    public class HKEntryNoticeSimpleView : UniqueView<Models.HKEntryNotice, ScCustomsReponsitory>
    {
        public HKEntryNoticeSimpleView()
        {
        }

        internal HKEntryNoticeSimpleView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKEntryNotice> GetIQueryable()
        {
            //入库通知
            var entryNoticeView = new EntryNoticeView(this.Reponsitory);
            return from entryNotice in entryNoticeView
                   where entryNotice.WarehouseType == Enums.WarehouseType.HongKong
                   select new Models.HKEntryNotice
                   {
                       ID = entryNotice.ID,
                       Order = entryNotice.Order,
                       ClientCode = entryNotice.ClientCode,
                       SortingRequire = entryNotice.SortingRequire,
                       WarehouseType = entryNotice.WarehouseType,
                       EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                       Status = entryNotice.Status,
                       CreateDate = entryNotice.CreateDate,
                       UpdateDate = entryNotice.UpdateDate,
                       Summary = entryNotice.Summary,
                   };
        }
    }

    /// <summary>
    /// 深圳入库通知视图
    /// </summary>
    public class SZEntryNoticeView : UniqueView<Models.SZEntryNotice, ScCustomsReponsitory>
    {
        public SZEntryNoticeView()
        {
        }

        internal SZEntryNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZEntryNotice> GetIQueryable()
        {
            //报关单
            var decHeadView = new DecHeadsView(this.Reponsitory);
            //代理报关订单
            var orderView = new OrdersView(this.Reponsitory);

            return from entryNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                   join order in orderView on entryNotice.OrderID equals order.ID
                   join decHead in decHeadView on entryNotice.DecHeadID equals decHead.ID into decHeads
                   from decHead in decHeads.DefaultIfEmpty()
                   where entryNotice.WarehouseType == (int)Enums.WarehouseType.ShenZhen
                   select new Models.SZEntryNotice
                   {
                       ID = entryNotice.ID,
                       Order = order,
                       DecHead = decHead,
                       ClientCode = entryNotice.ClientCode,
                       WarehouseType = (Enums.WarehouseType)entryNotice.WarehouseType,
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)entryNotice.EntryNoticeStatus,
                       Status = (Enums.Status)entryNotice.Status,
                       CreateDate = entryNotice.CreateDate,
                       UpdateDate = entryNotice.UpdateDate,
                       Summary = entryNotice.Summary,
                   };
        }
    }

    /// <summary>
    /// Icgoo视图
    /// </summary>
    public class IcgooHKEntryNoticeView : UniqueView<Models.IcgooHKEntryNotice, ScCustomsReponsitory>
    {
        public IcgooHKEntryNoticeView()
        {
        }

        internal IcgooHKEntryNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.IcgooHKEntryNotice> GetIQueryable()
        {
            //入库通知
            var entryNoticeView = new EntryNoticeView(this.Reponsitory);
            var entryNoticeItemView = new HKEntryNoticeItemView(this.Reponsitory);

            return from entryNotice in entryNoticeView
                   join item in entryNoticeItemView on entryNotice.ID equals item.EntryNoticeID into items
                   where entryNotice.WarehouseType == Enums.WarehouseType.HongKong
                   select new Models.IcgooHKEntryNotice
                   {
                       ID = entryNotice.ID,
                       Order = entryNotice.Order,
                       ClientCode = entryNotice.ClientCode,
                       SortingRequire = entryNotice.SortingRequire,
                       WarehouseType = entryNotice.WarehouseType,
                       EntryNoticeStatus = entryNotice.EntryNoticeStatus,
                       Status = entryNotice.Status,
                       CreateDate = entryNotice.CreateDate,
                       UpdateDate = entryNotice.UpdateDate,
                       Summary = entryNotice.Summary,
                       HKItems = items,
                   };
        }
    }
}
