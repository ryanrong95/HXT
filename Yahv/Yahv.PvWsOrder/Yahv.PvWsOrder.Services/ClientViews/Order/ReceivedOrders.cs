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
    /// 收货订单(待收货+即收即发)
    /// </summary>
    public class ReceivedOrders : OrderViewBase
    {
        readonly IUser _user;

        private ReceivedOrders()
        {

        }

        public ReceivedOrders(IUser user) : base(user, new OrderType[] { OrderType.Recieved, OrderType.Transport })
        {
            this._user = user;
        }

        public ReceivedOrders(IUser user, PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(user, reponsitory, iquery, new OrderType[] { OrderType.Recieved, OrderType.Transport })
        {
            this._user = user;
        }

        #region 查询条件查询
        /// <summary>
        /// 根据供应商查询
        /// </summary>
        /// <param name="supplier">供应商名称</param>
        /// <returns></returns>
        public ReceivedOrders SearchBySupplier(string supplier)
        {
            return new ReceivedOrders(this._user, this.Reponsitory, base.SearchBySupplierName(supplier));
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <returns></returns>
        public ReceivedOrders SearchByPart(string partNumber)
        {
            return new ReceivedOrders(this._user, this.Reponsitory, base.SearchByPartNumber(partNumber));
        }

        /// <summary>
        /// 根据MultiFiel查询
        /// </summary>
        /// <param name="multiField"></param>
        /// <returns></returns>
        public ReceivedOrders SearchByMultiField(string multiField)
        {
            return new ReceivedOrders(this._user, this.Reponsitory, base.SearchByMultiField(multiField));
        }
        #endregion


        /// <summary>
        /// 报关单获取列表数据
        /// </summary>
        /// <param name="expressions">过滤条件表达式</param>
        /// <param name="pageSize">每页展示数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public override PageList<OrderExtends> GetPageListOrders(LambdaExpression[] expressions, int pageSize, int pageIndex)
        {
            var orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var total = orders.Count();
            var linq = orders.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();

            var supplierIDs = linq.Select(t => t.SupplierID).ToArray();

            var wsnSuppliersTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSuppliersTopView>();
            var suppliers = wsnSuppliersTopView.Where(t => supplierIDs.Contains(t.ID)).ToArray();

            var data = from order in linq
                       join supplier in suppliers on order.SupplierID equals supplier.ID into suppliers2
                       from supplier in suppliers2.DefaultIfEmpty()
                       select new OrderExtends
                       {
                           ID = order.ID,
                           Type = order.Type,
                           ClientID = order.ClientID,
                           MainStatus = order.MainStatus,
                           PaymentStatus = order.PaymentStatus,
                           SettlementCurrency = order.SettlementCurrency,
                           CreateDate = order.CreateDate,
                           Input = new OrderInput
                           {
                               ID = order.Input.ID,
                               Currency = order.Input.Currency,
                           },
                           SupplierName2 = supplier != null ? supplier.EnglishName : "",
                       };

            return new PageList<OrderExtends>(pageIndex, pageSize, data, total);
        }

        /// <summary>
        /// 获取代付货款数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageList<OrderExtends> GetUnPayedOrders(LambdaExpression[] expressions, int pageSize, int pageIndex)
        {
            var orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var amounts = from item in new ApplicationItemsOrigin(this.Reponsitory)
                          join application in new ApplicationsOrigin(this.Reponsitory) on item.ApplicationID equals application.ID
                          where application.Type == Enums.ApplicationType.Payment && application.Status == GeneralStatus.Normal && item.Status == GeneralStatus.Normal
                          select new
                          {
                              item.OrderID,
                              item.Amount,
                          };

            var data = from order in orders
                       join amount in amounts on order.ID equals amount.OrderID into _amounts
                       where order.TotalPrice.GetValueOrDefault() > _amounts.Sum(item => (decimal?)item.Amount).GetValueOrDefault()
                       select order;
            
            var total = data.Count();
            var linq = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();

            var result = from order in linq
                         select new OrderExtends
                         {
                             ID = order.ID,
                             SupplierID = order.SupplierID,
                             Type = order.Type,
                             ClientID = order.ClientID,
                             MainStatus = order.MainStatus,
                             PaymentStatus = order.PaymentStatus,
                             SettlementCurrency = order.SettlementCurrency,
                             CreateDate = order.CreateDate,
                             Input = new OrderInput
                             {
                                 ID = order.Input.ID,
                                 Currency = order.Input.Currency,
                             },
                         };

            return new PageList<OrderExtends>(pageIndex, pageSize, result, total);
        }

        /// <summary>
        /// 获取指定订单的数据
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        public ReceivedOrder GetOrderDetail(string id)
        {
            var order = this[id];

            //获取运单数据
            var waybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];

            var orderitems = new OrderItemAlls(this.Reponsitory).GetItemDetailByOrderID(order.ID);
            var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();
            var data = new ReceivedOrder
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
                InWaybill = waybill,
                OrderItems = orderitems,
                Input = new OrderInput
                {
                    ID = order.Input.ID,
                    BeneficiaryID = order.Input.BeneficiaryID,
                    Conditions = order.Input.Conditions,
                    Currency = order.Input.Currency,
                    IsPayCharge = order.Input.IsPayCharge,
                    WayBillID = order.Input.WayBillID
                },
                OrderFiles = orderFiles,
            };

            //即收即发业务
            if (order.Type == OrderType.Transport)
            {
                data.OutWaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];
                data.Output = new OrderOutput
                {
                    ID = order.Output.ID,
                    BeneficiaryID = order.Output.BeneficiaryID,
                    Conditions = order.Output.Conditions,
                    Currency = order.Output.Currency,
                    IsReciveCharge = order.Output.IsReciveCharge,
                    WayBillID = order.Output.WayBillID
                };
                //货物特殊要求
                data.Requirements = new OrderRequirementOrigin(this.Reponsitory).Where(item => item.OrderID == order.ID).ToArray().
                    Select(item => new OrderRequirement
                    {
                        ID = item.ID,
                        OrderID = item.OrderID,
                        Type = item.Type,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice,
                        UnitPrice = item.UnitPrice,
                        Requirement = item.Requirement,
                        RequireFiles = new CenterFilesView().FirstOrDefault(a => a.WsOrderID == item.ID),
                    }).ToArray();
            }
            return data;
        }
    }
}
