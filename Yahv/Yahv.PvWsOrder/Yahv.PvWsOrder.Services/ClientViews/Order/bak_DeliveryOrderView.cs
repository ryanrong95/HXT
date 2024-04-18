using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Layers.Data.Sqls;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    ///// <summary>
    ///// 代发货订单视图
    ///// </summary>
    //public class DeliveryOrderView : OrderViewBase
    //{

    //    private DeliveryOrderView()
    //    {

    //    }

    //    private DeliveryOrderView(IUser user) : base(user, OrderType.Delivery)
    //    {

    //    }

    //    /// <summary>
    //    /// 代发货订单列表数据查询
    //    /// </summary>
    //    /// <param name="expressions"></param>
    //    /// <param name="PageSize"></param>
    //    /// <param name="PageIndex"></param>
    //    /// <returns></returns>
    //    public PageList<OrderExtends> GetPageData(LambdaExpression[] expressions, int PageSize, int PageIndex)
    //    {

    //        var orders = base.GetPageListOrders(expressions, PageSize, PageIndex);
    //        var waybillView = new WayBillAlls(this.Reponsitory);
    //        var linq = (from entity in orders.OrderByDescending(item => item.CreateDate)
    //                    join waybill in waybillView on entity.Output.WayBillID equals waybill.ID
    //                    select new OrderExtends
    //                    {
    //                        ID = entity.ID,
    //                        Type = entity.Type,
    //                        ClientID = entity.ClientID,
    //                        InvoiceID = entity.InvoiceID,
    //                        PayeeID = entity.PayeeID,
    //                        BeneficiaryID = entity.BeneficiaryID,
    //                        MainStatus = entity.MainStatus,
    //                        PaymentStatus = entity.PaymentStatus,
    //                        InvoiceStatus = entity.InvoiceStatus,
    //                        Summary = entity.Summary,
    //                        CreateDate = entity.CreateDate,
    //                        ModifyDate = entity.ModifyDate,
    //                        CreatorID = entity.CreatorID,
    //                        SettlementCurrency = entity.SettlementCurrency,
    //                        Output = new OrderOutput
    //                        {
    //                            ID = entity.Output.ID,
    //                            BeneficiaryID = entity.Output.BeneficiaryID,
    //                            IsReciveCharge = entity.Output.IsReciveCharge,
    //                            Conditions = entity.Output.Conditions,
    //                            WayBillID = entity.Output.WayBillID,
    //                            Currency = entity.Output.Currency
    //                        },
    //                        OutWaybill = waybill,
    //                    }).ToArray();
    //        return new PageList<OrderExtends>(PageIndex, PageSize, linq, orders.Total);
    //    }

    //    /// <summary>
    //    /// 获取指定订单的数据
    //    /// </summary>
    //    /// <param name="ID">订单ID</param>
    //    /// <returns></returns>
    //    public OrderExtends GetOrderDetail(string ID)
    //    {
    //        var order = this[ID];
    //        //获取运单数据
    //        var waybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];
    //        var Orderitems = new OrderItemAlls(this.Reponsitory).GetItemDetailByOrderID(order.ID);
    //        var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();
    //        var invoice = new MyInvoicesView(order.ClientID).Where(item => item.ID == order.InvoiceID).FirstOrDefault();

    //        return new OrderExtends
    //        {
    //            ID = order.ID,
    //            Type = order.Type,
    //            ClientID = order.ClientID,
    //            InvoiceID = order.InvoiceID,
    //            Invoice = invoice,
    //            PayeeID = order.PayeeID,
    //            BeneficiaryID = order.BeneficiaryID,
    //            MainStatus = order.MainStatus,
    //            PaymentStatus = order.PaymentStatus,
    //            InvoiceStatus = order.InvoiceStatus,
    //            Summary = order.Summary,
    //            SettlementCurrency = order.SettlementCurrency,
    //            CreateDate = order.CreateDate,
    //            ModifyDate = order.ModifyDate,
    //            CreatorID = order.CreatorID,
    //            SupplierID = order.SupplierID,
    //            OutWaybill = waybill,
    //            OrderItems = Orderitems,
    //            Output = new OrderOutput
    //            {
    //                ID = order.Output.ID,
    //                BeneficiaryID = order.Output.BeneficiaryID,
    //                IsReciveCharge = order.Output.IsReciveCharge,
    //                Conditions = order.Output.Conditions,
    //                WayBillID = order.Output.WayBillID,
    //                Currency = order.Output.Currency
    //            },
    //            OrderFiles = orderFiles,
    //        };
    //    }
    //}
}
