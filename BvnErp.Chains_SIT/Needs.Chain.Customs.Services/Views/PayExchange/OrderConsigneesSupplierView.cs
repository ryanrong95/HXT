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
    public class OrderConsigneesSupplierView : QueryView<OrderConsigneesSupplierViewModel, ScCustomsReponsitory>
    {
        public OrderConsigneesSupplierView()
        {
        }

        protected OrderConsigneesSupplierView(ScCustomsReponsitory reponsitory, IQueryable<OrderConsigneesSupplierViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<OrderConsigneesSupplierViewModel> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var payExchangeSuppliersView = new Views.OrderPayExchangeSuppliersView(this.Reponsitory);

            var iQuery = from order in orders
                         join payExchangeSupplier in payExchangeSuppliersView on order.ID equals payExchangeSupplier.OrderID into payExchangeSuppliers//on orderConsignee.ClientSupplierID equals clientSupplier.ID
                         where order.Status == (int)Enums.Status.Normal
                         && order.DeclarePrice != order.PaidExchangeAmount
                         select new OrderConsigneesSupplierViewModel
                         {
                             CreateDate = order.CreateDate,
                             ClientID = order.ClientID,
                             Currency = order.Currency,
                             ExchangeRate = order.RealExchangeRate,
                             OrderID = order.ID,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             PaidExchangeAmount = order.PaidExchangeAmount,
                             DeclarePrice = order.DeclarePrice,
                             PayExchangeSuppliers = payExchangeSuppliers.Select(item => new OrderPayExchangeSupplier()
                             {
                                 ID = item.ID,
                                 ClientSupplier = item.ClientSupplier,
                                 OrderID = item.OrderID,
                             }),
                         };
            return iQuery;
        }

    }
    public class OrderConsigneesSupplierViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        ///  创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 付汇汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public OrderPayExchangeSuppliers payExchangeSuppliers { get; set; }

        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 已申请预付金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public IEnumerable<OrderPayExchangeSupplier> PayExchangeSuppliers { get; set; }
    }
}