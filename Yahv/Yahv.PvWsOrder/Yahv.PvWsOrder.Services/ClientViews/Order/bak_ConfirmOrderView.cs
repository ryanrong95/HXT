using Layers.Data.Sqls;
using Layers.Linq;
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
    ///// 带客户确认订单视图 
    ///// </summary>
    //public class ConfirmOrderView : OrderAlls
    //{
    //    IUser User;

    //    private ConfirmOrderView(IUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<WsOrder> GetIQueryable()
    //    {
    //        var linq = base.GetIQueryable().Where(item => item.ClientID == this.User.EnterpriseID).
    //            Where(item => item.MainStatus == CgOrderStatus.待确认); //TODO: 修改待确认

    //        //if (!User.IsMain)
    //        //{
    //        //    linq = linq.Where(item => item.CreatorID == User.ID);
    //        //}
    //        return linq.OrderByDescending(item => item.CreateDate);
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
    //        var waybillView = new WayBillAlls(this.Reponsitory).Where(item => orders.Select(a => a.Output.WayBillID).Contains(item.ID)).ToArray();
    //        var orderitems = new OrderItemAlls(this.Reponsitory).Where(item => orders.Select(a => a.ID).Contains(item.OrderID)).ToArray();
    //        var linq = (from entity in orders.OrderByDescending(item => item.CreateDate)
    //                    join outwaybill in waybillView on entity.Output.WayBillID equals outwaybill.ID
    //                    join orderitem in orderitems on entity.ID equals orderitem.OrderID into items
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
    //                        Input = new OrderInput
    //                        {
    //                            ID = entity.Input.ID,
    //                            BeneficiaryID = entity.Input.BeneficiaryID,
    //                            Conditions = entity.Input.Conditions,
    //                            Currency = entity.Input.Currency,
    //                            IsPayCharge = entity.Input.IsPayCharge,
    //                            WayBillID = entity.Input.WayBillID
    //                        },
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
    //                        OrderItems = items.ToArray(),
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
    //        var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

    //        //获取对账单生成的实时汇率
    //        decimal RealRate = 0;

    //        using (ScCustomReponsitory res = new ScCustomReponsitory())
    //        {
    //            RealRate = res.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().FirstOrDefault(item => item.MainOrderId == ID).RealExchangeRate.GetValueOrDefault();
    //        }

    //        var Orderitems = new OrderItemAlls(this.Reponsitory).GetItemBillByOrderID(ID);
    //        var orderFiles = new CenterFilesView().SearchByOrderID(order.ID).ToArray();
    //        var invoice = new MyInvoicesView(order.ClientID).Where(item => item.ID == order.InvoiceID).FirstOrDefault();

    //        var supplier = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.MapsSupplier>().Where(item => ID == item.OrderID)
    //                       join sup in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WsSuppliersTopView>() on entity.SupplierID equals sup.ID
    //                       select new WsSupplier
    //                       {
    //                           ID = entity.SupplierID,
    //                           Name = sup.Name,
    //                       };

    //        var data = new DeclareOrderExtends
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
    //            SettlementCurrency = order.SettlementCurrency,
    //            Summary = order.Summary,
    //            CreateDate = order.CreateDate,
    //            ModifyDate = order.ModifyDate,
    //            CreatorID = order.CreatorID,
    //            SupplierID = order.SupplierID,
    //            OutWaybill = outwaybill,
    //            OrderItems = Orderitems.ToArray(),
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
    //        //转报关没有Inwaybill数据
    //        if (order.Type == OrderType.Declare)
    //        {
    //            data.InWaybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];
    //        }
    //        return data;
    //    }


    //    /// <summary>
    //    /// 获取报关入库通知数据
    //    /// </summary>
    //    /// <param name="ID"></param>
    //    /// <returns></returns>
    //    public OrderExtends GetEntryNoticeDataByOrderID(string ID)
    //    {
    //        var order = this[ID];
    //        //获取运单数据
    //        var outwaybill = new WayBillAlls(this.Reponsitory)[order.Output.WayBillID];

    //        //获取订单项数据
    //        var Orderitems = from item in new OrderItemAlls(this.Reponsitory).SearchByOrderID(ID)
    //                         join terms in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals terms.ID
    //                         join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
    //                         select new OrderItem
    //                         {
    //                             ID = item.ID,
    //                             Name = item.Name,
    //                             OrderID = item.OrderID,
    //                             InputID = item.InputID,
    //                             ProductID = item.ProductID,
    //                             TinyOrderID = item.TinyOrderID,
    //                             Origin = item.Origin,
    //                             DateCode = item.DateCode,
    //                             Quantity = item.Quantity,
    //                             Currency = item.Currency,
    //                             UnitPrice = item.UnitPrice,
    //                             Unit = item.Unit,
    //                             TotalPrice = item.TotalPrice,
    //                             CreateDate = item.CreateDate,
    //                             ModifyDate = item.ModifyDate,
    //                             GrossWeight = item.GrossWeight,
    //                             Volume = item.Volume,
    //                             Conditions = item.Conditions,
    //                             Status = item.Status,
    //                             Product = product,
    //                             StorageID = item.StorageID,
    //                             ItemsTerm = new Models.OrderItemsTerm
    //                             {
    //                                 ID = terms.ID,
    //                                 OriginRate = terms.OriginRate,
    //                                 FVARate = terms.FVARate,
    //                                 Ccc = terms.Ccc,
    //                                 Embargo = terms.Embargo,
    //                                 HkControl = terms.HkControl,
    //                                 Coo = terms.Coo,
    //                                 CIQ = terms.CIQ,
    //                                 CIQprice = terms.CIQprice,
    //                                 IsHighPrice = terms.IsHighPrice,
    //                                 IsDisinfected = terms.IsDisinfected,
    //                             },
    //                         };
    //        var data = new OrderExtends
    //        {
    //            ID = order.ID,
    //            Type = order.Type,
    //            ClientID = order.ClientID,
    //            InvoiceID = order.InvoiceID,
    //            PayeeID = order.PayeeID,
    //            BeneficiaryID = order.BeneficiaryID,
    //            MainStatus = order.MainStatus,
    //            PaymentStatus = order.PaymentStatus,
    //            InvoiceStatus = order.InvoiceStatus,
    //            Summary = order.Summary,
    //            CreateDate = order.CreateDate,
    //            ModifyDate = order.ModifyDate,
    //            CreatorID = order.CreatorID,
    //            SupplierID = order.SupplierID,
    //            OrderItems = Orderitems.ToArray(),
    //            OutWaybill = outwaybill,
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
    //        };
    //        //转报关没有Inwaybill数据
    //        if (order.Type == OrderType.Declare)
    //        {
    //            data.InWaybill = new WayBillAlls(this.Reponsitory)[order.Input.WayBillID];
    //        }
    //        return data;
    //    }


    //    ///// <summary>
    //    ///// 客户确认
    //    ///// </summary>
    //    ///// <param name="ID"></param>
    //    ///// <returns></returns>
    //    //public void ClientConfirm(string ID)
    //    //{
    //    //    var order = this.GetEntryNoticeDataByOrderID(ID);
    //    //    if (order == null)
    //    //    {
    //    //        throw new Exception("订单不存在！");
    //    //    }
    //    //    //订单项状态修改
    //    //    using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
    //    //    {
    //    //        // if (order.ConfirmStatus == OrderConfirmStatus.ModifyUnConfirm)  //TODO:修改待确认
    //    //        if (true)
    //    //        {
    //    //            reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
    //    //            {
    //    //                Status = (int)Enums.OrderItemStatus.Normal,
    //    //            }, item => item.OrderID == order.ID && item.Status == (int)Enums.OrderItemStatus.ConfirmUpdate);
    //    //            reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
    //    //            {
    //    //                Status = (int)Enums.OrderItemStatus.Deleted,
    //    //            }, item => item.OrderID == order.ID && item.Status == (int)Enums.OrderItemStatus.ConfirmDelete);
    //    //        }
    //    //    }
    //    //    //确认状态更新
    //    //    using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
    //    //    {
    //    //        //原有状态置为失效
    //    //        reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
    //    //        {
    //    //            IsCurrent = false,
    //    //        }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.ConfirmStatus);
    //    //        reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
    //    //        {
    //    //            ID = Guid.NewGuid().ToString(),
    //    //            MainID = order.ID,
    //    //            Type = (int)OrderStatusType.ConfirmStatus,
    //    //            Status = (int)OrderConfirmStatus.Confirmed,
    //    //            CreateDate = DateTime.Now,
    //    //            CreatorID = order.CreatorID,
    //    //            IsCurrent = true,
    //    //        });
    //    //    }

    //    //    #region 生成库房入库通知
    //    //    string message;
    //    //    if (order.Type == OrderType.Declare)
    //    //    {
    //    //        message = Extends.NoticeExtends.DecEntryNotice(order);
    //    //        //message = Extends.NoticeExtends.CgDecEntryNotice(order);
    //    //    }
    //    //    else
    //    //    {
    //    //        message = Extends.NoticeExtends.DecTranferEntryNotice(order);
    //    //    }
    //    //    //调用结果日志
    //    //    Logger.log(this.User.ID, new OperatingLog
    //    //    {
    //    //        MainID = order.ID,
    //    //        Operation = message,
    //    //        Summary = "报关订单库房通知对接结果日志！",
    //    //    });
    //    //    #endregion

    //    //    #region 芯达通接口同步数据
    //    //    Task.Run(() =>
    //    //    {
    //    //        //调用芯达通接口同步数据
    //    //        //  var IsModify = order.ConfirmStatus == OrderConfirmStatus.ModifyUnConfirm; //TODO: 修改待确认
    //    //        var IsModify = false;
    //    //        var orderconfirm = new XDTModels.OrderConfirmed()
    //    //        {
    //    //            OrderID = order.ID,
    //    //            UserID = this.User.ID,
    //    //            IsCancel = false,
    //    //            IsHangUpConfirm = IsModify,
    //    //            CancelReason = string.Empty,
    //    //        };

    //    //        message = Extends.NoticeExtends.XDTOrderConfirm(orderconfirm);
    //    //        //调用结果日志
    //    //        Logger.log(this.User.ID, new OperatingLog
    //    //        {
    //    //            MainID = order.ID,
    //    //            Operation = message,
    //    //            Summary = "芯达通订单确认对接结果日志！",
    //    //        });
    //    //    });
    //    //    #endregion
    //    //}

    //    /// <summary>
    //    /// 客户取消
    //    /// </summary>
    //    /// <param name="ID">主键</param>
    //    /// <param name="reason">原因</param>
    //    public void ClientCancel(string ID, string reason)
    //    {
    //        var order = this[ID];
    //        if (order == null)
    //        {
    //            throw new Exception("订单不存在！");
    //        }
    //        //状态更新
    //        using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
    //        {
    //            //修改主状态
    //            reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
    //            {
    //                IsCurrent = false,
    //            }, item => item.MainID == order.ID && item.Type == (int)OrderStatusType.MainStatus);
    //            reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
    //            {
    //                ID = Guid.NewGuid().ToString(),
    //                MainID = order.ID,
    //                Type = (int)OrderStatusType.MainStatus,
    //                Status = (int)CgOrderStatus.取消,
    //                CreateDate = DateTime.Now,
    //                CreatorID = order.CreatorID,
    //                IsCurrent = true,
    //            });

    //            //修改运单状态为关闭
    //            reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
    //            {
    //                Status = (int)GeneralStatus.Closed,
    //                ModifyDate = DateTime.Now,
    //            }, item => item.ID == order.Input.WayBillID);
    //        }

    //        #region 芯达通接口同步数据
    //        Task.Run(() =>
    //        {
    //            //调用芯达通接口同步数据
    //            //    var IsModify = order.ConfirmStatus == OrderConfirmStatus.ModifyUnConfirm;  //TODO:修改待确认
    //            var IsModify = false;
    //            var orderconfirm = new XDTModels.OrderConfirmed()
    //            {
    //                OrderID = order.ID,
    //                UserID = this.User.ID,
    //                IsCancel = true,
    //                CancelReason = reason,
    //            };

    //            var message = Extends.NoticeExtends.XDTOrderConfirm(orderconfirm);
    //            //调用结果日志
    //            Logger.log(this.User.ID, new OperatingLog
    //            {
    //                MainID = order.ID,
    //                Operation = message,
    //                Summary = "芯达通订单取消对接结果日志！",
    //            });
    //        });
    //        #endregion
    //    }
    //}
}
