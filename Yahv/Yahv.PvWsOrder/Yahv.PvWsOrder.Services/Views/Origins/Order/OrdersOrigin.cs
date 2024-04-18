using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 订单原视图
    /// </summary>
    public class OrderOrigin : UniqueView<Order, PvWsOrderReponsitory>
    {
        public OrderOrigin()
        {

        }

        public OrderOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            var currentStatus = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CurrentWsOrderStatusTopView>();
            var linq = from order in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>()
                       join orderstatus in currentStatus on order.ID equals orderstatus.MainID
                       select new Order()
                       {
                           ID = order.ID,
                           Type = (OrderType)order.Type,
                           ClientID = order.ClientID,
                           InvoiceID = order.InvoiceID,
                           PayeeID = order.PayeeID,
                           BeneficiaryID = order.BeneficiaryID,
                           MainStatus = (CgOrderStatus)orderstatus.MainStatus,
                           PaymentStatus = (OrderPaymentStatus)orderstatus.PaymentStatus,
                           InvoiceStatus = (OrderInvoiceStatus)orderstatus.InvoiceStatus,
                           RemittanceStatus = (OrderRemittanceStatus)orderstatus.RemittanceStatus,
                           Summary = order.Summary,
                           CreateDate = order.CreateDate,
                           ModifyDate = order.ModifyDate,
                           CreatorID = order.CreatorID,
                           SupplierID = order.SupplierID,
                           SettlementCurrency = (Currency)order.SettlementCurrency,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 订单收货扩展视图
    /// </summary>
    public class OrderInputOrigin : UniqueView<OrderInput, PvWsOrderReponsitory>
    {
        public OrderInputOrigin()
        {

        }

        internal OrderInputOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderInput> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderInputs>()
                       select new OrderInput
                       {
                           ID = entity.ID,
                           BeneficiaryID = entity.BeneficiaryID,
                           IsPayCharge = entity.IsPayCharge,
                           WayBillID = entity.WayBillID,
                           Conditions = entity.Conditions,
                           Currency = (Currency)entity.Currency,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 订单发货扩展视图
    /// </summary>
    public class OrderOutputOrigin : UniqueView<OrderOutput, PvWsOrderReponsitory>
    {
        public OrderOutputOrigin()
        {

        }

        internal OrderOutputOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderOutput> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderOutputs>()
                       select new OrderOutput
                       {
                           ID = entity.ID,
                           BeneficiaryID = entity.BeneficiaryID,
                           IsReciveCharge = entity.IsReciveCharge,
                           WayBillID = entity.WayBillID,
                           Conditions = entity.Conditions,
                           Currency = (Currency)entity.Currency,
                       };
            return linq;
        }
    }
}
