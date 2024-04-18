using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 订单账单
    /// </summary>
    public class MyOrderBillView : Yahv.Linq.QueryView<VoucherStatistic, PvWsOrderReponsitory>
    {
        IUser user;

        private MyOrderBillView()
        {

        }

        public MyOrderBillView(IUser User)
        {
            this.user = User;
        }

        public MyOrderBillView(PvWsOrderReponsitory reponsitory, IQueryable<VoucherStatistic> iQuery) : base(reponsitory, iQuery)
        {

        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            var view = new Yahv.Services.Views.VouchersStatisticsView<PvWsOrderReponsitory>(this.Reponsitory);
            return from entity in view
                   where entity.Payee == PvClientConfig.CompanyID && entity.Payer == this.user.EnterpriseID
                   select entity;
        }

        /// <summary>
        /// 根据订单ID查询账单详情数据
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public MyOrderBillView SearchByOrderID(string OrderID)
        {
            var iQuery = this.IQueryable.Where(item => item.OrderID == OrderID);
            return new MyOrderBillView(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 获取订单的账单统计数据
        /// </summary>
        /// <returns></returns>
        private IQueryable<Bill> OrderBill(OrderType[] orderTypes)
        {
            var ordersview = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrdersTopView>().Where(item => item.ClientID == this.user.EnterpriseID && item.MainStatus >= (int)CgOrderStatus.待确认 && orderTypes.Contains((OrderType)item.Type));

            if (!this.user.IsMain)
            {
                ordersview = ordersview.Where(item => item.CreatorID == this.user.ID);
            }

            var linq = from order in ordersview
                       //join entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.VouchersStatisticsView>() on order.ID equals entity.OrderID
                       select new Bill
                       {
                           ID = order.ID,
                           CreateDate = order.CreateDate,
                           Type = (OrderType)order.Type,
                           MainStatus = (CgOrderStatus)order.MainStatus,
                       };
            return linq;
        }

        /// <summary>
        /// 根据传入参数获取订单数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<Bill> GetBills(LambdaExpression[] expressions, OrderType[] orderTypes)
        {
            var linq = this.OrderBill(orderTypes);
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    linq = linq.Where(expression as Expression<Func<Bill, bool>>);
                }
            }
            return linq;
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<Bill> GetPageListBills(LambdaExpression[] expressions, OrderType[] orderTypes, int PageSize, int PageIndex)
        {
            var results0 = this.GetBills(expressions, orderTypes).Distinct();
            var results = (from result in results0
                           join bill in this.IQueryable on result.ID equals bill.OrderID into bills
                           where bills.Any()
                           select result).Distinct();

            int total = results.Count();
            var orders = results.OrderByDescending(item => item.CreateDate).Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();
            var billview = this.IQueryable.Where(item => orders.Select(a => a.ID).Contains(item.OrderID)).ToArray();
            var linq = from order in orders
                       join entity in billview on order.ID equals entity.OrderID into bills
                       where bills.Any()
                       select new Bill
                       {
                           ID = order.ID,
                           CreateDate = order.CreateDate,
                           Type = (Underly.OrderType)order.Type,
                           BillCreateDate = bills.Max(item => item.LeftDate),
                           LeftPrice = bills.Sum(item => (decimal?)item.LeftPrice),
                           RightPrice = bills.Sum(item => item.RightPrice),
                           ReducePrice = bills.Sum(item => item.ReducePrice),
                           CouponPrice = bills.Sum(item => item.CouponPrice),
                       };
            return new PageList<Bill>(PageIndex, PageSize, linq, total);
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<Bill> GetPageListBillsForDeclareBill(LambdaExpression[] expressions, OrderType[] orderTypes, int PageSize, int PageIndex)
        {
            var results = this.GetBills(expressions, orderTypes).Distinct();
            int total = results.Count();
            var orders = results.OrderByDescending(item => item.CreateDate).Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();
            var billview = this.IQueryable.Where(item => orders.Select(a => a.ID).Contains(item.OrderID)).ToArray();

            var billviewOthers = billview.Where(t => !(t.Catalog == "税款" && t.Subject == "关税")
                                                  && !(t.Catalog == "税款" && t.Subject == "消费税")
                                                  && !(t.Catalog == "税款" && t.Subject == "销售增值税")).ToArray();
            var billviewTraiff = billview.Where(t => t.Catalog == "税款" && t.Subject == "关税").ToArray();
            var billviewExcise = billview.Where(t => t.Catalog == "税款" && t.Subject == "消费税").ToArray();
            var billviewAddTax = billview.Where(t => t.Catalog == "税款" && t.Subject == "销售增值税").ToArray();

            var linq = from order in orders
                       join entity in billview on order.ID equals entity.OrderID into bills
                       where bills.Any()

                       join entity in billviewOthers on order.ID equals entity.OrderID into billsOthers
                       join entity in billviewTraiff on order.ID equals entity.OrderID into billsTraiff
                       join entity in billviewExcise on order.ID equals entity.OrderID into billsExcise
                       join entity in billviewAddTax on order.ID equals entity.OrderID into billsAddTax

                       let traiffLeft = billviewTraiff.Where(t => t.OrderID == order.ID).Sum(t => t.LeftPrice)
                       let exciseLeft = billviewExcise.Where(t => t.OrderID == order.ID).Sum(t => t.LeftPrice)
                       let addTaxLeft = billviewAddTax.Where(t => t.OrderID == order.ID).Sum(t => t.LeftPrice)

                       select new Bill
                       {
                           ID = order.ID,
                           CreateDate = order.CreateDate,
                           Type = (Underly.OrderType)order.Type,
                           BillCreateDate = bills.Max(item => item.LeftDate),

                           LeftPrice = billsOthers.Sum(item => (decimal?)item.LeftPrice)
                                     + (traiffLeft > 50 ? billsTraiff.Sum(item => (decimal?)item.LeftPrice) : 0)
                                     + (exciseLeft > 50 ? billsExcise.Sum(item => (decimal?)item.LeftPrice) : 0)
                                     + (addTaxLeft > 50 ? billsAddTax.Sum(item => (decimal?)item.LeftPrice) : 0),

                           RightPrice = billsOthers.Sum(item => item.RightPrice)
                                      + (traiffLeft > 50 ? billsTraiff.Sum(item => item.RightPrice) : 0)
                                      + (exciseLeft > 50 ? billsExcise.Sum(item => item.RightPrice) : 0)
                                      + (addTaxLeft > 50 ? billsAddTax.Sum(item => item.RightPrice) : 0),

                           ReducePrice = billsOthers.Sum(item => item.ReducePrice)
                                       + (traiffLeft > 50 ? billsTraiff.Sum(item => item.ReducePrice) : 0)
                                       + (exciseLeft > 50 ? billsExcise.Sum(item => item.ReducePrice) : 0)
                                       + (addTaxLeft > 50 ? billsAddTax.Sum(item => item.ReducePrice) : 0),

                           CouponPrice = billsOthers.Sum(item => item.CouponPrice)
                                       + (traiffLeft > 50 ? billsTraiff.Sum(item => item.CouponPrice) : 0)
                                       + (exciseLeft > 50 ? billsExcise.Sum(item => item.CouponPrice) : 0)
                                       + (addTaxLeft > 50 ? billsAddTax.Sum(item => item.CouponPrice) : 0),
                       };
            return new PageList<Bill>(PageIndex, PageSize, linq, total);
        }
    }
}
