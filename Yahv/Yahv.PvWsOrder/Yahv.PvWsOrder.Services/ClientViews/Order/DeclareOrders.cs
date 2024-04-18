using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.Services.Models;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Services.Views;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 报关订单(报关+转报关)
    /// </summary>
    public class DeclareOrders : OrderViewBase
    {
        IUser user;

        private DeclareOrders()
        {

        }

        public DeclareOrders(IUser User) : base(User, new OrderType[] { OrderType.Declare, OrderType.TransferDeclare })
        {
            this.user = User;
        }

        public DeclareOrders(IUser User, PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(User, reponsitory, iquery, new OrderType[] { OrderType.Declare, OrderType.TransferDeclare })
        {
            this.user = User;
        }

        #region 查询条件查询
        /// <summary>
        /// 根据供应商查询
        /// </summary>
        /// <param name="Supplier"></param>
        /// <returns></returns>
        public DeclareOrders SearchBySupplier(string Supplier)
        {
            return new DeclareOrders(this.user, this.Reponsitory, base.SearchBySupplierName(Supplier));
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public DeclareOrders SearchByPart(string PartNumber)
        {
            return new DeclareOrders(this.user, this.Reponsitory, base.SearchByPartNumber(PartNumber));
        }
        #endregion

        /// <summary>
        /// 报关单获取列表数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public override PageList<OrderExtends> GetPageListOrders(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            var Orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var total = Orders.Count();
            var linq = Orders.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();

            var declareprices = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderDeclareStatisticsView>().Where(item => linq.Select(a => a.ID).Contains(item.OrderID)).ToArray();

            var data = from order in linq
                       join declareprice in declareprices on order.ID equals declareprice.OrderID into _declareprices
                       select new OrderExtends
                       {
                           ID = order.ID,
                           Type = order.Type,
                           ClientID = order.ClientID,
                           SupplierID = order.SupplierID,
                           MainStatus = order.MainStatus,
                           PaymentStatus = order.PaymentStatus,
                           InvoiceStatus = order.InvoiceStatus,
                           RemittanceStatus = order.RemittanceStatus,
                           SettlementCurrency = order.SettlementCurrency,
                           CreateDate = order.CreateDate,
                           Output = new OrderOutput
                           {
                               ID = order.Output.ID,
                               Currency = order.Output.Currency,
                               Conditions = order.Output.Conditions,
                               IsReciveCharge = order.Output.IsReciveCharge,
                               WayBillID = order.Output.WayBillID,
                           },
                           TotalTraiff = _declareprices.Sum(item => item.CutomsPrice).GetValueOrDefault(),
                           TotalAddTax = _declareprices.Sum(item => item.DutyPrice).GetValueOrDefault(),
                           TotalAgencyFee = _declareprices.Sum(item => item.AgentPrice).GetValueOrDefault(),
                           TotalInspectionFee = _declareprices.Sum(item => item.otherPrice).GetValueOrDefault(),
                       };
            return new PageList<OrderExtends>(PageIndex, PageSize, data, total);
        }

        /// <summary>
        /// 获取未处理订单数量
        /// </summary>
        /// <returns></returns>
        public int GetUnHandleOrderCount(LambdaExpression[] expressions)
        {
            var Orders = GetOrders(expressions);
            var total = Orders.Count();
            return total;
        }

        /// <summary>
        /// 获取指定订单的数据
        /// </summary>
        /// <param name="ID">订单ID</param>
        /// <returns></returns>
        public DeclareOrder GetOrderDetail(string ID)
        {
            var order = this[ID];
            if (order == null)
            {
                return null;
            }
            //获取运单数据
            var inwaybill = new WayBillAlls(this.Reponsitory)[order.Input?.WayBillID];
            var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

            var Orderitems = new OrderItemAlls(this.Reponsitory).GetDeclareItemDetailByOrderID(order.ID);

            var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();

            var invoice = new MyInvoicesView(order.ClientID).Where(item => item.ID == order.InvoiceID).FirstOrDefault();

            //获取付汇供应商
            var PaySupplierids = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.MapsSupplier>().Where(item => ID == item.OrderID)
                .Select(item => item.SupplierID).ToArray();
            var supplier = new wsnSuppliersTopView<PvbCrmReponsitory>().Where(item => PaySupplierids.Contains(item.ID)).ToArray();

            return new DeclareOrder
            {
                ID = order.ID,
                Type = order.Type,
                ClientID = order.ClientID,
                InvoiceID = order.InvoiceID,
                Invoice = invoice,
                PayeeID = order.PayeeID,
                BeneficiaryID = order.BeneficiaryID,
                MainStatus = order.MainStatus,
                PaymentStatus = order.PaymentStatus,
                InvoiceStatus = order.InvoiceStatus,
                SettlementCurrency = order.SettlementCurrency,
                Summary = order.Summary,
                CreateDate = order.CreateDate,
                ModifyDate = order.ModifyDate,
                CreatorID = order.CreatorID,
                SupplierID = order.SupplierID,
                InWaybill = inwaybill,
                OutWaybill = outwaybill,
                OrderItems = Orderitems,
                Input = order.Type == OrderType.TransferDeclare ? null : new OrderInput
                {
                    ID = order.Input.ID,
                    BeneficiaryID = order.Input.BeneficiaryID,
                    Conditions = order.Input.Conditions,
                    Currency = order.Input.Currency,
                    IsPayCharge = order.Input.IsPayCharge,
                    WayBillID = order.Input.WayBillID
                },
                Output = new OrderOutput
                {
                    ID = order.Output.ID,
                    BeneficiaryID = order.Output.BeneficiaryID,
                    IsReciveCharge = order.Output.IsReciveCharge,
                    Conditions = order.Output.Conditions,
                    WayBillID = order.Output.WayBillID,
                    Currency = order.Output.Currency
                },
                OrderFiles = orderFiles,
                PayExchangeSupplier = supplier,
            };
        }

    }
}
