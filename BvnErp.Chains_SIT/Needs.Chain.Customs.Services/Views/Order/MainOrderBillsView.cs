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
    public class MainOrderBillsView : UniqueView<Models.OrderBill, ScCustomsReponsitory>
    {
        public MainOrderBillsView()
        {

        }

        protected override IQueryable<OrderBill> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);         
            var linq = from order in ordersView                    
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
                           OrderStatus = order.OrderStatus                         
                       };
            return linq;
        }
    }
}