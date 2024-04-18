using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 待发货订单
    /// </summary>
    public class OrderDeliveriesView : UniqueView<Models.Order, ScCustomsReponsitory>
    {
        public OrderDeliveriesView()
        {
        }

        internal OrderDeliveriesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            //未全部送货的深圳Sorting
            var exitNoticeItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                      where item.Status == (int)Enums.Status.Normal
                                      select item;
            var sortingsView = from sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()
                               join item in exitNoticeItemsView on sorting.ID equals item.SortingID into items
                               where sorting.WarehouseType == (int)Enums.WarehouseType.ShenZhen && (items.Count() == 0 || sorting.Quantity > items.Select(item => item.Quantity).Sum())
                               select sorting;

            //有到货但还没有全部送货的订单
            var ordersView = new OrdersView(this.Reponsitory);
            return from order in ordersView
                   where sortingsView.Any(s => s.OrderID == order.ID)
                   select new Models.Order
                   {
                       ID = order.ID,
                       AdminID = order.AdminID,
                       Client = order.Client,
                       ClientAgreement = order.ClientAgreement,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                   };
        }
    }
}
