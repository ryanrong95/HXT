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
    /// 入库通知Item视图
    /// </summary>
    public class EntryNoticeItemView : UniqueView<Models.EntryNoticeItem, ScCustomsReponsitory>
    {
        public EntryNoticeItemView()
        {
        }

        internal EntryNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.EntryNoticeItem> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                   where item.Status == (int)Enums.Status.Normal
                   select new Models.EntryNoticeItem
                   {
                       ID = item.ID,
                       EntryNoticeID = item.EntryNoticeID,
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)item.EntryNoticeStatus,
                       Status = (Enums.Status)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                   };
        }
    }

    /// <summary>
    /// 香港入库通知Item视图
    /// </summary>
    public class HKEntryNoticeItemView : UniqueView<Models.HKEntryNoticeItem, ScCustomsReponsitory>
    {
        public HKEntryNoticeItemView()
        {
        }

        internal HKEntryNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HKEntryNoticeItem> GetIQueryable()
        {
            //查询香港Sorting的装箱数量
            var sortingView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()
                .Where(item => item.Status == (int)Enums.Status.Normal).Where(item => item.WarehouseType == (int)Enums.WarehouseType.HongKong);
            var sortingSum = from sorting in sortingView
                             group sorting by sorting.OrderItemID into g
                             select new
                             {
                                 OrderItemID = g.Key,
                                 Quantity = g.Sum(c => c.Quantity),
                             };

            var orderItemView = new OrderItemsView(this.Reponsitory);

            var result = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                         join orderItem in orderItemView on item.OrderItemID equals orderItem.ID into orderItems
                         from orderItem in orderItems.DefaultIfEmpty()
                         join sortingItem in sortingSum on orderItem.ID equals sortingItem.OrderItemID into sortingItems
                         from sortingItem in sortingItems.DefaultIfEmpty()
                         where item.Status == (int)Enums.Status.Normal
                         select new Models.HKEntryNoticeItem
                         {
                             ID = item.ID,
                             EntryNoticeID = item.EntryNoticeID,
                             OrderItem = orderItem,
                             IsSportCheck = item.IsSpotCheck.Value,
                             EntryNoticeStatus = (Enums.EntryNoticeStatus)item.EntryNoticeStatus,
                             Status = (Enums.Status)item.Status,
                             CreateDate = item.CreateDate,
                             UpdateDate = item.UpdateDate,
                             //未装箱数量 //TODO:单独写一个View
                             RelQuantity = orderItem.Quantity - (sortingItem == null ? 0M : sortingItem.Quantity),
                         };
            return result;
        }
    }

    /// <summary>
    /// 深圳入库通知Item视图
    /// </summary>
    public class SZEntryNoticeItemView : UniqueView<Models.SZEntryNoticeItem, ScCustomsReponsitory>
    {
        public SZEntryNoticeItemView()
        {
        }

        internal SZEntryNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZEntryNoticeItem> GetIQueryable()
        {
            var decListsView = new DecListsView(this.Reponsitory);

            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                   join decList in decListsView on item.DecListID equals decList.ID into decListItems
                   from decList in decListItems.DefaultIfEmpty()
                   where item.Status == (int)Enums.Status.Normal
                   select new Models.SZEntryNoticeItem
                   {
                       ID = item.ID,
                       EntryNoticeID = item.EntryNoticeID,
                       DecList = decList,
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)item.EntryNoticeStatus,
                       Status = (Enums.Status)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                   };
        }
    }

    public class SZEntryNoticeItemExportView : UniqueView<Models.SZEntryNoticeItem, ScCustomsReponsitory>
    {
        public SZEntryNoticeItemExportView()
        {
        }

        internal SZEntryNoticeItemExportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZEntryNoticeItem> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var decListsView = new DecListsView(this.Reponsitory);
            var entryNoticeView = new EntryNoticeView(this.Reponsitory);

            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                   join entryNotice in entryNoticeView on item.EntryNoticeID equals entryNotice.ID
                   join decList in decListsView on item.DecListID equals decList.ID 
                   where item.Status == (int)Enums.Status.Normal && entryNotice.WarehouseType==Enums.WarehouseType.ShenZhen
                   select new Models.SZEntryNoticeItem
                   {
                       ID = item.ID,
                       EntryNoticeID = item.EntryNoticeID,
                       DecList = new Models.DecList {
                           ID=decList.ID,
                           CaseNo=decList.CaseNo,
                           NetWt=decList.NetWt,
                           GrossWt=decList.GrossWt
                       },
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)item.EntryNoticeStatus,
                       Status = (Enums.Status)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                       EntryNotice = entryNotice
                   };
        }
    }
}
