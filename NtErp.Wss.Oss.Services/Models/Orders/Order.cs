using Needs.Linq;
using Needs.Utils.Descriptions;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using Needs.Utils.Converters;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 订单
    /// </summary>
    public class Order : IUnique //, Interfaces.IOrderAction
    {
        public Order()
        {
            this.Status = OrderStatus.Paying;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.EnterSuccess += Order_EnterSuccess;
        }
        /// <summary>
        /// 订单创建成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_EnterSuccess(object sender, SuccessEventArgs e)
        {
            // 清除购物车产品项

            //var current = sender as Order;

            //foreach (var item in current.Items)
            //{
            //     // item.ServiceID
            //}
        }

        #region 属性

        public string ID { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// 客户下单备注
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #endregion

        #region 扩展属性

        decimal total;

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal Total
        {
            get
            {
                if (this.items == null && this.premiums == null)
                {
                    return this.total;
                }

                return this.Items.Total + this.Premiums?.Total ?? 0m;
            }
            internal set { this.total = value; }
        }

        decimal paid;

        public decimal Paid
        {
            get
            {
                if (this.Paids == null)
                {
                    return this.paid;
                }

                return this.Paids.Sum(item => item.Amount).Twoh();
            }
            internal set { this.paid = value; }
        }

        public decimal? SendRate { get; set; }

        /// <summary>
        /// 货币符号
        /// </summary>
        public string Symbol
        {
            get
            {
                return this.Beneficiary.Currency.GetLegal().Symbol;
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        public ClientTop Client { get; set; }
        /// <summary>
        /// 发票
        /// </summary>
        public Invoice Invoice { get; set; }
        /// <summary>
        /// 受益人
        /// </summary>
        public Beneficiary Beneficiary { get; set; }
        /// <summary>
        /// 提货人
        /// </summary>
        public Party Consignee { get; set; }
        /// <summary>
        /// 交货人
        /// </summary>
        public Party Deliverer { get; set; }
        /// <summary>
        /// 运输条款
        /// </summary>
        public TransportTerm TransportTerm { get; set; }

        OrderItems items;
        /// <summary>
        /// 产品项
        /// </summary>
        public OrderItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.OrderItemsAlls())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.Items = new OrderItems(string.IsNullOrWhiteSpace(this.ID) ? new OrderItem[0] : query.ToArray());
                    }
                }

                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new OrderItems(value, delegate (OrderItem item)
                {
                    item.OrderID = this.ID;
                });
            }
        }

        Premiums premiums;
        /// <summary>
        /// 产品项
        /// </summary>
        public Premiums Premiums
        {
            get
            {
                if (premiums == null)
                {
                    using (var view = new Views.PremiumsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        var arry = new Premium[0];
                        if (string.IsNullOrWhiteSpace(this.ID))
                        {
                            arry = new Premium[0];
                        }
                        else
                        {
                            arry = query.ToArray();
                        }

                        this.Premiums = new Premiums(arry);
                    }
                }

                return this.premiums;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.premiums = new Premiums(value, delegate (Premium item)
                {
                    item.OrderID = this.ID;
                });
            }
        }

        AdminUserOutput[] paids;

        /// <summary>
        /// 已经支付
        /// </summary>
        public AdminUserOutput[] Paids
        {
            get
            {
                using (var view = new Views.UserOutputsAdmins(this.ID))
                {
                    return paids = view.ToArray();
                }
            }
        }

        /// <summary>
        /// 已发运单
        /// </summary>
        public WayItemOrder[] Waybills
        {
            get
            {
                using (var view = new Views.WayItemsOrder(this.ID))
                {
                    return view.ToArray();
                }
            }
        }


        #endregion

        #region 持久化
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder Completed;
        public event SuccessHanlder Closed;
        public event SuccessHanlder PaySuccess;

        void InEnter()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Order);
                    reponsitory.Insert(this.ToLinq());

                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }

        public void Enter()
        {
            this.Invoice.Enter();
            this.Beneficiary.Enter();
            this.Consignee.Enter();
            this.Deliverer.Enter();

            this.InEnter();

            foreach (var item in this.Premiums)
            {
                item.InEnter();
            }

            foreach (var item in this.Items)
            {
                item.InEnter();
            }

            this.TransportTerm.ID = this.ID;
            this.TransportTerm.Enter();

            Refund(this.ID);

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 订单-完成
        /// </summary>
        public void Complete()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.CvOss.Orders>(new
                {
                    Status = this.Status = OrderStatus.Completed,
                    UpdateDate = this.UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
            if (this != null && this.Completed != null)
            {
                this.Completed(this, new SuccessEventArgs(this));
            }
        }
        /// <summary>
        /// 订单-关闭
        /// </summary>
        public void Close()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                this.Status = OrderStatus.Closed;
                this.UpdateDate = DateTime.Now;
                reponsitory.Update<Layer.Data.Sqls.CvOss.Orders>(new
                {
                    Status = this.Status,
                    UpdateDate = this.UpdateDate,
                }, item => item.ID == this.ID);
            }

            var prices = this.GetPaids();

            Refund(this.ID);

            if (this != null && this.Closed != null)
            {
                this.Closed(this, new SuccessEventArgs(this.ID));
            }
        }

        public event ErrorHanlder NotEnough;

        public void Pay(bool isCash = true, bool isCredit = true)
        {
            this.InPay(null, isCash, isCredit);
        }
        public void Pay(decimal amount, bool isCash = true, bool isCredit = true)
        {
            this.InPay(amount, isCash, isCredit);
        }
        public void AgentPay(decimal amount, bool isCash = true, bool isCredit = true)
        {
            this.InPay(amount, isCash, isCredit, Needs.Erp.Generic.Inner.Current.ID);
        }

        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="amount">支付金额</param>
        void InPay(decimal? amount = null, bool isCash = true, bool isCredit = true, string adminid = null)
        {
            if (!isCash && !isCredit)
            {
                return;
            }
            // 放在事物中
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                decimal paid = this.Paid;
                decimal paying = this.Total - paid;

                if (amount.HasValue)
                {
                    if (amount.Value > paying)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        paying = amount.Value;
                    }
                }

                var curreny = this.Beneficiary.Currency;
                decimal total = 0m;

                decimal cash = 0m;
                if (isCash)
                {
                    cash = this.Client.GetBalance(curreny, UserAccountType.Cash);
                    total += cash;
                }

                decimal credit = 0m;
                if (isCredit && total < paying)
                {
                    credit = this.Client.GetBalance(curreny, UserAccountType.Credit);
                    total += credit;
                }

                //是否足够支付订单
                if (total < paying)
                {
                    if (this != null && this.NotEnough != null)
                    {
                        this.NotEnough(this, new ErrorEventArgs());
                    }
                    return;
                }

                #region 借用
                decimal borrow = 0m;
                if (paying > cash)
                {
                    borrow = isCredit ? paying - cash : 0m;
                    //if (borrow > 0)
                    //{
                    reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                        ClientID = this.Client.ID,
                        Type = (int)UserAccountType.Credit,
                        From = (int)OutputTo.Pay,
                        OrderID = this.ID,
                        UserInputID = null,
                        Currency = (int)curreny,
                        Amount = borrow,
                        DateIndex = DateTime.Now.GetDateIndex(),
                        CreateDate = DateTime.Now,
                        AdminID = Needs.Erp.Generic.Inner.Current?.ID
                    });
                    //}
                }
                #endregion

                #region 花费

                decimal cost = isCash ? paying - borrow : 0m;
                if (cost > 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                        ClientID = this.Client.ID,
                        Type = (int)UserAccountType.Cash,
                        From = (int)OutputTo.Pay,
                        OrderID = this.ID,
                        UserInputID = null,
                        Currency = (int)curreny,
                        Amount = cost,
                        DateIndex = DateTime.Now.GetDateIndex(),
                        CreateDate = DateTime.Now,
                        AdminID = Needs.Erp.Generic.Inner.Current?.ID
                    });
                }

                #endregion

                reponsitory.Update<Layer.Data.Sqls.CvOss.Orders>(new
                {
                    Status = this.Status = OrderStatus.HasPaid,
                    UpdateDate = this.UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);

            }

            if (this != null && this.PaySuccess != null)
            {
                this.PaySuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion

        /// <summary>
        /// 订单退款
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="aciont"></param>
        static internal void Refund(string orderID, Entering aciont = null)
        {
            decimal old_total;

            using (var alls = new Views.OrderAlls())
            {
                var temp = alls[orderID];
                old_total = temp.Total;
            }

            aciont?.Invoke();

            decimal new_total;
            decimal paid;
            OrderPay[] paids;
            Models.Order order;
            using (var alls = new Views.OrderAlls())
            {
                order = alls[orderID];
                new_total = order.Total;
                paid = order.Paid;
                paids = order.GetPaids();
            }

            decimal total = old_total - new_total;

            if (total > 0)
            {
                //要退的总额
                decimal ding = paid - new_total;
                if (ding > 0)
                {
                    //退款
                    using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                    {
                        var left = ding;
                        foreach (var item in paids)
                        {
                            if (left < 0)
                            {
                                throw new Exception();
                            }

                            if (left == 0)
                            {
                                break;
                            }

                            var value = left > item.Price ? left : item.Price;
                            reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                                ClientID = order.Client.ID,
                                Type = (int)item.Type,
                                From = (int)OutputTo.Pay,
                                OrderID = order.ID,
                                UserInputID = null,
                                Currency = (int)order.Beneficiary.Currency,
                                Amount = 0 - Math.Abs(value),
                                DateIndex = item.Type == UserAccountType.Credit ? (int?)DateTime.Now.GetDateIndex() : null,
                                CreateDate = DateTime.Now,
                                AdminID = Needs.Erp.Generic.Inner.Current.ID
                            });
                            left -= value;
                        }
                    }
                }
            }
        }
    }
}
