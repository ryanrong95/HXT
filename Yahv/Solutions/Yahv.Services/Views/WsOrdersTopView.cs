using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    public class WsOrdersTopView<TReponsitory> : UniqueView<WsOrder, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsOrdersTopView()
        {

        }
        public WsOrdersTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        public WsOrdersTopView(TReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(reponsitory, iquery)
        {

        }

        protected override IQueryable<WsOrder> GetIQueryable()
        {
            return from order in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrdersTopView>()
                   //join orderstatus in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CurrentWsOrderStatusTopView>() on order.ID equals orderstatus.MainID
                   select new WsOrder
                   {
                       ID = order.ID,
                       Type = (OrderType)order.Type,
                       ClientID = order.ClientID,
                       InvoiceID = order.InvoiceID,
                       PayeeID = order.PayeeID,
                       BeneficiaryID = order.BeneficiaryID,
                       MainStatus = (CgOrderStatus)order.MainStatus,
                       PaymentStatus = (OrderPaymentStatus)order.PaymentStatus,
                       InvoiceStatus = (OrderInvoiceStatus)order.InvoiceStatus,
                       RemittanceStatus = (OrderRemittanceStatus)order.RemittanceStatus,
                       SupplierID = order.SupplierID,
                       Summary = order.Summary,
                       CreateDate = order.CreateDate,
                       ModifyDate = order.ModifyDate,
                       CreatorID = order.CreatorID,
                       SettlementCurrency = (Currency?)order.SettlementCurrency,
                       TotalPrice = order.TotalPrice,
                       Input = new WsOrderInput
                       {
                           ID = order.ID,
                           BeneficiaryID = order.inBeneficiaryID,
                           Conditions = order.InConditions,
                           Currency = (Currency?)order.inCurrency,
                           WayBillID = order.InWayBillID,
                           IsPayCharge = order.IsPayCharge,
                       },
                       Output = new WsOrderOutput
                       {
                           ID = order.ID,
                           BeneficiaryID = order.outBeneficiaryID,
                           Conditions = order.OutConditions,
                           Currency = (Currency?)order.outCurrency,
                           WayBillID = order.OutWayBillID,
                           IsReciveCharge = order.IsReciveCharge,
                       },
                   };
        }
    }
}
