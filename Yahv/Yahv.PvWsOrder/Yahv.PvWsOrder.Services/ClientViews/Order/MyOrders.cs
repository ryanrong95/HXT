using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using OrderInput = Yahv.PvWsOrder.Services.ClientModels.OrderInput;
using OrderItem = Yahv.PvWsOrder.Services.ClientModels.OrderItem;
using OrderOutput = Yahv.PvWsOrder.Services.ClientModels.OrderOutput;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyOrders : OrderAlls
    {
        IUser User;

        internal protected MyOrders()
        {

        }

        public MyOrders(IUser user)
        {
            this.User = user;
        }

        protected override IQueryable<WsOrder> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => item.ClientID == User.EnterpriseID);

            if (!User.IsMain)
            {
                linq = linq.Where(item => item.CreatorID == User.ID);
            }
            return linq.OrderByDescending(item => item.CreateDate);
        }

        /// <summary>
        /// 全部订单列表数据查询
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<OrderExtends> GetPageData(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            var orders = base.GetPageListOrders(expressions, PageSize, PageIndex);
            var waybillView = new WayBillAlls(this.Reponsitory);
            var linq = (from entity in orders.OrderByDescending(item => item.CreateDate)
                        select new OrderExtends
                        {
                            ID = entity.ID,
                            Type = entity.Type,
                            ClientID = entity.ClientID,
                            InvoiceID = entity.InvoiceID,
                            PayeeID = entity.PayeeID,
                            BeneficiaryID = entity.BeneficiaryID,
                            MainStatus = entity.MainStatus,
                            PaymentStatus = entity.PaymentStatus,
                            //ExecutionStatus = entity.ExecutionStatus,
                            InvoiceStatus = entity.InvoiceStatus,
                            //ConfirmStatus=entity.ConfirmStatus,
                            RemittanceStatus = entity.RemittanceStatus,
                            Summary = entity.Summary,
                            CreateDate = entity.CreateDate,
                            ModifyDate = entity.ModifyDate,
                            CreatorID = entity.CreatorID,
                            SettlementCurrency = entity.SettlementCurrency,
                            Input = new OrderInput
                            {
                                ID = entity.Input.ID,
                                BeneficiaryID = entity.Input.BeneficiaryID,
                                Conditions = entity.Input.Conditions,
                                WayBillID = entity.Input.WayBillID,
                                Currency = entity.Input.Currency
                            },
                            Output = new OrderOutput
                            {
                                ID = entity.Output.ID,
                                BeneficiaryID = entity.Output.BeneficiaryID,
                                IsReciveCharge = entity.Output.IsReciveCharge,
                                Conditions = entity.Output.Conditions,
                                WayBillID = entity.Output.WayBillID,
                                Currency = entity.Output.Currency
                            },
                        }).ToArray();
            return new PageList<OrderExtends>(PageIndex, PageSize, linq, orders.Total);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public OrderExtends GetOrderDetail(string ID)
        {
            var order = this[ID];
            var invoice = new InvoicesTopView<PvWsOrderReponsitory>()[order.InvoiceID];
            var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();
            var Orderitems = from item in new OrderItemAlls(this.Reponsitory).SearchByOrderID(order.ID)
                             join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                             select new OrderItem
                             {
                                 ID = item.ID,
                                 OrderID = item.OrderID,
                                 InputID = item.InputID,
                                 ProductID = item.ProductID,
                                 TinyOrderID = item.TinyOrderID,
                                 Origin = item.Origin,
                                 DateCode = item.DateCode,
                                 Quantity = item.Quantity,
                                 Currency = item.Currency,
                                 UnitPrice = item.UnitPrice,
                                 Unit = item.Unit,
                                 TotalPrice = item.TotalPrice,
                                 CreateDate = item.CreateDate,
                                 ModifyDate = item.ModifyDate,
                                 GrossWeight = item.GrossWeight,
                                 Volume = item.Volume,
                                 Conditions = item.Conditions,
                                 Status = item.Status,
                                 Product = product,
                             };
            return new OrderExtends
            {
                ID = order.ID,
                ClientID = order.ClientID,
                Invoice = invoice,
                SupplierID = order.SupplierID,
                OrderFiles = orderFiles,
                PaymentStatus = order.PaymentStatus,
                OrderItems = Orderitems.ToArray(),
                SettlementCurrency = order.SettlementCurrency,
                CreatorID = order.CreatorID,
            };
        }


        /// <summary>
        /// 获取需要代收（付）款的订单信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IQueryable<ApplicationItem> GetApplyOrders(ApplicationType type, string[] ids)
        {
            var orders = GetIQueryable().Where(m => ids.Contains(m.ID));
            var applys = from apps in new ApplicationsOrigin(this.Reponsitory)
                         join items in new ApplicationItemsOrigin(this.Reponsitory) on apps.ID equals items.ApplicationID
                         where apps.Type == type && apps.Status == GeneralStatus.Normal && items.Status == GeneralStatus.Normal
                         select new
                         {
                             items.OrderID,
                             items.Amount
                         };
            IQueryable<ApplicationItem> linq = null;
            if (type == ApplicationType.Payment)
            {
                linq = from order in orders
                       join apply in applys on order.ID equals apply.OrderID into _apply
                       select new ApplicationItem()
                       {
                           OrderID = order.ID,
                           SettlementCurrency = order.Input.Currency,
                           AppliedPrice = _apply.Sum(m => (decimal?)m.Amount).GetValueOrDefault(),
                           SupplierID = order.SupplierID
                       };
            }
            else
            {
                linq = from order in orders
                       join apply in applys on order.ID equals apply.OrderID into _apply
                       select new ApplicationItem()
                       {
                           OrderID = order.ID,
                           SettlementCurrency = order.Output.Currency,
                           AppliedPrice = _apply.Sum(m => (decimal?)m.Amount).GetValueOrDefault(),
                           SupplierID = order.SupplierID
                       };
            }

            return linq;
        }

        /// <summary>
        /// 获取报关委托书数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public XDTModels.OrderAgentProxy GetOrderAgentProxy(string ID)
        {
            var order = this[ID];
            var client = new WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory)[order.ClientID];
            var contact = new ContactsTopView<PvWsOrderReponsitory>(Business.WarehouseServicing, this.Reponsitory).FirstOrDefault(item => item.EnterpriseID == order.ClientID);
            var Waybill = new WaybillsTopView<PvWsOrderReponsitory>(this.Reponsitory)[order.Output.WayBillID];

            var orderitems = new OrderItemAlls(this.Reponsitory).GetClassfiedItemByOrderID(ID);

            return new XDTModels.OrderAgentProxy
            {
                ID = order.ID,
                Order = order,
                Client = client,
                ClientContact = contact,
                Orderitems = orderitems,
                PackNo = Waybill.TotalParts,
            };
        }


        /// <summary>
        /// 获取报关对账单数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public XDTModels.OrderBillProxy GetOrderBillProxy(string ID)
        {
            var order = this[ID];
            if (order == null)
            {
                return null;
            }
            var client = new WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory)[order.ClientID];
            var contact = new ContactsTopView<PvWsOrderReponsitory>(this.Reponsitory).FirstOrDefault(item => item.EnterpriseID == order.ClientID);

            OrderItemAlls orderItemAlls = new OrderItemAlls(this.Reponsitory);
            var orderitems = orderItemAlls.GetItemBillByOrderID(order.ID);
            var tinyOrderRates = orderItemAlls.GetTinyOrderRates(order.ID);
            for (int i = 0; i < orderitems.Length; i++)
            {
                var tinyOrderRate = tinyOrderRates.Where(t => t.TinyOrderID == orderitems[i].TinyOrderID).FirstOrDefault();
                decimal realRate = tinyOrderRate != null ? tinyOrderRate.RealRate : 0;
                decimal customsRate = tinyOrderRate != null ? tinyOrderRate.CustomsRate : 0;

                orderitems[i].RealExchangeRate = realRate;
                orderitems[i].CustomsExchangeRate = customsRate;
                orderitems[i].DeclareTotalPrice = orderitems[i].TotalPrice * realRate;
            }

            return new XDTModels.OrderBillProxy
            {
                ContrNo = "",
                Order = order,
                Client = client,
                ClientContact = contact,
                orderitems = orderitems.ToArray(),
            };
        }


        /// <summary>
        /// 仓储业务确认账单
        /// </summary>
        /// <param name="OrderID"></param>
        public void ConfirmBill(string OrderID)
        {
            var order = this[OrderID];
            if (order == null)
            {
                throw new Exception("订单不存在！");
            }
            //支付状态更新
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //修改主状态
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false,
                }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.PaymentStatus);
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.PaymentStatus,
                    Status = (int)OrderPaymentStatus.ToBePaid,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });
            }
        }


        /// <summary>
        /// 仓储业务不同意账单,取消订单
        /// </summary>
        /// <param name="OrderID"></param>
        public void CancelBill(string OrderID)
        {
            var order = this[OrderID];
            if (order == null)
            {
                throw new Exception("订单不存在！");
            }
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //修改主状态
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false,
                }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.MainStatus);
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.MainStatus,
                    Status = (int)CgOrderStatus.取消,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });

                var waybillid = order.Type == OrderType.Delivery ? order.Output.WayBillID : order.Input.WayBillID;
                //修改运单状态为关闭
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Status = (int)GeneralStatus.Closed,
                    ModifyDate = DateTime.Now,
                }, item => item.ID == waybillid);
            }
        }
    }
}
