using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class MatchView : QueryView<Models.MatchViewModel, ScCustomsReponsitory>
    {
        public MatchView()
        {
        }

        internal MatchView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected MatchView(ScCustomsReponsitory reponsitory, IQueryable<Models.MatchViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.MatchViewModel> GetIQueryable()
        {
          
            var iQuery = from sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()                        
                         where sorting.Status == (int)Enums.Status.Normal 
                         select new Models.MatchViewModel
                         {
                            SortingID = sorting.ID,
                            CaseNo = sorting.BoxIndex,
                            OrderID = sorting.OrderID,
                            OrderItemID = sorting.OrderItemID,
                            Qty = sorting.Quantity,
                            SortingDecStatus = (Enums.SortingDecStatus)sorting.DecStatus,
                            WarehouseType = (Enums.WarehouseType)sorting.WarehouseType,
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.MatchViewModel> iquery = this.IQueryable.Cast<Models.MatchViewModel>().OrderBy(item => item.CaseNo);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_Sorting = iquery.ToArray();          
            var orderItemIDs = ienum_Sorting.Select(t => t.OrderItemID).ToList();
            var orderIDs = ienum_Sorting.Select(t => t.OrderID).ToList();

            var iquery_OrderItem = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                   where orderItemIDs.Contains(c.ID)
                                   select new Models.MatchViewModel
                                   {
                                       OrderItemID = c.ID,
                                       Name = c.Name,
                                       Model = c.Model,
                                       Brand = c.Manufacturer,
                                       Origin = c.Origin,
                                       Unit = c.Unit,
                                       UnitPrice = c.UnitPrice,
                                       TotalPrice = c.TotalPrice,
                                       BatchNo = c.Batch,
                                       OrderItemQty = c.Quantity,
                                   };

            var ienum_OrderItem = iquery_OrderItem.ToArray();


            var results = from sorting in ienum_Sorting
                          join orderItem in ienum_OrderItem on sorting.OrderItemID equals orderItem.OrderItemID
                          select new Models.MatchViewModel
                          {
                              ID = sorting.SortingID,
                              CaseNo = sorting.CaseNo,
                              BatchNo = orderItem.BatchNo,
                              Name = orderItem.Name,
                              Brand = orderItem.Brand,
                              Model = orderItem.Model,
                              Origin = orderItem.Origin,
                              Qty = sorting.Qty,
                              Unit = orderItem.Unit,
                              UnitPrice = orderItem.UnitPrice,
                              TotalPrice = orderItem.TotalPrice,
                              OrderItemID = orderItem.OrderItemID,
                              OrderItemQty = orderItem.OrderItemQty,
                          };

            var iquery_UnExpectedOrderItem = from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>()
                                             where orderIDs.Contains(orderItem.OrderID) && orderItem.IsMapped==false && orderItem.Status==(int)Enums.Status.Normal
                                             select new Models.MatchViewModel
                                             {
                                                 ID = orderItem.ID,
                                                 Model = orderItem.Model,
                                                 Brand = orderItem.Brand,
                                                 Qty = orderItem.Qty,
                                                 Origin = orderItem.Origin,
                                                 BatchNo = orderItem.Batch,
                                                 CaseNo = orderItem.BoxIndex,
                                             };

            var ienum_UnExpectedOrderItem = iquery_UnExpectedOrderItem.ToArray();

            results = results.Union(ienum_UnExpectedOrderItem).OrderBy(item => item.CaseNo);

            var res = results.Select(
                        item => new
                        {
                           item.ID,
                           item.CaseNo,
                           item.BatchNo,
                           item.Name,
                           item.Brand,
                           item.Model,
                           item.Origin,
                           item.Qty,
                           item.Unit,
                           item.UnitPrice,
                           TotalPrice =item.UnitPrice*item.Qty,
                           item.OrderItemID,
                           item.OrderItemQty,
                        }
                     ).ToArray();

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return res.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public MatchView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID == orderID
                       select query;

            var view = new MatchView(this.Reponsitory, linq);
            return view;
        }

        public MatchView SearchSortingDecStatusNo()
        {
            var linq = from query in this.IQueryable
                       where query.SortingDecStatus == Enums.SortingDecStatus.No
                       select query;

            var view = new MatchView(this.Reponsitory, linq);
            return view;
        }

        public MatchView SearchWarehouseType()
        {
            var linq = from query in this.IQueryable
                       where query.WarehouseType == Enums.WarehouseType.HongKong
                       select query;

            var view = new MatchView(this.Reponsitory, linq);
            return view;
        }
    }
}
