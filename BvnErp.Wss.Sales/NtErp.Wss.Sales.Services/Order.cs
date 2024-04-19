using Needs.Overall;
using NtErp.Wss.Sales.Services.Extends;
using NtErp.Wss.Sales.Services.Model;
using NtErp.Wss.Sales.Services.Model.Orders;
using NtErp.Wss.Sales.Services.Model.Orders.Hanlders;
using NtErp.Wss.Sales.Services.Models;
using NtErp.Wss.Sales.Services.Models.Orders;
using NtErp.Wss.Sales.Services.Models.Orders.Commodity;
using NtErp.Wss.Sales.Services.Overalls;
using NtErp.Wss.Sales.Services.Overalls.Rates;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Utils.Converters;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services
{
    /// <summary>
    /// 订单
    /// </summary>
    public class Order
    {
        const string session_name = "tj85j94jgj";
        //public const string session_productid = session_name + "_productsid";
        public const string session_cartid = session_name + "_cartid ";

        #region 属性
        [XmlIgnore]
        public string ID { get; set; }
        [XmlIgnore]
        public string UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [XmlIgnore]
        public string SiteUserName { get; set; }
        /// <summary>
        /// 公司名
        /// </summary>
        [XmlIgnore]
        public string CompanyName { get; set; }
        [XmlIgnore]
        public DateTime CreateDate { get; set; }
        [XmlIgnore]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [XmlIgnore]
        public string Summary { get; set; }
        [XmlIgnore]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 承运方式
        /// </summary>
        [XmlIgnore]
        public TransportTerm Transport { get; set; }



        District district;

        /// <summary>
        /// 交货地
        /// </summary>
        [XmlIgnore]
        public District District
        {
            get
            {

                return this.district;
            }
            set
            {
                this.rates = new FixedRates(value);
                this.district = value;
            }
        }

        /// <summary>
        /// 交易币种
        /// </summary>
        [XmlIgnore]
        public Currency Currency { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        /// <example>
        /// 特有的管理
        /// </example>
        public Source Source { get; set; }
        Receipts receipts;

        /// <summary>
        /// 收据
        /// </summary>
        public Receipts Receipts
        {
            get
            {
                if (this.receipts == null)
                {
                    this.receipts = new Receipts();
                }

                foreach (var item in receipts)
                {
                    Tutopo.From(this, item);
                }

                return this.receipts;
            }
            set
            {
                this.receipts = value;
            }
        }

        FixedRates rates;

        /// <summary>
        /// 固定汇率
        /// </summary>
        public FixedRates Rates
        {
            get
            {
                if (this.rates == null)
                {
                    this.rates = new FixedRates(this.District);
                }
                return this.rates;
            }
            set
            {
                this.rates = value;
            }
        }

        Premiums premiums;
        /// <summary>
        /// 附加价值体系
        /// </summary>
        public Premiums Premiums
        {
            get
            {
                if ((this.Status == OrderStatus.Placed || this.Status == OrderStatus.Creating) && this.premiums == null)
                {
                    var view = UnifyFees.Current.Order.Where(this.Source, this.District, this.Currency).Select(item => new Premium
                    {
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = 1,
                        Currency = item.Currency,
                        Summary = item.Summary
                    });

                    this.premiums = new Premiums(view);
                }
                return this.premiums;
            }
            set
            {
                this.premiums = value;
            }
        }

        InvoiceBase invoice;
        /// <summary>
        /// 开票
        /// </summary>
        /// <example>
        /// 到底是否容许修改没有想通，主要原因是，没有想通当要修改的时候要有那些人员工作协调的问题。（这其中可能还有公司管理上的要求）
        /// </example>
        public InvoiceBase Invoice
        {
            get
            {
                if (invoice == null)
                {
                    return null;
                }

                Tutopo.From(this, this.invoice);
                return this.invoice;
            }
            set
            {
                this.invoice = value;
            }
        }

        /// <summary>
        /// 是否需要发票
        /// </summary>
        public bool IsNeedInvoice { get; set; }

        /// <summary>
        /// 平台的执行公司
        /// </summary>
        public Distributor Beneficiary { get; set; }


        ///// <summary>
        ///// 换货集合
        ///// </summary>
        //[XmlIgnore]
        //public ReplaceApplys Replaces
        //{
        //    get
        //    {
        //        // return new ReplaceApplys(this.UserID, this.ID);
        //        return new ReplaceApplys();
        //    }
        //}

        ///// <summary>
        ///// 退货集合
        ///// </summary>
        //[XmlIgnore]
        //public ReturnApplys Returns
        //{
        //    get
        //    {
        //        //return new ReturnApplys(this.UserID, this.ID);
        //        return new ReturnApplys();
        //    }
        //}
        #endregion

        #region Alters

        ServiceDetails details;
        /// <summary>
        /// 订单详情
        /// </summary>
        public ServiceDetails Details
        {
            get
            {
                if (this.details == null)
                {
                    this.details = new ServiceDetails();
                }

                foreach (var item in this.details)
                {
                    Tutopo.From(this, item);
                }
                return this.details;
            }
            set
            {
                if (details == null || details.Count == 0)
                {
                    details = value;
                    return;
                }

                throw new NotSupportedException("Do not support multiple assignment!");
            }
        }

        /// <summary>
        /// 提货人
        /// </summary>
        Consignees consignee;
        public Consignees Consignee
        {
            get
            {
                if (this.consignee == null)
                {
                    this.consignee = new Consignees();
                }
                return consignee;
            }
            set
            {
                this.consignee = value;
            }
        }
        /// <summary>
        /// 账单地址
        /// </summary>
        public Consignee BillConsignee { get; set; }
        #endregion

        #region 一些开发显示订单需要的统计


        /// <summary>
        /// 订单总价
        /// </summary>
        public decimal Total
        {
            get
            {
                return this.Premiums.Total + this.Details.Total;
            }
        }

        /// <summary>
        /// 已支付金额
        /// </summary>
        public decimal Paid
        {
            get
            {
                return new UserOutputsView(this.UserID).Where(item => item.OrderID == this.ID).Sum(item => item.Amount as decimal?).GetValueOrDefault().Fourh();
            }
        }

        /// <summary>
        /// 剩余未支付金额
        /// </summary>
        public decimal Unpaid
        {
            get
            {
                return this.Total - this.Paid;
            }
        }

        /// <summary>
        /// 发货比例
        /// </summary>
        public float DeliveryRatio { get; set; }
        /// <summary>
        /// 支付比例
        /// </summary>
        public float PaidRatio { get; set; }
        /// <summary>
        /// 是否已开始发货
        /// </summary>
        public bool IsSend { get; set; }
        #endregion

        public Order()
        {
            //this.Consignee = new Consignees();
            this.Status = OrderStatus.Creating;
            //this.Premiums = new Premiums();
            this.IsNeedInvoice = true;
            this.CreateDate = this.UpdateDate = DateTime.Now;

            this.CloseSuccess += Order_CloseSuccess;
            this.ChangeSuccess += Order_ChangeSuccess;
        }

        internal Order(System.Xml.Linq.XElement xml)
        {
            var entity = xml.XmlEleTo<Order>();

            this.District = entity.District;
            this.Currency = entity.Currency;
            this.Consignee = entity.Consignee;
            this.Details = entity.Details;
            this.Beneficiary = entity.Beneficiary;
            this.Source = entity.Source;
            this.Invoice = entity.Invoice;
            this.Premiums = entity.Premiums;
            this.Rates = entity.Rates;
            this.Receipts = entity.Receipts;
            this.IsNeedInvoice = entity.IsNeedInvoice;
            this.BillConsignee = entity.BillConsignee;
            this.IsSend = entity.IsSend;

            this.CloseSuccess += Order_CloseSuccess;
            this.ChangeSuccess += Order_ChangeSuccess;
        }

        #region 持久化
        internal void Enter()
        {
            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                this.UpdateDate = DateTime.Now;
                this.PaidRatio = ((float)this.Paid / (float)this.Total) * 100;

                var t1 = this.Details.Select(t => t.Commodity.Receivable).Sum();
                var t2 = this.Details.Select(t => t.Commodity.Sent).Sum();
                this.DeliveryRatio = ((float)t2 / t1) * 100;

                repository.Update(this.ToLinq(), item => item.ID == this.ID);

                repository.Delete<Layer.Data.Sqls.BvOrders.OrderShowers>(item => item.MainID == this.ID);
                repository.Insert(new Layer.Data.Sqls.BvOrders.OrderShowers
                {
                    MainID = this.ID,
                    Xml = this.XmlEle()
                });

            }
        }

        /// <summary>
        /// 添加产品项
        /// </summary>
        /// <param name="detail"></param>
        public void AddDetail(ServiceDetail detail)
        {
            this.Details.Add(detail);
            this.Enter();

            // 商品帐
            new CommodityInput
            {
                OrderID = this.ID,
                UserID = this.UserID,
                ServiceOuputID = detail.ServiceOutputID,
                Count = detail.Quantity
            }.Enter();
        }

        /// <summary>
        /// 订单完成
        /// </summary>
        public void Completed()
        {
            // 这个应该是被调用，是出库完成或客户签收之后的动作？

            //复杂的验证
            /*
            首先要验证出库是否完成 数量
            如果出库都没有完成直接抛异常，失败的事件对前端进行提示
            出库完成了，超过一定时间自动完成
            */

            this.Status = OrderStatus.Completed;
            this.UpdateDate = DateTime.Now;

            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                repository.Update<Layer.Data.Sqls.BvOrders.Orders>(new
                {
                    Status = this.Status,
                    UpdateDate = this.UpdateDate
                }, item => item.ID == this.ID);
            }

        }

        //其他的方法 完成退钱

        //内部事件 完成 退钱 与其他流程的操作


        /// <summary>
        /// 订单更改（数量价格）
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="price">价格</param>
        public void Change(ServiceDetail detail, int count, decimal price)
        {
            if (this.Status == OrderStatus.Closed || this.Status == OrderStatus.Completed)
            {
                throw new Exception("Current status is not allowed to be modified");
            }

            foreach (var item in this.Details.Where(t => t.ServiceOutputID == detail.ServiceOutputID && t.Status == Underly.Collections.AlterStatus.Normal))
            {
                item.Summary = detail.Summary;
                item.AlterDate = DateTime.Now;

            }

            int oldcount = detail.Quantity;
            decimal oldprice = detail.Price;
            detail.Quantity = count;
            detail.Price = price;
            detail.AlterDate = DateTime.Now;

            this.Details.Add(detail);

            if (oldcount * oldprice != count * price) // 总价未变不进行钱款操作
            {
                decimal paid = this.Receipts.Sum(item => item.Amount);
                if (paid > 0 && paid > this.Total)
                {
                    #region 3.支付大于应付款，且大于应付款的金额大于等于移除项金额，退该项的全部金额；
                    if (paid - this.Total >= detail.SubTotal)
                    {
                        // 优先信用
                        var credit = this.Receipts.Where(t => t.PaymentMethod == PaymentMethod.Credit).Sum(t => t.Amount as decimal?).GetValueOrDefault(0);

                        if (credit >= detail.SubTotal)
                        {
                            #region 1.信用金额足够退
                            decimal amount = -detail.SubTotal;
                            // 添加票据
                            this.Receipts.Add(new Receipt
                            {
                                Currency = this.Currency,
                                Amount = amount,
                                PaymentMethod = PaymentMethod.Credit,
                                Drawee = "",
                                Summary = "",
                            });
                            // 记录用户的out帐
                            new UserOutput
                            {
                                Type = PaymentMethod.Credit.ToAccountType(),
                                Amount = amount,
                                Currency = this.Currency,
                                OrderID = this.ID,
                                UserID = this.UserID,
                            }.Balanced();
                            #endregion
                        }
                        else
                        {
                            #region 2.信用金额不足，先退全部信用，再退现金
                            // 添加票据
                            this.Receipts.Add(new Receipt
                            {
                                Currency = this.Currency,
                                Amount = -credit,
                                PaymentMethod = PaymentMethod.Credit,
                                Drawee = "",
                                Summary = "",
                            });
                            // 记录用户的out帐
                            new UserOutput
                            {
                                Type = PaymentMethod.Credit.ToAccountType(),
                                Amount = -credit,
                                Currency = this.Currency,
                                OrderID = this.ID,
                                UserID = this.UserID,
                            }.Balanced();

                            #endregion
                            #region 3.信用金额为0，直接退现金
                            decimal money = detail.SubTotal - credit;
                            // 添加票据
                            this.Receipts.Add(new Receipt
                            {
                                Currency = this.Currency,
                                Amount = -money,
                                PaymentMethod = PaymentMethod.Credit,
                                Drawee = "",
                                Summary = "",
                            });
                            // 记录用户的out帐
                            new UserOutput
                            {
                                Type = PaymentMethod.Credit.ToAccountType(),
                                Amount = -money,
                                Currency = this.Currency,
                                OrderID = this.ID,
                                UserID = this.UserID,
                            }.Balanced();
                            #endregion

                        }

                    }
                    #endregion
                    #region  4.支付大于应付款，且大于应付款的金额小于移除项金额，退大于应付款的金额；
                    else
                    {
                        var pay = paid - detail.SubTotal;// 应退金额
                                                         // 优先信用
                        var credit = this.Receipts.Where(t => t.PaymentMethod == PaymentMethod.Credit).Sum(t => t.Amount as decimal?).GetValueOrDefault(0);

                        if (credit >= pay)
                        {
                            #region 1.信用金额足够退
                            decimal amount = -pay;
                            // 添加票据
                            this.Receipts.Add(new Receipt
                            {
                                Currency = this.Currency,
                                Amount = amount,
                                PaymentMethod = PaymentMethod.Credit,
                                Drawee = "",
                                Summary = "",
                            });
                            // 记录用户的out帐
                            new UserOutput
                            {
                                Type = PaymentMethod.Credit.ToAccountType(),
                                Amount = amount,
                                Currency = this.Currency,
                                OrderID = this.ID,
                                UserID = this.UserID,
                            }.Balanced();
                            #endregion
                        }
                        else
                        {
                            #region 2.信用金额不足，先退全部信用，再退现金
                            // 添加票据
                            this.Receipts.Add(new Receipt
                            {
                                Currency = this.Currency,
                                Amount = -credit,
                                PaymentMethod = PaymentMethod.Credit,
                                Drawee = "",
                                Summary = "",
                            });
                            // 记录用户的out帐
                            new UserOutput
                            {
                                Type = PaymentMethod.Credit.ToAccountType(),
                                Amount = -credit,
                                Currency = this.Currency,
                                OrderID = this.ID,
                                UserID = this.UserID,
                            }.Balanced();

                            #endregion
                            #region 3.信用金额为0，直接退现金
                            decimal money = pay - credit;
                            // 添加票据
                            this.Receipts.Add(new Receipt
                            {
                                Currency = this.Currency,
                                Amount = -money,
                                PaymentMethod = PaymentMethod.Credit,
                                Drawee = "",
                                Summary = "",
                            });
                            // 记录用户的out帐
                            new UserOutput
                            {
                                Type = PaymentMethod.Credit.ToAccountType(),
                                Amount = -money,
                                Currency = this.Currency,
                                OrderID = this.ID,
                                UserID = this.UserID,
                            }.Balanced();
                            #endregion

                        }
                    }
                    #endregion
                }

            }

            this.Enter();
            //// 商品帐
            //var inputs = new CommodityInputsView().Where(t => t.ServiceOuputID == detail.ServiceOutputID);
            //var count1 = inputs.Sum(t => t.Count);
            //var count2 = new CommodityOutputsView().Where(t => inputs.Select(item => item.ID).Contains(t.InputID)).Sum(t => t.Count);
            //if (count1 - count2 > 0)
            //{
            //    new CommodityInput
            //    {
            //        OrderID = this.ID,
            //        UserID = this.UserID,
            //        ServiceOuputID = detail.ServiceOutputID,
            //        Count = -(count1 - count2)
            //    }.Enter();

            //}
            //new CommodityInput
            //{
            //    OrderID = this.ID,
            //    UserID = this.UserID,
            //    ServiceOuputID = detail.ServiceOutputID,
            //    Count = count
            //}.Enter();

        }

        /// <summary>
        /// 订单关闭
        /// </summary>
        public void Close()
        {

            // 1.判断是否已支付，已支付，则按支付方式退回已支付金额。

            // 2.查找和本订单有关的询报价，进行关闭操作。

            // 3.流程更改为关闭状态。


            //数据与状态 还有流程
            //要考虑退钱等

            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                this.Status = OrderStatus.Closed;
                repository.Update<Layer.Data.Sqls.BvOrders.Orders>(new
                {
                    Status = this.Status,
                    UpdateDate = DateTime.Now
                }, item => item.ID == this.ID);


            }
            if (this != null && this.CloseSuccess != null)
            {
                this.CloseSuccess(this, new CloseEventArgs());
            }

        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        public void Pay(PaymentMethod type, decimal amount)
        {
            if (amount == 0)
            {
                throw new Exception("The amount of payment must be more than 0");
            }
            // 添加票据
            this.Receipts.Add(new Receipt
            {
                Amount = amount,
                PaymentMethod = type,
                Drawee = "",
                Summary = "",
            });

            // 记录用户的out帐
            new UserOutput
            {
                Type = type.ToAccountType(),
                Amount = amount,
                Currency = this.Currency,
                OrderID = this.ID,
                UserID = this.UserID,
            }.Balanced();

            this.Enter();
        }
        /// <summary>
        /// 删除产品项
        /// </summary>
        /// <param name="detail"></param>
        public void RemoveItem(ServiceDetail detail)
        {
            if (this.Status == OrderStatus.Closed || this.Status == OrderStatus.Completed)
            {
                throw new Exception("Current status is not allowed to be modified");
            }

            this.Details.Single(t => t.ServiceOutputID == detail.ServiceOutputID && t.Status == Underly.Collections.AlterStatus.Normal).AdminID = detail.AdminID;
            this.Details.Remove(t => t.ServiceOutputID == detail.ServiceOutputID && t.Status == Underly.Collections.AlterStatus.Normal);
            // 算账 应写成事件
            /* 
                1.没有支付过，不考虑；
                2.支付小于等于应付款，不考虑；
                3.支付大于应付款，且大于应付款的金额大于等于移除项金额，退该项的全部金额；
                4.支付大于应付款，且大于应付款的金额小于移除项金额，退大于应付款的金额；
            */
            decimal paid = this.Receipts.Sum(item => item.Amount);
            if (paid > 0 && paid > this.Total)
            {
                #region 3.支付大于应付款，且大于应付款的金额大于等于移除项金额，退该项的全部金额；
                if (paid - this.Total >= detail.SubTotal)
                {
                    // 优先信用
                    var credit = this.Receipts.Where(t => t.PaymentMethod == PaymentMethod.Credit).Sum(t => t.Amount as decimal?).GetValueOrDefault(0);

                    if (credit >= detail.SubTotal)
                    {
                        #region 1.信用金额足够退
                        decimal amount = -detail.SubTotal;
                        // 添加票据
                        this.Receipts.Add(new Receipt
                        {
                            Currency = this.Currency,
                            Amount = amount,
                            PaymentMethod = PaymentMethod.Credit,
                            Drawee = "",
                            Summary = "",
                        });
                        // 记录用户的out帐
                        new UserOutput
                        {
                            Type = PaymentMethod.Credit.ToAccountType(),
                            Amount = amount,
                            Currency = this.Currency,
                            OrderID = this.ID,
                            UserID = this.UserID,
                        }.Balanced();
                        #endregion
                    }
                    else
                    {
                        #region 2.信用金额不足，先退全部信用，再退现金
                        // 添加票据
                        this.Receipts.Add(new Receipt
                        {
                            Currency = this.Currency,
                            Amount = -credit,
                            PaymentMethod = PaymentMethod.Credit,
                            Drawee = "",
                            Summary = "",
                        });
                        // 记录用户的out帐
                        new UserOutput
                        {
                            Type = PaymentMethod.Credit.ToAccountType(),
                            Amount = -credit,
                            Currency = this.Currency,
                            OrderID = this.ID,
                            UserID = this.UserID,
                        }.Balanced();

                        #endregion
                        #region 3.信用金额为0，直接退现金
                        decimal money = detail.SubTotal - credit;
                        // 添加票据
                        this.Receipts.Add(new Receipt
                        {
                            Currency = this.Currency,
                            Amount = -money,
                            PaymentMethod = PaymentMethod.Credit,
                            Drawee = "",
                            Summary = "",
                        });
                        // 记录用户的out帐
                        new UserOutput
                        {
                            Type = PaymentMethod.Credit.ToAccountType(),
                            Amount = -money,
                            Currency = this.Currency,
                            OrderID = this.ID,
                            UserID = this.UserID,
                        }.Balanced();
                        #endregion

                    }

                }
                #endregion
                #region  4.支付大于应付款，且大于应付款的金额小于移除项金额，退大于应付款的金额；
                else
                {
                    var pay = paid - detail.SubTotal;// 应退金额
                    // 优先信用
                    var credit = this.Receipts.Where(t => t.PaymentMethod == PaymentMethod.Credit).Sum(t => t.Amount as decimal?).GetValueOrDefault(0);

                    if (credit >= pay)
                    {
                        #region 1.信用金额足够退
                        decimal amount = -pay;
                        // 添加票据
                        this.Receipts.Add(new Receipt
                        {
                            Currency = this.Currency,
                            Amount = amount,
                            PaymentMethod = PaymentMethod.Credit,
                            Drawee = "",
                            Summary = "",
                        });
                        // 记录用户的out帐
                        new UserOutput
                        {
                            Type = PaymentMethod.Credit.ToAccountType(),
                            Amount = amount,
                            Currency = this.Currency,
                            OrderID = this.ID,
                            UserID = this.UserID,
                        }.Balanced();
                        #endregion
                    }
                    else
                    {
                        #region 2.信用金额不足，先退全部信用，再退现金
                        // 添加票据
                        this.Receipts.Add(new Receipt
                        {
                            Currency = this.Currency,
                            Amount = -credit,
                            PaymentMethod = PaymentMethod.Credit,
                            Drawee = "",
                            Summary = "",
                        });
                        // 记录用户的out帐
                        new UserOutput
                        {
                            Type = PaymentMethod.Credit.ToAccountType(),
                            Amount = -credit,
                            Currency = this.Currency,
                            OrderID = this.ID,
                            UserID = this.UserID,
                        }.Balanced();

                        #endregion
                        #region 3.信用金额为0，直接退现金
                        decimal money = pay - credit;
                        // 添加票据
                        this.Receipts.Add(new Receipt
                        {
                            Currency = this.Currency,
                            Amount = -money,
                            PaymentMethod = PaymentMethod.Credit,
                            Drawee = "",
                            Summary = "",
                        });
                        // 记录用户的out帐
                        new UserOutput
                        {
                            Type = PaymentMethod.Credit.ToAccountType(),
                            Amount = -money,
                            Currency = this.Currency,
                            OrderID = this.ID,
                            UserID = this.UserID,
                        }.Balanced();
                        #endregion

                    }
                }
                #endregion
            }
            // 入库
            this.Enter();

            // 商品帐
            new CommodityInput
            {
                OrderID = this.ID,
                UserID = this.UserID,
                ServiceOuputID = detail.ServiceOutputID,
                Count = -detail.Quantity
            }.Enter();

        }
        /// <summary>
        /// 添加附加价值
        /// </summary>
        /// <param name="model"></param>
        public void AddPremium(Premium model)
        {
            if (this.Status == OrderStatus.Closed || this.Status == OrderStatus.Completed)
            {
                throw new Exception("Current status is not allowed to be modified");
            }
            this.Premiums.Add(model);
            this.Enter();
        }
        #endregion

        #region 事件
        public event CloseSuccessHanlder CloseSuccess;
        public event ChangeSuccessHanlder ChangeSuccess;
        public event PlacedSuccessHanlder PlacedSuccess;

        /// <summary>
        /// 验证库存数量，是否出库
        /// </summary>
        /// <param name="bibSign"></param>
        /// <returns></returns>
        int GetQuantityAvailable(string bibSign)
        {
            return 0;
        }



        bool isChanged;

        /// <summary>
        /// 内部订单更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_ChangeSuccess(object sender, ChangeEventArgs e)
        {
            this.isChanged = true;


            //throw new NotImplementedException();
        }

        /// <summary>
        ///  退款
        /// </summary>
        void Refund(decimal amount)
        {
            if (amount == 0)
            {
                return;
            }
            // 


            #region 真实退款流程操作
            #endregion

        }

        /// <summary>
        /// 内部订单关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_CloseSuccess(object sender, CloseEventArgs e)
        {
            var current = sender as Order;

            // 判断是否已支付
            decimal paid = current.Receipts.Sum(item => item.Amount);
            if (paid > 0)
            {
                // 是否第一次退款
                bool IsFirstRefund = current.Receipts.Count(item => item.Amount < 0) == 0;
                // 添加票据
                var returns = current.Receipts.GroupBy(item => item.PaymentMethod).Select(item => new Receipt
                {
                    PaymentMethod = item.Key,
                    Amount = 0 - item.Sum(t => t.Amount),
                    Code = "",
                    Drawee = "",
                    Summary = "close"
                }).Where(item => item.Amount > 0);
                foreach (var item in returns)
                {
                    current.Receipts.Add(item);
                }

                // 用户账
                var outputs = new UserOutputsView(current.UserID).Where(item => item.OrderID == current.ID);

                #region 未退过款的订单关闭处理
                if (IsFirstRefund)
                {

                    foreach (var item in outputs)
                    {
                        // 平销账目
                        new UserOutput
                        {
                            UserID = current.UserID,
                            Currency = item.Currency,
                            Type = item.Type,
                            Amount = 0 - item.Amount,
                            OrderID = current.ID,
                            UserInputID = item.UserInputID
                        }.Enter();
                    }
                }
                #endregion

                #region 退过款的订单关闭处理
                else
                {
                    var groups = outputs.GroupBy(item => item.UserInputID).Select(item => new UserOutput
                    {
                        OrderID = current.ID,
                        UserID = current.UserID,
                        Amount = 0 - item.Sum(t => t.Amount),
                        Currency = current.Currency,
                        UserInputID = item.Key,
                        Type = item.First().Type
                    }).Where(item => item.Amount > 0);

                    foreach (var output in groups)
                    {
                        output.Enter();
                    }
                }
                #endregion

                current.Enter(); // receipts
            }
            // 退款
            // 记账
            // 锁定库存数量恢复
            //throw new NotImplementedException();
        }

        #endregion 
        void Fire(EventArgs e)
        {
            if (this == null)
            {
                return;
            }

            if (e is ChangeEventArgs && this.ChangeSuccess != null)
            {
                this.ChangeSuccess(this, e as ChangeEventArgs);
            }
        }
    }
}
