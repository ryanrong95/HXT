//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Layers.Data.Sqls;
//using Layers.Linq;
//using Layers.Data.Sqls.PvData;
//using Yahv.Underly;
//using Yahv.Services.Models;
//using Yahv.PvWsOrder.Services.ClientModels;

//namespace Yahv.PvWsOrder.ClassifyService
//{
//    public class test
//    {
//        public void test1()
//        {
//            //this.ClientConfirm();
//        }

//        /// <summary>
//        /// 客户确认
//        /// </summary>
//        public void ClientConfirm()
//        {
//            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
//            {
//                var Orders = new PvWsOrder.Services.ClientViews.OrderAlls(reponsitory)
//                    .Where(item => item.MainStatus == Yahv.Underly.CgOrderStatus.待确认);
//                using (PvCenterReponsitory centerReponsitory = LinqFactory<PvCenterReponsitory>.Create())
//                {
//                    foreach (var order in Orders)
//                    {
//                        //原有状态置为失效
//                        centerReponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
//                        {
//                            IsCurrent = false,
//                        }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.MainStatus);
//                        centerReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
//                        {
//                            ID = Guid.NewGuid().ToString(),
//                            MainID = order.ID,
//                            Type = (int)OrderStatusType.MainStatus,
//                            Status = (int)CgOrderStatus.待收货,
//                            CreateDate = DateTime.Now,
//                            CreatorID = order.CreatorID,
//                            IsCurrent = true,
//                        });

//                        #region 生成库房入库通知
//                        string message;
//                        var data = this.GetOrderExtends(order, reponsitory);
//                        if (order.Type == OrderType.Declare)
//                        {
//                            message = PvWsOrder.Services.Extends.NoticeExtends.CgDecEntryNotice(data);
//                        }
//                        else
//                        {
//                            message = PvWsOrder.Services.Extends.NoticeExtends.CgDecBoxingNotice(data);
//                        }

//                        //调用结果日志
//                        PvWsOrder.Services.Logger.log("User1000000188", new OperatingLog
//                        {
//                            MainID = order.ID,
//                            Operation = message,
//                            Summary = "报关订单库房通知对接结果日志！",
//                        });
//                        #endregion

//                        #region 芯达通接口同步数据
//                        Task.Run(() =>
//                        {
//                            //调用芯达通接口同步数据
//                            var orderconfirm = new PvWsOrder.Services.XDTModels.OrderConfirmed()
//                            {
//                                OrderID = order.ID,
//                                UserID = "User1000000188",
//                                IsCancel = false,
//                                CancelReason = string.Empty,
//                            };

//                            message = PvWsOrder.Services.Extends.NoticeExtends.XDTOrderConfirm(orderconfirm);
//                            //调用结果日志
//                            PvWsOrder.Services.Logger.log("User1000000188", new OperatingLog
//                            {
//                                MainID = order.ID,
//                                Operation = message,
//                                Summary = "芯达通订单确认对接结果日志！",
//                            });
//                        });
//                        #endregion
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="order"></param>
//        /// <returns></returns>
//        public PvWsOrder.Services.ClientModels.OrderExtends GetOrderExtends(WsOrder order, PvWsOrderReponsitory reponsitory)
//        {
//            //获取运单数据
//            var outwaybill = new Services.ClientViews.WayBillAlls(reponsitory)[order.Output.WayBillID];

//            //获取订单项数据
//            var Orderitems = from item in new Services.ClientViews.OrderItemAlls(reponsitory).SearchByOrderID(order.ID)
//                             join terms in reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals terms.ID
//                             join product in new Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>(reponsitory) on item.ProductID equals product.ID
//                             select new OrderItem
//                             {
//                                 ID = item.ID,
//                                 Name = item.Name,
//                                 OrderID = item.OrderID,
//                                 InputID = item.InputID,
//                                 ProductID = item.ProductID,
//                                 TinyOrderID = item.TinyOrderID,
//                                 Origin = item.Origin,
//                                 DateCode = item.DateCode,
//                                 Quantity = item.Quantity,
//                                 Currency = item.Currency,
//                                 UnitPrice = item.UnitPrice,
//                                 Unit = item.Unit,
//                                 TotalPrice = item.TotalPrice,
//                                 CreateDate = item.CreateDate,
//                                 ModifyDate = item.ModifyDate,
//                                 GrossWeight = item.GrossWeight,
//                                 Volume = item.Volume,
//                                 Conditions = item.Conditions,
//                                 Status = item.Status,
//                                 Product = product,
//                                 StorageID = item.StorageID,
//                                 ItemsTerm = new PvWsOrder.Services.Models.OrderItemsTerm
//                                 {
//                                     ID = terms.ID,
//                                     OriginRate = terms.OriginRate,
//                                     FVARate = terms.FVARate,
//                                     Ccc = terms.Ccc,
//                                     Embargo = terms.Embargo,
//                                     HkControl = terms.HkControl,
//                                     Coo = terms.Coo,
//                                     CIQ = terms.CIQ,
//                                     CIQprice = terms.CIQprice,
//                                     IsHighPrice = terms.IsHighPrice,
//                                     IsDisinfected = terms.IsDisinfected,
//                                 },
//                             };
//            var data = new PvWsOrder.Services.ClientModels.OrderExtends
//            {
//                ID = order.ID,
//                Type = order.Type,
//                ClientID = order.ClientID,
//                InvoiceID = order.InvoiceID,
//                PayeeID = order.PayeeID,
//                MainStatus = order.MainStatus,
//                PaymentStatus = order.PaymentStatus,
//                InvoiceStatus = order.InvoiceStatus,
//                Summary = order.Summary,
//                CreateDate = order.CreateDate,
//                ModifyDate = order.ModifyDate,
//                CreatorID = order.CreatorID,
//                SupplierID = order.SupplierID,
//                OrderItems = Orderitems.ToArray(),
//                OutWaybill = outwaybill,
//                Input = new OrderInput
//                {
//                    ID = order.Input.ID,
//                    BeneficiaryID = order.Input.BeneficiaryID,
//                    Conditions = order.Input.Conditions,
//                    Currency = order.Input.Currency,
//                    IsPayCharge = order.Input.IsPayCharge,
//                    WayBillID = order.Input.WayBillID
//                },
//                Output = new OrderOutput
//                {
//                    ID = order.Output.ID,
//                    BeneficiaryID = order.Output.BeneficiaryID,
//                    IsReciveCharge = order.Output.IsReciveCharge,
//                    Conditions = order.Output.Conditions,
//                    WayBillID = order.Output.WayBillID,
//                    Currency = order.Output.Currency
//                },
//            };
//            //转报关没有Inwaybill数据
//            if (order.Type == OrderType.Declare)
//            {
//                data.InWaybill = new Services.ClientViews.WayBillAlls(reponsitory)[order.Input.WayBillID];
//            }
//            return data;
//        }
//    }
//}
