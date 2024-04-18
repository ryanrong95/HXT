using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views.Alls
{
    public class HKEntryNoticesAll : Needs.Linq.Generic.Unique1Classics<HKEntryNotice, ScCustomsReponsitory>
    {
        protected override IQueryable<HKEntryNotice> GetIQueryable(Expression<Func<HKEntryNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            var ordersView = new Rolls.OrdersRoll(this.Reponsitory).Where(o => o.Status == Enums.Status.Normal);
            var noticesView = new Origins.EntryNoticesOrigin(this.Reponsitory);
            var linq = from notice in noticesView
                       join order in ordersView on notice.OrderID equals order.ID
                       where notice.Status == Enums.Status.Normal && notice.WarehouseType == Enums.WarehouseType.HongKong
                       orderby notice.UpdateDate descending
                       select new HKEntryNotice
                       {
                           ID = notice.ID,
                           OrderID = notice.OrderID,
                           Order = order,
                           DecHeadID = notice.DecHeadID,
                           ClientCode = notice.ClientCode,
                           SortingRequire = notice.SortingRequire,
                           WarehouseType = notice.WarehouseType,
                           EntryNoticeStatus = notice.EntryNoticeStatus,
                           Status = notice.Status,
                           CreateDate = notice.CreateDate,
                           UpdateDate = notice.UpdateDate,
                           Summary = notice.Summary
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<HKEntryNotice, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<HKEntryNotice> OnReadShips(HKEntryNotice[] results)
        {
            return from notice in results
                   select notice;
        }
    }

    public class HKEntryNoticesWithItemsAll : Needs.Linq.Generic.Unique1Classics<HKEntryNotice, ScCustomsReponsitory>
    {
        protected override IQueryable<HKEntryNotice> GetIQueryable(Expression<Func<HKEntryNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            //入库通知项, 页面根据型号筛选入库通知时使用
            var orderItemsView = new Origins.OrderItemsOrigin(this.Reponsitory);
            var noticeItemsView = new Origins.EntryNoticeItemsOrigin(this.Reponsitory);
            var hkNoticeItemsView = from item in noticeItemsView
                                    join orderItem in orderItemsView on item.OrderItemID equals orderItem.ID
                                    where item.Status == Enums.Status.Normal
                                    select new HKEntryNoticeItem
                                    {
                                        ID = item.ID,
                                        EntryNoticeID = item.EntryNoticeID,
                                        OrderItemID = item.OrderItemID,
                                        OrderItem = orderItem,
                                        EntryNoticeStatus = item.EntryNoticeStatus,
                                    };

            var ordersView = new Rolls.OrdersRoll(this.Reponsitory).Where(o => o.Status == Enums.Status.Normal);
            var noticesView = new Origins.EntryNoticesOrigin(this.Reponsitory);
            var linq = from notice in noticesView
                       join order in ordersView on notice.OrderID equals order.ID
                       join item in hkNoticeItemsView on notice.ID equals item.EntryNoticeID into items
                       where notice.Status == Enums.Status.Normal && notice.WarehouseType == Enums.WarehouseType.HongKong
                       orderby notice.CreateDate descending
                       select new HKEntryNotice
                       {
                           ID = notice.ID,
                           OrderID = notice.OrderID,
                           Order = order,
                           DecHeadID = notice.DecHeadID,
                           ClientCode = notice.ClientCode,
                           SortingRequire = notice.SortingRequire,
                           WarehouseType = notice.WarehouseType,
                           EntryNoticeStatus = notice.EntryNoticeStatus,
                           Status = notice.Status,
                           CreateDate = notice.CreateDate,
                           UpdateDate = notice.UpdateDate,
                           Summary = notice.Summary,

                           HKItems = items
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<HKEntryNotice, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<HKEntryNotice> OnReadShips(HKEntryNotice[] results)
        {
            return from notice in results
                   select notice;
        }
    }
}
