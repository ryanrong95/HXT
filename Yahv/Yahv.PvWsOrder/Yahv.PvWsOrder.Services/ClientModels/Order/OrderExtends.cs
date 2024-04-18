using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    /// <summary>
    /// 订单扩展类
    /// </summary>
    public class OrderExtends : Order
    {
        //是否修改
        protected bool IsModify;

        //protected internal OrderExtends()
        public OrderExtends()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.MainStatus = CgOrderStatus.暂存;
            this.InvoiceStatus = OrderInvoiceStatus.UnInvoiced;
            this.PaymentStatus = OrderPaymentStatus.Waiting;
        }

        #region 扩展属性
        /// <summary>
        /// 入库运单对象
        /// </summary>
        public Waybill InWaybill { get; set; }

        /// <summary>
        /// 出库运单对象
        /// </summary>
        public Waybill OutWaybill { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName
        {
            get
            {
                using (var view = new MySuppliers(this.ClientID))
                {
                    var supplier = view[SupplierID];
                    return supplier?.EnglishName;
                }
            }
        }

        /// <summary>
        /// 供应商名称2
        /// </summary>
        public string SupplierName2 { get; set; }

        /// <summary>
        /// 订单附件
        /// </summary>
        public CenterFileDescription[] OrderFiles { get; set; }

        /// <summary>
        /// 代收货款
        /// </summary>
        public Application Receive { get; set; }

        /// <summary>
        /// 发票
        /// </summary>
        public Invoice Invoice { get; set; }

        /// <summary>
        /// 获取特殊要求
        /// </summary>
        public OrderRequirement[] Requirements { get; set; }

        /// <summary>
        /// 通知的信息
        /// </summary>
        public JMessage NoticeMessage { get; private set; }

        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 报关对接用，是否编辑
        /// </summary>
        public bool IsReturned { get; set; }

        /// <summary>
        /// 芯达通客户名称
        /// </summary>
        public string XDTClientName { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string[] PayExchangeSuppliers { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalPrice
        {
            get
            {
                using (OrderItemAlls items = new OrderItemAlls())
                {
                    return items.SearchByOrderID(this.ID).ToArray().Sum(item => item.TotalPrice);
                }
            }
        }

        /// <summary>
        /// 已申请金额
        /// </summary>
        public decimal ApplicationApplyPrice { get; set; }

        #region 报关金额
        /// <summary>
        /// 关税
        /// </summary>
        public decimal TotalTraiff { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal TotalAddTax { get; set; }

        /// <summary>
        /// 代理费
        /// </summary>
        public decimal TotalAgencyFee { get; set; }

        /// <summary>
        /// 杂费
        /// </summary>
        public decimal TotalInspectionFee { get; set; }

        #endregion

        #endregion


        #region 订单保存成功事件注册
        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        #endregion


        #region 数据库表持久化
        /// <summary>
        /// 订单主表插入
        /// </summary>
        /// <param name="reponsitory"></param>
        protected void OrderEnter(PvWsOrderReponsitory reponsitory)
        {
            var order = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>().FirstOrDefault(item => item.ID == this.ID);
            if (order == null)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Orders
                {
                    ID = this.ID,
                    CreatorID = this.CreatorID,
                    Type = (int)this.Type,
                    ClientID = this.ClientID,
                    InvoiceID = this.InvoiceID,
                    PayeeID = this.PayeeID,
                    BeneficiaryID = this.BeneficiaryID,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Summary = this.Summary,
                    SupplierID = this.SupplierID,
                    SettlementCurrency = (int?)this.SettlementCurrency,
                    TotalPrice = this.OrderItems.Sum(item=>item.TotalPrice),
                });
            }
            else
            {
                this.IsModify = true;
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                {
                    CreatorID,
                    Type = (int)this.Type,
                    this.ClientID,
                    this.InvoiceID,
                    this.PayeeID,
                    this.BeneficiaryID,
                    ModifyDate = DateTime.Now,
                    this.Summary,
                    this.SupplierID,
                    SettlementCurrency = (int?)this.SettlementCurrency,
                    TotalPrice = this.OrderItems.Sum(item => item.TotalPrice),
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 订单收货拓展表插入
        /// </summary>
        /// <param name="reponsitory"></param>
        protected void OrderInputEnter(PvWsOrderReponsitory reponsitory)
        {
            if (this.Input == null)
            {
                return;
            }
            int count = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderInputs>().Count(item => item.ID == this.ID);
            if (count == 0)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderInputs
                {
                    ID = this.ID,
                    IsPayCharge = this.Input.IsPayCharge,
                    WayBillID = this.InWaybill.ID,
                    BeneficiaryID = this.Input.BeneficiaryID,
                    Currency = (int?)this.Input.Currency,
                    Conditions = this.Input.Conditions,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderInputs>(new
                {
                    this.Input.IsPayCharge,
                    WayBillID = this.InWaybill.ID,
                    this.Input.BeneficiaryID,
                    Currency = (int?)this.Input?.Currency,
                    this.Input.Conditions,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 订单发货拓展表插入
        /// </summary>
        /// <param name="reponsitory"></param>
        protected void OrderOutputEnter(PvWsOrderReponsitory reponsitory)
        {
            if (this.Output == null)
            {
                return;
            }
            int count = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderOutputs>().Count(item => item.ID == this.ID);
            if (count == 0)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderOutputs
                {
                    ID = this.ID,
                    IsReciveCharge = this.Output.IsReciveCharge,
                    WayBillID = this.OutWaybill.ID,
                    Currency = (int?)this.Output?.Currency,
                    BeneficiaryID = this.Output.BeneficiaryID,
                    Conditions = this.Output.Conditions,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderOutputs>(new
                {
                    this.Output.IsReciveCharge,
                    WayBillID = this.OutWaybill.ID,
                    Currency = (int?)this.Output?.Currency,
                    this.Output.BeneficiaryID,
                    this.Output.Conditions,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 记录状态历史记录
        /// </summary>
        protected void StatusLogEnter()
        {
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                if (!IsModify)
                {
                    #region 主状态
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = this.ID,
                        Type = (int)OrderStatusType.MainStatus,
                        Status = (int)this.MainStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        IsCurrent = true,
                    });
                    #endregion

                    #region 支付状态
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = this.ID,
                        Type = (int)OrderStatusType.PaymentStatus,
                        Status = (int)this.PaymentStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        IsCurrent = true,
                    });
                    #endregion

                    #region 开票状态
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = this.ID,
                        Type = (int)OrderStatusType.InvoiceStatus,
                        Status = (int)this.InvoiceStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        IsCurrent = true,
                    });
                    #endregion

                    #region 付汇状态
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = this.ID,
                        Type = (int)OrderStatusType.RemittanceStatus,
                        Status = (int)this.RemittanceStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        IsCurrent = true,
                    });
                    #endregion

                    //#region 确认状态
                    //reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    //{
                    //    ID = Guid.NewGuid().ToString(),
                    //    MainID = this.ID,
                    //    Type = (int)OrderStatusType.ConfirmStatus,
                    //    Status = (int)this.ConfirmStatus,
                    //    CreateDate = DateTime.Now,
                    //    CreatorID = this.CreatorID,
                    //    IsCurrent = true,
                    //});
                    //#endregion
                }
                else
                {
                    #region 主状态
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.MainID == this.ID && item.Type == (int)OrderStatusType.MainStatus);
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = this.ID,
                        Type = (int)OrderStatusType.MainStatus,
                        Status = (int)this.MainStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        IsCurrent = true,
                    });
                    #endregion
                }
            }
        }

        /// <summary>
        /// 货物特殊要求
        /// </summary>
        protected void OrderRequirementEnter(PvWsOrderReponsitory reponsitory)
        {
            if (this.Requirements == null || !this.Requirements.Any())
            {
                return;
            }

            //先删除当前的,然后插入
            if (this.IsModify)
            {
                reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderRequirements>(item => item.OrderID == this.ID);
            }

            foreach (var a in this.Requirements)
            {
                a.ID = Guid.NewGuid().ToString();
            }
            //数据处理
            var linq = this.Requirements.Select(item => new Layers.Data.Sqls.PvWsOrder.OrderRequirements
            {
                ID = item.ID,
                OrderID = this.ID,
                Type = (int)item.Type,
                Name = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                Requirement = item.Requirement ?? "",
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
            }).ToArray();
            reponsitory.Insert(linq);
        }


        /// <summary>
        /// 订单项修改持久化
        /// </summary>
        /// <param name="updateitems"></param>
        /// <returns></returns>
        protected OrderItem[] UpdateOrderItems(OrderItem[] updateitems)
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                //修改后数据重新插入数据库
                foreach (var item in updateitems)
                {
                    item.ProductID = item.Product.ID ?? string.Concat(item.Product.PartNumber, item.Product.Manufacturer).MD5();
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        OrderID = item.OrderID = this.ID,
                        ProductID = item.Product.ID,
                        CustomName = item.Name,
                        item.Origin,
                        item.DateCode,
                        item.Quantity,
                        Currency = (int)item.Currency,
                        item.UnitPrice,
                        Unit = (int)item.Unit,
                        item.TotalPrice,
                        item.CreateDate,
                        ModifyDate = DateTime.Now,
                        item.GrossWeight,
                        item.Volume,
                        item.Conditions,
                        Status = (int)item.Status,
                        item.IsAuto,
                        item.WayBillID,
                        Type = (int)item.Type,
                    }, c => c.ID == item.ID);
                }
            }
            return updateitems;
        }

        /// <summary>
        /// 订单项新增持久化
        /// </summary>
        /// <returns></returns>
        protected void InsertOrderItems()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                bool isDeclare = this.Type == OrderType.Declare;
                var insertItems = this.OrderItems.Where(item => string.IsNullOrWhiteSpace(item.ID)).ToArray();
                foreach (var item in insertItems)
                {
                    item.ID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem);
                    // 2022-01-19 LK 香港库房重构，报关订单的InputID 改为OrderItemID
                    if (isDeclare)
                    {
                        item.InputID = item.ID;
                    }
                    else
                    {
                        item.InputID = item.InputID ?? Layers.Data.PKeySigner.Pick(PKeyType.Input);
                    }
                    item.OutputID = this.Output == null ? null : Layers.Data.PKeySigner.Pick(PKeyType.Output);
                    item.OrderID = this.ID;
                    item.ProductID = item.Product.ID ?? string.Concat(item.Product.PartNumber, item.Product.Manufacturer).MD5();
                }
                var linq = insertItems.Select(item => new Layers.Data.Sqls.PvWsOrder.OrderItems
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    TinyOrderID = item.OrderID + "-01",
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    ProductID = item.ProductID,
                    CustomName = item.Name,
                    Origin = item.Origin,
                    DateCode = item.DateCode,
                    Quantity = item.Quantity,
                    Currency = (int)item.Currency,
                    UnitPrice = item.UnitPrice,
                    Unit = (int)item.Unit,
                    TotalPrice = item.TotalPrice,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    GrossWeight = item.GrossWeight,
                    Volume = item.Volume,
                    Conditions = item.Conditions,
                    Status = (int)item.Status,
                    IsAuto = item.IsAuto,
                    WayBillID = item.WayBillID,
                    Type = (int)item.Type,
                    StorageID = item.StorageID,
                }).ToArray();
                reponsitory.Insert(linq);
            }
        }

        /// <summary>
        /// 订单项历史记录
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="items"></param>
        protected void OrderItemsLogEnter(PvWsOrderReponsitory reponsitory, OrderItem[] items)
        {
            var linq = items.Select(item => new Layers.Data.Sqls.PvWsOrder.Logs_OrderItems
            {
                ID = Guid.NewGuid().ToString(),
                OrderItemID = item.ID,
                OrderID = item.OrderID,
                TinyOrderID = item.TinyOrderID,
                InputID = item.InputID,
                ProductID = item.ProductID,
                CustomName = item.Name,
                Origin = item.Origin,
                DateCode = item.DateCode,
                Quantity = item.Quantity,
                Currency = (int)item.Currency,
                UnitPrice = item.UnitPrice,
                Unit = (int)item.Unit,
                TotalPrice = item.TotalPrice,
                CreateDate = item.CreateDate,
                ModifyDate = item.ModifyDate,
                GrossWeight = item.GrossWeight,
                Volume = item.Volume,
                Conditions = item.Conditions,
                Status = (int)item.Status,
                IsAuto = item.IsAuto,
                WayBillID = item.WayBillID,
            }).ToArray();
            //将原来的数据插入历史记录表
            reponsitory.Insert(linq);
        }

        /// <summary>
        /// 付汇供应商录入
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="supplierids"></param>
        /// <param name="orderId"></param>
        protected void MapsSupplierEnter(PvWsOrderReponsitory reponsitory, string[] supplierids, string orderId)
        {
            reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.MapsSupplier>(item => item.OrderID == orderId);
            reponsitory.Insert(supplierids.Select(item => new Layers.Data.Sqls.PvWsOrder.MapsSupplier
            {
                OrderID = orderId,
                SupplierID = item,
            }).ToArray());
        }
        #endregion


        #region 辅助方法
        /// <summary>
        /// 当前数据与数据库比较是否发生数据改变
        /// </summary>
        /// <param name="items">页面订单项数据</param>
        /// <param name="reponsitory">数据库实例</param>
        /// <returns></returns>
        protected OrderItem[] OrderItemCompare(OrderItem[] items, PvWsOrderReponsitory reponsitory)
        {
            List<OrderItem> updateitems = new List<OrderItem>();
            foreach (var item in items)
            {
                var orderitem = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(a => a.ID == item.ID);
                var itemjson = new
                {
                    ProductID = string.Concat(item.Product.PartNumber, item.Product.Manufacturer).MD5(),
                    item.Origin,
                    item.DateCode,
                    item.Quantity,
                    item.Currency,
                    item.Unit,
                    item.TotalPrice,
                    item.GrossWeight,
                    item.Volume,
                }.Json();
                var orderitemjson = new
                {
                    orderitem?.ProductID,
                    orderitem?.Origin,
                    orderitem?.DateCode,
                    orderitem?.Quantity,
                    orderitem?.Currency,
                    orderitem?.Unit,
                    orderitem?.TotalPrice,
                    orderitem?.GrossWeight,
                    orderitem?.Volume,
                }.Json();
                //比较页面数据是否与数据库中数据一致,决定是否数据有修改
                if (!itemjson.Equals(orderitemjson))
                {
                    updateitems.Add(item);
                }
            }
            return updateitems.ToArray();
        }

        /// <summary>
        /// 后续任务处理
        /// </summary>
        /// <param name="task"></param>
        protected void OrderContinue(Task task)
        {
            task.ContinueWith(t =>
            {
                var ex = t.Exception;
                throw new Exception(ex?.Message);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        #endregion


        #region 出库通知结果处理
        /// <summary>
        /// 出库结果处理
        /// </summary>
        /// <param name="result"></param>
        protected void HandleOutNotice(string result)
        {
            //调用结果日志
            Logger.log(this.CreatorID, new OperatingLog
            {
                MainID = this.ID,
                Operation = "代发货出库通知的回执信息！",
                Summary = result,
            });

            //获取库房返回的信息
            this.NoticeMessage = result.JsonTo<JMessage>();
            if (NoticeMessage.success)
            {
                //入库成功后,状态改为待发货
                using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.MainID == this.ID && item.Type == (int)OrderStatusType.MainStatus && item.IsCurrent);
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = this.ID,
                        Type = (int)OrderStatusType.MainStatus,
                        Status = (int)CgOrderStatus.待收货,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        IsCurrent = true,
                    });
                }
            }
            if (NoticeMessage.code == 400)
            {
                using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                    {
                        MainStatus = (int)CgOrderStatus.取消,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion


        #region 确认账单 信用支付
        /// <summary>
        /// 确认账单
        /// </summary>
        public void ConfirmBill()
        {
            this.UpdatePaymentStatus(OrderPaymentStatus.ToBePaid);
        }

        /// <summary>
        /// 更改支付状态
        /// </summary>
        /// <param name="paymentStatus"></param>
        private void UpdatePaymentStatus(OrderPaymentStatus paymentStatus)
        {
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false,
                    CreatorID
                }, item => item.MainID == this.ID && item.Type == (int)OrderStatusType.PaymentStatus);

                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = this.ID,
                    Type = (int)OrderStatusType.PaymentStatus,
                    Status = (int)paymentStatus,
                    CreateDate = DateTime.Now,
                    CreatorID = this.CreatorID,
                    IsCurrent = true,
                });
            }
        }
        #endregion


        #region 报关订单提交发给芯达通
        protected void SumbitToXdt()
        {
            //调用芯达通接口传下单数据
            var result = this.XDTOrderNotice();

            //调用结果日志
            Logger.log(this.CreatorID, new OperatingLog
            {
                MainID = this.ID,
                Operation = "芯达通订单数据对接结果日志！",
                Summary = result,
            });
        }
        #endregion
    }
}
