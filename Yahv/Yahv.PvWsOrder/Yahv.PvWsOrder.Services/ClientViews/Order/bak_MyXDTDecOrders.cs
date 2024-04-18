using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    ///// <summary>
    ///// 进口报关+转报关订单
    ///// </summary>
    //public class MyXDTDecOrders : OrderAlls
    //{
    //    IUser User;

    //    private MyXDTDecOrders(IUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<WsOrder> GetIQueryable()
    //    {
    //        var linq = base.GetIQueryable().Where(item => item.ClientID == User.EnterpriseID && (item.Type == OrderType.Declare || item.Type == OrderType.TransferDeclare));

    //        //if (!User.IsMain)
    //        //{
    //        //    linq = linq.Where(item => item.CreatorID == User.ID);
    //        //}
    //        return linq.OrderByDescending(item => item.CreateDate);
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

    //        var Orderitems = new OrderItemAlls(this.Reponsitory).GetItemBillByOrderID(order.ID);

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
    //            InvoiceStatus = order.InvoiceStatus,
    //            Summary = order.Summary,
    //            CreateDate = order.CreateDate,
    //            ModifyDate = order.ModifyDate,
    //            CreatorID = order.CreatorID,
    //            SupplierID = order.SupplierID,
    //            InWaybill = inwaybill,
    //            OutWaybill = outwaybill,
    //            OrderItems = Orderitems.ToArray(),
    //            SettlementCurrency = order.SettlementCurrency,
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
