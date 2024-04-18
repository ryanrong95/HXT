using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SZOutputView : UniqueView<Models.SZOutput, ScCustomsReponsitory>
    {
        public SZOutputView()
        { }

        internal SZOutputView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZOutput> GetIQueryable()
        {
            //var productsView = new ProductsViews(this.Reponsitory);
            var ordersView = new OrdersView(this.Reponsitory);

            var linq = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                       join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on item.SortingID equals sorting.ID
                       //join product in productsView on sorting.ProductID equals product.ID
                       join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on sorting.OrderItemID equals orderItem.ID
                       join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on orderItem.ID equals decList.OrderItemID
                       join order in ordersView on sorting.OrderID equals order.ID
                       where item.Status == (int)Enums.Status.Normal && item.ExitNoticeStatus == (int)Enums.ExitNoticeStatus.Exited &&
                             sorting.WarehouseType == (int)Enums.WarehouseType.ShenZhen
                       select new Models.SZOutput
                       {
                           ID = item.ID,
                           ExitNoticeID = item.ExitNoticeID,
                           Quantity = item.Quantity,
                           //Sorting = new Models.SZSorting
                           //{
                           //    Product = product
                           //},
                           ExitNoticeStatus = (Enums.ExitNoticeStatus)item.ExitNoticeStatus,
                           Status = (Enums.Status)item.Status,
                           CreateDate = item.CreateDate,
                           UpdateDate = item.UpdateDate,

                           TotalQuantity = orderItem.Quantity,
                           InUnitPrice = decList.DeclPrice,
                           IsExternalOrder = order.Type == Enums.OrderType.Outside,
                           InvoiceCompany = (order.Type == Enums.OrderType.Outside ? order.Client.Company.Name : HZB1BCompany.CompanyName),
                       };

            /*
            var ExitNoticeItems = new SZExitNoticeItemView(this.Reponsitory)
                .Where(item => item.ExitNoticeStatus == Enums.ExitNoticeStatus.Exited);
            //报关单项
            var DecListsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            //报关单
            var DecHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var result = from item in ExitNoticeItems
                         join decList in DecListsView on item.Sorting.OrderItem.ID equals decList.OrderItemID
                         join decHead in DecHeadsView on decList.DeclarationID equals decHead.ID
                         select new Models.SZOutput
                         {
                             ID = item.ID,
                             ExitNoticeID = item.ExitNoticeID,
                             Quantity = item.Quantity,
                             Sorting = item.Sorting,
                             ExitNoticeStatus = item.ExitNoticeStatus,
                             Status = item.Status,
                             CreateDate = item.CreateDate,
                             UpdateDate = item.UpdateDate,

                             TotalQuantity = item.Sorting.OrderItem.Quantity,
                             InUnitPrice = decList.DeclPrice,
                             IsExternalOrder = false,
                             //InvoiceCompany = decHead.OwnerName,
                         };
                         */
            return linq;
        }
    }
}
