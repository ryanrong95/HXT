using Layers.Data.Sqls;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 发货订单
    /// </summary>
    public class DeliveryOrders : OrderViewBase
    {
        readonly IUser _user;

        public DeliveryOrders(IUser user) : base(user, OrderType.Delivery)
        {
            this._user = user;
        }

        public DeliveryOrders(IUser user, PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(user, reponsitory, iquery, OrderType.Delivery)
        {
            this._user = user;
        }

        #region 查询条件查询
        /// <summary>
        /// 根据供应商查询
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public DeliveryOrders SearchBySupplier(string supplier)
        {
            return new DeliveryOrders(this._user, this.Reponsitory, SearchBySupplierName(supplier));
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public DeliveryOrders SearchByPart(string partNumber)
        {
            return new DeliveryOrders(this._user, this.Reponsitory, SearchByPartNumber(partNumber));
        }
        #endregion

        /// <summary>
        /// 报关单获取列表数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override PageList<OrderExtends> GetPageListOrders(LambdaExpression[] expressions, int pageSize, int pageIndex)
        {
            var orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var total = orders.Count();
            var linq = orders.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();
            var waybills = new WayBillAlls(this.Reponsitory).Where(item => linq.Select(a => a.Output.WayBillID).Contains(item.ID));

            var data = from order in linq
                       join waybill in waybills on order.Output.WayBillID equals waybill.ID
                       select new OrderExtends
                       {
                           ID = order.ID,
                           MainStatus = order.MainStatus,
                           PaymentStatus = order.PaymentStatus,
                           CreateDate = order.CreateDate,
                           OutWaybill = waybill,
                       };

            return new PageList<OrderExtends>(pageIndex, pageSize, data, total);
        }

        /// <summary>
        /// 获取代收货款数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageList<OrderExtends> GetToChargeOrders(LambdaExpression[] expressions, int pageSize, int pageIndex)
        {
            var orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var amounts = from item in new ApplicationItemsOrigin(this.Reponsitory)
                          join application in new ApplicationsOrigin(this.Reponsitory) on item.ApplicationID equals application.ID
                          where application.Type == Enums.ApplicationType.Receival && item.Status == GeneralStatus.Normal && application.Status == GeneralStatus.Normal
                          group item by item.OrderID into _item
                          select new
                          {
                              OrderID = _item.Key,
                              TotalAmount = _item.Sum(a => a.Amount),
                          };

            var orderamounts = (from amount in amounts
                                join orderitem in new OrderItemAlls(this.Reponsitory) on amount.OrderID equals orderitem.OrderID
                                into _orderitems
                                select new
                                {
                                    amount.OrderID,
                                    amount.TotalAmount,
                                    TotalPrice = _orderitems.Any(item => item.Type == Enums.OrderItemType.Modified) ?
                                    _orderitems.Where(item => item.Type == Enums.OrderItemType.Modified).Sum(item => item.TotalPrice) :
                                    _orderitems.Where(item => item.Type == Enums.OrderItemType.Normal).Sum(item => item.TotalPrice)
                                }).Where(item => item.TotalPrice > item.TotalAmount).Select(item => item.OrderID);


            var data = from order in orders
                       join amount in orderamounts on order.ID equals amount into _amount
                       select order;

            var total = data.Count();
            var linq = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();

            var waybills = new WayBillAlls(this.Reponsitory).Where(item => linq.Select(a => a.Output.WayBillID).Contains(item.ID));

            var result = from order in linq
                         join waybill in waybills on order.Output.WayBillID equals waybill.ID
                         select new OrderExtends
                         {
                             ID = order.ID,
                             MainStatus = order.MainStatus,
                             PaymentStatus = order.PaymentStatus,
                             CreateDate = order.CreateDate,
                             SupplierID = order.SupplierID,
                             OutWaybill = waybill,
                             Output = new OrderOutput
                             {
                                 ID = order.Output.ID,
                                 Currency = order.Output.Currency,
                             }
                         };

            return new PageList<OrderExtends>(pageIndex, pageSize, result, total);
        }

        /// <summary>
        /// 获取订单详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeliveryOrder GetOrderDetail(string id)
        {
            var order = this[id];

            //获取运单数据
            var waybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

            var orderitems = new OrderItemAlls(this.Reponsitory).GetItemDetailByOrderID(order.ID);
            var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();
            //货物特殊要求
            var requirements = new OrderRequirementOrigin(this.Reponsitory).Where(item => item.OrderID == order.ID).ToArray().
                Select(item => new OrderRequirement
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    Type = item.Type,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    Requirement = item.Requirement,
                    RequireFiles = new CenterFilesView().FirstOrDefault(a => a.WsOrderID == item.ID),
                }).ToArray();

            var data = new DeliveryOrder
            {
                ID = order.ID,
                Type = order.Type,
                ClientID = order.ClientID,
                InvoiceID = order.InvoiceID,
                PayeeID = order.PayeeID,
                SettlementCurrency = order.SettlementCurrency,
                BeneficiaryID = order.BeneficiaryID,
                MainStatus = order.MainStatus,
                PaymentStatus = order.PaymentStatus,
                InvoiceStatus = order.InvoiceStatus,
                Summary = order.Summary,
                CreateDate = order.CreateDate,
                ModifyDate = order.ModifyDate,
                CreatorID = order.CreatorID,
                SupplierID = order.SupplierID,
                OutWaybill = waybill,
                OrderItems = orderitems,
                Output = new OrderOutput
                {
                    ID = order.Output.ID,
                    BeneficiaryID = order.Output.BeneficiaryID,
                    Conditions = order.Output.Conditions,
                    Currency = order.Output.Currency,
                    IsReciveCharge = order.Output.IsReciveCharge,
                    WayBillID = order.Output.WayBillID
                },
                OrderFiles = orderFiles,
                Requirements = requirements,
            };

            return data;
        }
    }
}
