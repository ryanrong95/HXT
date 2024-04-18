using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.DeclarationNotice
{
    class Class1 : QueryView<DecNoticeListModel, ScCustomsReponsitory>
    {
        public Class1()
        {
        }

        protected Class1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected Class1(ScCustomsReponsitory reponsitory, IQueryable<DecNoticeListModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DecNoticeListModel> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        object kkk()
        {

            IQueryable<Models.DecNoticeListModel> iquery = this.IQueryable.Cast<Models.DecNoticeListModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            //if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            //{
            //    iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            //}

            //获取数据
            var ienum_myDeclares = iquery.ToArray();

            //获取订单的ID
            var ordersID = ienum_myDeclares.Select(item => item.OrderID);


            var orderItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

            var linq_orderItems = from item in orderItemsView
                                  where item.Status == (int)Enums.Status.Normal && ordersID.Contains(item.OrderID)
                                  select new
                                  {
                                      //item.UnitPrice
                                      //item.DeclaredQuantity
                                      item.OrderID,
                                      item.Quantity,
                                      item.TotalPrice,
                                  };

            var groups_orderItems = from item in linq_orderItems.ToArray()
                                    group item by item.OrderID into groups
                                    select new
                                    {
                                        OrderID = groups.Key,
                                        TotalDeclarePrice = groups.Sum(t => t.TotalPrice),
                                        TotalQty = groups.Sum(t => t.Quantity),
                                    };
            #region icgoo

            var icgooOrderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();

            var linq_icgoo = from map in icgooOrderView
                             where ordersID.Contains(map.OrderID)
                             select new
                             {
                                 map.OrderID,
                                 map.IcgooOrder,
                             };
            var ienums_icgoo = linq_icgoo.ToArray();

            #endregion

            return null;
        }
    }
}
