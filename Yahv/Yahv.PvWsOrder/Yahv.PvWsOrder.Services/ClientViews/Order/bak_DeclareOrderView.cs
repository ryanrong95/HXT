using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    ///// <summary>
    ///// 报关订单视图
    ///// </summary>
    //public class DeclareOrderView : OrderViewBase
    //{

    //    private DeclareOrderView()
    //    {

    //    }

    //    private DeclareOrderView(IUser user) : base(user, OrderType.Declare)
    //    {

    //    }

    //    /// <summary>
    //    /// 报关订单列表数据查询
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
    //                        //join inwaybill in waybillView on entity.Input.WayBillID equals inwaybill.ID
    //                    //join outwaybill in waybillView on entity.Output.WayBillID equals outwaybill.ID
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
    //                        //ExecutionStatus = entity.ExecutionStatus,
    //                        InvoiceStatus = entity.InvoiceStatus,
    //                        //ConfirmStatus = entity.ConfirmStatus,
    //                        Summary = entity.Summary,
    //                        CreateDate = entity.CreateDate,
    //                        ModifyDate = entity.ModifyDate,
    //                        CreatorID = entity.CreatorID,
    //                        SettlementCurrency = entity.SettlementCurrency,
    //                        //Input = new OrderInput
    //                        //{
    //                        //    ID = entity.Input.ID,
    //                        //    BeneficiaryID = entity.Input.BeneficiaryID,
    //                        //    Conditions = entity.Input.Conditions,
    //                        //    Currency = entity.Input.Currency,
    //                        //    IsPayCharge = entity.Input.IsPayCharge,
    //                        //    WayBillID = entity.Input.WayBillID
    //                        //},
    //                        //InWaybill = inwaybill,
    //                        //Output = new OrderOutput
    //                        //{
    //                        //    ID = entity.Output.ID,
    //                        //    BeneficiaryID = entity.Output.BeneficiaryID,
    //                        //    Currency = entity.Output.Currency,
    //                        //    IsReciveCharge = entity.Output.IsReciveCharge,
    //                        //    Conditions = entity.Output.Conditions,
    //                        //    WayBillID = entity.Output.WayBillID
    //                        //},
    //                        //OutWaybill = outwaybill,
    //                        SupplierID = entity.SupplierID,
    //                    }).ToArray();
    //        return new PageList<OrderExtends>(PageIndex, PageSize, linq, orders.Total);
    //    }

    //    /// <summary>
    //    /// 获取指定订单的数据
    //    /// </summary>
    //    /// <param name="ID">订单ID</param>
    //    /// <returns></returns>
    //    public DeclareOrderExtends GetOrderDetail(string ID)
    //    {
    //        var order = this[ID];
    //        //获取运单数据
    //        var inwaybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];
    //        var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

    //        var Orderitems = new OrderItemAlls(this.Reponsitory).GetDeclareItemDetailByOrderID(order.ID);

    //        var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();

    //        var invoice = new MyInvoicesView(order.ClientID).Where(item => item.ID == order.InvoiceID).FirstOrDefault();
    //        var supplier = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.MapsSupplier>().Where(item => ID == item.OrderID)
    //                       join sup in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsSuppliersTopView>() on entity.SupplierID equals sup.ID
    //                       select new WsSupplier
    //                       {
    //                           ID = entity.SupplierID,
    //                           Name = sup.Name,
    //                       };

    //        return new DeclareOrderExtends
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
    //            //ExecutionStatus = order.ExecutionStatus,
    //            //ConfirmStatus = order.ConfirmStatus,
    //            InvoiceStatus = order.InvoiceStatus,
    //            SettlementCurrency = order.SettlementCurrency,
    //            Summary = order.Summary,
    //            CreateDate = order.CreateDate,
    //            ModifyDate = order.ModifyDate,
    //            CreatorID = order.CreatorID,
    //            SupplierID = order.SupplierID,
    //            InWaybill = inwaybill,
    //            OutWaybill = outwaybill,
    //            OrderItems = Orderitems,
    //            Input = new OrderInput
    //            {
    //                ID = order.Input.ID,
    //                BeneficiaryID = order.Input.BeneficiaryID,
    //                Conditions = order.Input.Conditions,
    //                Currency = order.Input.Currency,
    //                IsPayCharge = order.Input.IsPayCharge,
    //                WayBillID = order.Input.WayBillID
    //            },
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
    //            PayExchangeSupplier = supplier.ToArray()
    //        };
    //    }
    //}
}
