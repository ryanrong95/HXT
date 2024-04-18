using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 所有订单视图
    /// </summary>
    public class OrderAlls : WsOrdersTopView<PvWsOrderReponsitory>
    {
        public OrderAlls()
        {

        }

        public OrderAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected OrderAlls(PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(reponsitory, iquery)
        {

        }

        #region 查询条件查询
        /// <summary>
        /// 根据供应商查询
        /// </summary>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        protected IQueryable<WsOrder> SearchBySupplierName(string supplierName)
        {
            return from order in this.IQueryable
                   join supplier in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSuppliersTopView>() on order.SupplierID equals supplier.ID
                   where supplier.ChineseName.Contains(supplierName) || supplier.EnglishName.Contains(supplierName)
                   select order;
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        protected IQueryable<WsOrder> SearchByPartNumber(string PartNumber)
        {
            var linq = from order in this.IQueryable
                       join item in new OrderItemAlls(this.Reponsitory) on order.ID equals item.OrderID
                       join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                       //where product.PartNumber.Contains(PartNumber)
                       where product.PartNumber == PartNumber
                       select order;
            return linq.Distinct();
        }

        /// <summary>
        /// 根据MultiFiel查询
        /// </summary>
        /// <param name="multiField"></param>
        /// <returns></returns>
        protected IQueryable<WsOrder> SearchByMultiField(string multiField)
        {
            var linq = from order in this.IQueryable
                       join supplier in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSuppliersTopView>() on order.SupplierID equals supplier.ID
                       where supplier.ChineseName.Contains(multiField) || supplier.EnglishName.Contains(multiField)
                          || order.ID.Contains(multiField)
                       select order;
            return linq.Distinct();
        }
        #endregion


        /// <summary>
        /// 根据传入参数获取订单数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<WsOrder> GetOrders(LambdaExpression[] expressions)
        {
            var orders = this.IQueryable;
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    orders = orders.Where(expression as Expression<Func<WsOrder, bool>>);
                }
            }
            return orders;
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        virtual public PageList<OrderExtends> GetPageListOrders(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            var orders = this.GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            int total = orders.Count();
            var linq = orders.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray().Select(item => new OrderExtends
            {
                ID = item.ID,
                Type = item.Type,
                ClientID = item.ClientID,
                InvoiceID = item.InvoiceID,
                PayeeID = item.PayeeID,
                BeneficiaryID = item.BeneficiaryID,
                MainStatus = item.MainStatus,
                PaymentStatus = item.PaymentStatus,
                InvoiceStatus = item.InvoiceStatus,
                RemittanceStatus = item.RemittanceStatus,
                SupplierID = item.SupplierID,
                Summary = item.Summary,
                CreateDate = item.CreateDate,
                ModifyDate = item.ModifyDate,
                CreatorID = item.CreatorID,
                SettlementCurrency = item.SettlementCurrency,
            });
            return new PageList<OrderExtends>(PageIndex, PageSize, linq, total);
        }


        /// <summary>
        /// 获取报关入库通知数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public OrderExtends GetDecNoticeDataByOrderID(string ID)
        {
            var order = this[ID];
            //非报关业务数据,返回null
            if (order.Type != OrderType.Declare && order.Type != OrderType.TransferDeclare)
            {
                return null;
            }
            //获取运单数据
            var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

            //获取订单项数据
            var Orderitems = from item in new OrderItemAlls(this.Reponsitory).SearchByOrderID(ID)
                             join terms in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals terms.ID
                             join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                             select new OrderItem
                             {
                                 ID = item.ID,
                                 Name = item.Name,
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
                                 StorageID = item.StorageID,
                                 ItemsTerm = new Models.OrderItemsTerm
                                 {
                                     ID = terms.ID,
                                     OriginRate = terms.OriginRate,
                                     FVARate = terms.FVARate,
                                     Ccc = terms.Ccc,
                                     Embargo = terms.Embargo,
                                     HkControl = terms.HkControl,
                                     Coo = terms.Coo,
                                     CIQ = terms.CIQ,
                                     CIQprice = terms.CIQprice,
                                     IsHighPrice = terms.IsHighPrice,
                                     IsDisinfected = terms.IsDisinfected,
                                 },
                             };

            var data = new OrderExtends
            {
                ID = order.ID,
                Type = order.Type,
                ClientID = order.ClientID,
                InvoiceID = order.InvoiceID,
                PayeeID = order.PayeeID,
                BeneficiaryID = order.BeneficiaryID,
                MainStatus = order.MainStatus,
                PaymentStatus = order.PaymentStatus,
                InvoiceStatus = order.InvoiceStatus,
                Summary = order.Summary,
                CreateDate = order.CreateDate,
                ModifyDate = order.ModifyDate,
                CreatorID = order.CreatorID,
                SupplierID = order.SupplierID,
                OrderItems = Orderitems.ToArray(),
                OutWaybill = outwaybill,
            };
            //转报关没有Inwaybill数据
            if (order.Type == OrderType.Declare)
            {
                data.InWaybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];
            }
            return data;
        }


        /// <summary>
        /// 确认操作
        /// </summary>
        /// <param name="order"></param>
        public void Confirm(OrderExtends order, string UserID)
        {
            //校验库存数量是否足够
            if (order.Type == OrderType.TransferDeclare)
            {
                var linq = from item in order.OrderItems
                           join storage in new StoragesTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.StorageID equals storage.ID
                           where item.Quantity > storage.Quantity
                           select item.Product.PartNumber;

                if (linq.Any())
                {
                    throw new Exception("库存不足,型号为" + linq.First());
                }
            }

            //确认状态更新
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //原有状态置为失效
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false,
                }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.MainStatus);
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.MainStatus,
                    Status = (int)CgOrderStatus.待交货,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });
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

            #region 生成库房入库通知
            string message;
            if (order.Type == OrderType.Declare)
            {
                // 2022-01-19 香港库房重构 ，不需要发入库通知
                // message = Extends.NoticeExtends.CgDecEntryNotice(order);
                message = "";
            }
            else
            {
                message = Extends.NoticeExtends.CgDecBoxingNotice(order);
            }
            //调用结果日志
            Logger.log(UserID, new OperatingLog
            {
                MainID = order.ID,
                Operation = "报关订单库房通知对接结果日志！",
                Summary = message,
            });
            #endregion

            #region 芯达通接口同步数据
            Task.Run(() =>
            {
                var orderconfirm = new XDTModels.OrderConfirmed()
                {
                    OrderID = order.ID,
                    UserID = UserID,
                    IsCancel = false,
                    CancelReason = string.Empty,
                };

                message = Extends.NoticeExtends.XDTOrderConfirm(orderconfirm);
                //调用结果日志
                Logger.log(UserID, new OperatingLog
                {
                    MainID = order.ID,
                    Operation = "芯达通订单确认对接结果日志！",
                    Summary = message,
                });
            });
            #endregion
        }
    }
}
