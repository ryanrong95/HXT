using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 订单视图
    /// </summary>
    public class OrderAlls : UniqueView<Order, PvWsOrderReponsitory>
    {
        public OrderAlls()
        {

        }

        internal OrderAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            var orderView = new Origins.OrderOrigin(this.Reponsitory);
            var inputView = new OrderInputAlls(this.Reponsitory);
            var outputView = new OrderOutputAlls(this.Reponsitory);
            var clientsView = new WsClientsAlls(this.Reponsitory);
            var supplierView = new WsSupplierAlls(this.Reponsitory);

            var linq = from entity in orderView
                       join client in clientsView on entity.ClientID equals client.ID
                       join supplier in supplierView on new { entity.SupplierID, ClientID = client.ID }
                           equals new { SupplierID = supplier.ID, ClientID = supplier.OwnID } into suppliers
                       from supplier in suppliers.DefaultIfEmpty()
                       join input in inputView on entity.ID equals input.ID into inputs
                       from input in inputs.DefaultIfEmpty()
                       join output in outputView on entity.ID equals output.ID into outputs
                       from output in outputs.DefaultIfEmpty()
                       where entity.MainStatus != CgOrderStatus.取消
                       select new Order
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           InvoiceID = entity.InvoiceID,
                           PayeeID = entity.PayeeID,
                           BeneficiaryID = entity.BeneficiaryID,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           InvoiceStatus = entity.InvoiceStatus,
                           RemittanceStatus = entity.RemittanceStatus,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           CreatorID = entity.CreatorID,
                           SupplierID = entity.SupplierID,
                           SettlementCurrency = entity.SettlementCurrency,

                           OrderInput = input,
                           OrderOutput = output,
                           OrderClient = client,
                           OrderSupplier = supplier,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 订单收货扩展视图
    /// </summary>
    public class OrderInputAlls : UniqueView<OrderInput, PvWsOrderReponsitory>
    {
        public OrderInputAlls()
        {

        }

        internal OrderInputAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderInput> GetIQueryable()
        {
            var inputView = new Origins.OrderInputOrigin(this.Reponsitory);
            var waybillView = new WaybillsAlls(this.Reponsitory);

            var linq = from entity in inputView
                       join waybill in waybillView on entity.WayBillID equals waybill.ID into waybills
                       from waybill in waybills.DefaultIfEmpty()
                       select new OrderInput
                       {
                           ID = entity.ID,
                           BeneficiaryID = entity.BeneficiaryID,
                           WayBillID = entity.WayBillID,
                           IsPayCharge = entity.IsPayCharge,
                           Conditions = entity.Conditions,
                           Currency = entity.Currency,
                           Waybill = waybill,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 订单发货扩展视图
    /// </summary>
    public class OrderOutputAlls : UniqueView<OrderOutput, PvWsOrderReponsitory>
    {
        public OrderOutputAlls()
        {

        }

        internal OrderOutputAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderOutput> GetIQueryable()
        {
            var outputView = new Origins.OrderOutputOrigin(this.Reponsitory);
            var waybillView = new WaybillsAlls(this.Reponsitory);

            var linq = from entity in outputView
                       join waybill in waybillView on entity.WayBillID equals waybill.ID into waybills
                       from waybill in waybills.DefaultIfEmpty()
                       select new OrderOutput
                       {
                           ID = entity.ID,
                           BeneficiaryID = entity.BeneficiaryID,
                           WayBillID = entity.WayBillID,
                           IsReciveCharge = entity.IsReciveCharge,
                           Conditions = entity.Conditions,
                           Currency = entity.Currency,
                           Waybill = waybill,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 代收付款订单视图
    /// </summary>
    public class PaymentOrderView : UniqueView<Order, PvWsOrderReponsitory>
    {
        public PaymentOrderView()
        {
        }

        internal PaymentOrderView(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            var ordersView = new OrderAlls(this.Reponsitory).Where(item => item.MainStatus != CgOrderStatus.取消);
            //订单总货款
            var orderPrice = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().GroupBy(item => item.OrderID).Select(item => new
            {
                OrderID = item.Key,
                TotalPrice = item.Sum(t => t.TotalPrice)
            });
            ////已申请的付款
            //var payView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>()
            //    .Where(item => item.Type == (int)ApplicationType.Payment && item.Status == (int)GeneralStatus.Normal)
            //    .GroupBy(item => item.OrderID)
            //    .Select(item => new
            //    {
            //        OrderID = item.Key,
            //        TotalPrice = item.Sum(t => t.Price)
            //    });
            ////已申请的收款
            //var receivesView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>()
            //    .Where(item => item.Type == (int)ApplicationType.Receival && item.Status == (int)GeneralStatus.Normal)
            //    .GroupBy(item => item.OrderID)
            //    .Select(item => new
            //    {
            //        OrderID = item.Key,
            //        TotalPrice = item.Sum(t => t.Price)
            //    });

            var linq = from entity in ordersView
                       join price in orderPrice on entity.ID equals price.OrderID into prices
                       //from price in prices.DefaultIfEmpty()
                       //join pay in payView on entity.ID equals pay.OrderID into pays
                       //from pay in pays.DefaultIfEmpty()
                       //join receive in receivesView on entity.ID equals receive.OrderID into receives
                       //from receive in receives.DefaultIfEmpty()
                       select new Order
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           InvoiceID = entity.InvoiceID,
                           PayeeID = entity.PayeeID,
                           BeneficiaryID = entity.BeneficiaryID,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           InvoiceStatus = entity.InvoiceStatus,
                           RemittanceStatus = entity.RemittanceStatus,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           CreatorID = entity.CreatorID,
                           SupplierID = entity.SupplierID,
                           SettlementCurrency = entity.SettlementCurrency,

                           OrderInput = entity.OrderInput,
                           OrderOutput = entity.OrderOutput,
                           OrderClient = entity.OrderClient,
                           OrderSupplier = entity.OrderSupplier,

                           //TotalPrice = price == null ? 0m : price.TotalPrice,
                           //PayAppliedPrice = pay == null ? 0m : pay.TotalPrice,
                           //RecAppliedPrice = receive == null ? 0m : receive.TotalPrice,
                       };
            return linq;
        }
    }
}
