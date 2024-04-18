using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;
using System.Linq.Expressions;
using Yahv.Services.Views;
using Layers.Data.Sqls;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Models;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    ///// <summary>
    ///// 仓储收货订单视图
    ///// </summary>
    //public class RecievedOrderView : OrderViewBase
    //{
    //    private RecievedOrderView()
    //    {

    //    }

    //    private RecievedOrderView(IUser user) : base(user, OrderType.Recieved)
    //    {
    //    }


    //    public void GetData(LambdaExpression[] expressions)
    //    {
    //        //Expression<Func<Order, bool>> lambda = item => item.ClientID == User.EnterpriseID;
    //        //expressions.Concat(new[] { lambda });
    //        var orders = this.GetOrders(expressions).OrderByDescending(item => item.CreateDate);
    //        //连接你需要的其他对象
    //    }


    //    /// <summary>
    //    /// 代入仓订单列表数据查询
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
    //                    join waybill in waybillView on entity.Input.WayBillID equals waybill.ID
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
    //                        Summary = entity.Summary,
    //                        CreateDate = entity.CreateDate,
    //                        ModifyDate = entity.ModifyDate,
    //                        CreatorID = entity.CreatorID,
    //                        Input = new OrderInput
    //                        {
    //                            ID = entity.Input.ID,
    //                            BeneficiaryID = entity.Input.BeneficiaryID,
    //                            Conditions = entity.Input.Conditions,
    //                            Currency = entity.Input.Currency,
    //                            IsPayCharge = entity.Input.IsPayCharge,
    //                            WayBillID = entity.Input.WayBillID
    //                        },
    //                        InWaybill = waybill,
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
    //        var waybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];

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
    //            SettlementCurrency=order.SettlementCurrency,
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
    //            InWaybill = waybill,
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
    //            OrderFiles = orderFiles,
    //        };
    //    }
    //}
}
