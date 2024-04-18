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
    /// <summary>
    /// 转报关订单视图
    /// </summary>
    //public class TransDeclareOrderView : OrderViewBase
    //{

    //    private TransDeclareOrderView()
    //    {

    //    }

    //    private TransDeclareOrderView(IUser user) : base(user, OrderType.TransferDeclare)
    //    {

    //    }

    //    /// <summary>
    //    /// 转报关订单列表数据查询
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
    //                    join outwaybill in waybillView on entity.Output.WayBillID equals outwaybill.ID
    //                    select new OrderExtends
    //                    {
    //                        ID = entity.ID,
    //                        Type = entity.Type,
    //                        ClientID = entity.ClientID,
    //                        InvoiceID = entity.InvoiceID,
    //                        PayeeID = entity.PayeeID,
    //                        BeneficiaryID = entity.BeneficiaryID,
    //                        MainStatus = entity.MainStatus,
    //                        //ConfirmStatus = entity.ConfirmStatus,
    //                        PaymentStatus = entity.PaymentStatus,
    //                        //ExecutionStatus = entity.ExecutionStatus,
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
    //                            Currency = entity.Output.Currency,
    //                            IsReciveCharge = entity.Output.IsReciveCharge,
    //                            Conditions = entity.Output.Conditions,
    //                            WayBillID = entity.Output.WayBillID
    //                        },
    //                        OutWaybill = outwaybill,
    //                        SupplierID = entity.SupplierID,
    //                    }).ToArray();
    //        return new PageList<OrderExtends>(PageIndex, PageSize, linq, orders.Total);
    //    }

    //    /// <summary>
    //    /// 获取指定订单的数据
    //    /// </summary>
    //    /// <param name="ID">订单ID</param>
    //    /// <returns></returns>
    //    public TransferEntryOrderExtends GetOrderDetail(string ID)
    //    {
    //        var order = this[ID];
    //        //获取运单数据
    //        var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

    //        var Orderitems = new OrderItemAlls(this.Reponsitory).GetItemDetailByOrderID(order.ID);
    //        var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();
    //        var invoice = new MyInvoicesView(order.ClientID).Where(item => item.ID == order.InvoiceID).FirstOrDefault();
    //        var supplier = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.MapsSupplier>().Where(item => ID == item.OrderID)
    //                       join sup in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsSuppliersTopView>() on entity.SupplierID equals sup.ID
    //                       select new WsSupplier
    //                       {
    //                           ID = entity.SupplierID,
    //                           Name = sup.Name,
    //                       };

    //        return new TransferEntryOrderExtends
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
    //            InvoiceStatus = order.InvoiceStatus,
    //            Summary = order.Summary,
    //            SettlementCurrency = order.SettlementCurrency,
    //            CreateDate = order.CreateDate,
    //            ModifyDate = order.ModifyDate,
    //            CreatorID = order.CreatorID,
    //            SupplierID = order.SupplierID,
    //            OutWaybill = outwaybill,
    //            OrderItems = Orderitems,
    //            Output = new OrderOutput
    //            {
    //                ID = order.Output.ID,
    //                BeneficiaryID = order.Output.BeneficiaryID,
    //                IsReciveCharge = order.Output.IsReciveCharge,
    //                Conditions = order.Output.Conditions,
    //                WayBillID = order.Output.WayBillID,
    //                Currency = order.Output.Currency,
    //            },
    //            OrderFiles = orderFiles,
    //            PayExchangeSupplier = supplier.ToArray()
    //        };
    //    }
    //}
}
