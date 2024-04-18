using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单的对账单
    /// </summary>
    public class OrderBillsView : UniqueView<Models.OrderBill, ScCustomsReponsitory>
    {
        public OrderBillsView()
        {

        }

        protected override IQueryable<OrderBill> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var filesView = new OrderFilesView(this.Reponsitory).Where(file => file.FileType == Enums.FileType.OrderBill &&
                                                                               file.Status == Enums.Status.Normal).OrderByDescending(file => file.CreateDate);
            var linq = from order in ordersView
                       join file in filesView on order.ID equals file.OrderID into files
                       //from file in files.DefaultIfEmpty()
                           //where order.OrderStatus > Enums.OrderStatus.Quoted && order.OrderStatus < Enums.OrderStatus.Completed
                       select new Models.OrderBill()
                       {
                           ID = order.ID,
                           Order = order,
                           Client = order.Client,
                           Agreement = order.ClientAgreement,
                           Currency = order.Currency,
                           CustomsExchangeRate = order.CustomsExchangeRate ?? 0,
                           RealExchangeRate = order.RealExchangeRate ?? 0,
                           DeclarePrice = order.DeclarePrice,
                           IsLoan = order.IsLoan,
                           CreateDate = order.CreateDate,
                           OrderType = order.Type,
                           OrderStatus = order.OrderStatus,
                           File = files.FirstOrDefault()
                       };
            return linq;
        }
    }
}