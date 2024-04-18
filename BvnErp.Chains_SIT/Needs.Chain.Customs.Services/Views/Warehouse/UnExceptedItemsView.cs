using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class UnExceptedItemsView : QueryView<Models.UnExpectedOrderItem, ScCustomsReponsitory>
    {
        public UnExceptedItemsView()
        {

        }

        internal UnExceptedItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected UnExceptedItemsView(ScCustomsReponsitory reponsitory, IQueryable<Models.UnExpectedOrderItem> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.UnExpectedOrderItem> GetIQueryable()
        {
            var unexpetcedView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>();
            var result = from unExpected in unexpetcedView
                         where  unExpected.Status == (int)Enums.Status.Normal                       
                         select new Models.UnExpectedOrderItem
                         {
                             ID = unExpected.ID,
                             OrderID = unExpected.OrderID,
                             Model = unExpected.Model,
                             Brand = unExpected.Brand,
                             Qty = unExpected.Qty,
                             Origin = unExpected.Origin,
                             Batch = unExpected.Batch,
                             BoxIndex = unExpected.BoxIndex,
                             GrossWeight = unExpected.GrossWeight,
                             Volume = unExpected.Volume,
                             Status = (Enums.Status)unExpected.Status,
                             CreateDate = unExpected.CreateDate,
                             UpdateDate = unExpected.UpdateDate,
                             UnExpectedReason = unExpected.UnexpectedReason,
                             Summary = unExpected.Summary,
                             IsMapped = unExpected.IsMapped,
                         };

            return result;
        }

        public UnExceptedItemsView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID == orderID
                       select query;

            var view = new UnExceptedItemsView(this.Reponsitory, linq);
            return view;
        }

        public UnExceptedItemsView SearchByIsMapped(bool isMapped)
        {
            var linq = from query in this.IQueryable
                       where query.IsMapped == isMapped
                       select query;

            var view = new UnExceptedItemsView(this.Reponsitory, linq);
            return view;
        }
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.UnExpectedOrderItem> iquery = this.IQueryable.Cast<Models.UnExpectedOrderItem>().OrderBy(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

          
            var results = ienum_EntryNotices;

            var res = results.Select(
                        unExpected => new
                        {
                            ID = unExpected.ID,
                            OrderID = unExpected.OrderID,
                            Model = unExpected.Model,
                            Brand = unExpected.Brand,
                            Qty = unExpected.Qty,
                            Origin = unExpected.Origin,
                            Batch = unExpected.Batch,
                            BoxIndex = unExpected.BoxIndex,
                            GrossWeight = unExpected.GrossWeight,
                            Volume = unExpected.Volume,
                            Status = unExpected.Status,
                            CreateDate = unExpected.CreateDate,
                            UpdateDate = unExpected.UpdateDate,
                            UnExpectedReason = unExpected.UnExpectedReason,
                            Summary = unExpected.Summary,
                            IsMapped = unExpected.IsMapped,
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
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
    }
}
