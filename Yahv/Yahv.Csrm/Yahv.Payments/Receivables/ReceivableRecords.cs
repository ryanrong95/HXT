using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Layers.Data;
using Layers.Linq;
using Yahv.Linq.Extends;
using Yahv.Payments.Models;
using Yahv.Payments.Models.Origins;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views;
using Yahv.Payments.Views.Origins;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Events;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Settings;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 应收账款
    /// </summary>
    public class ReceivableRecords : Recorder
    {
        #region 构造函数
        public ReceivableRecords(PayInfo payInfo)
        {
            this.PayInfo = payInfo;
        }
        #endregion

        #region 记账

        #region 记账

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="waybillID">运单ID</param>
        /// <param name="id">应收ID</param>
        /// <param name="tinyID">小订单ID</param>
        /// <param name="rightPrice">实收金额</param>
        /// <param name="itemID"></param>
        /// <param name="applicationID">付汇申请ID</param>
        /// <param name="AgentID">代理人ID</param>
        /// <param name="source">来源</param>
        /// <param name="trackingNum">快递单号</param>
        override public string Record(Currency currency, decimal? price, string orderID = null, string waybillID = null, string id = null, string tinyID = null, decimal? rightPrice = null, string itemID = null, string applicationID = null, string AgentID = null, int? quantity = null, string source = "", string trackingNum = "", string data = "", decimal? originPrice = null)
        {
            id = id ?? PKeySigner.Pick(PKeyType.Receivables);

            //if (SubjectManager.Current[this.PayInfo.Conduct][this.PayInfo.Catalog].Subjects.All(item => item.Name != this.PayInfo.Subject) && !string.IsNullOrWhiteSpace(this.PayInfo.Subject))
            //{
            //    throw new Exception("该科目不存在!");
            //}

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            //应收
            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var statistics = new VouchersStatisticsView(reponsitory))
            {
                var orderEventArg = new OrderEventArgs(orderID);
                this.Fire(this, orderEventArg);

                //有订单ID，币种是未知的时候 提示错误信息
                if (orderEventArg.Currency == Currency.Unknown && !string.IsNullOrWhiteSpace(orderID))
                {
                    throw new Exception("订单币种不能为未知!");
                }

                //下单时的币种，为结算币种
                var currencySettle = orderEventArg.Currency;

                //如果订单ID为空，则按照传过来的币种为结算币种
                if (string.IsNullOrWhiteSpace(orderID))
                {
                    currencySettle = currency;
                }

                //现金收款
                if (rightPrice > 0)
                {
                    currencySettle = currency;

                    using (var payees = new PayeesTopView())
                    {
                        var payee = payees[PayInfo.Payee, Methord.Cash, currencySettle];

                        if (payee == null || string.IsNullOrWhiteSpace(payee.ID))
                        {
                            throw new Exception($"未找到收款人对应币种的现金账户!");
                        }

                        //收款账户ID
                        this.PayInfo.PayeeID = payee.ID;
                    }
                }

                var rate = ExchangeRates.Universal[currency, Currency.CNY];
                var rateSettle = ExchangeRates.Universal[currency, currencySettle];
                var rateUsd = ExchangeRates.Universal[currencySettle, Currency.CNY] / ExchangeRates.Universal[Currency.USD, Currency.CNY];

                DateTime currTime = DateTime.Now;
                //var originDate = GetOriginalDate(this.PayInfo.Payer, this.PayInfo.Payee, this.PayInfo.Conduct, this.PayInfo.Catalog, orderEventArg.OrderCreateDate ?? currTime);
                var originDate = GetOriginalDate(this.PayInfo.Payer, this.PayInfo.Payee, this.PayInfo.Conduct, this.PayInfo.Catalog, currTime);
                int dateIndex = GetDateIndexByDateTime(originDate);

                //没有实收金额，应收币种必须和订单币种一致
                //if (!(rightPrice.HasValue && rightPrice.Value > 0) && orderID.StartsWith("Order"))
                //{
                //    if (currency != orderEventArg.Currency)
                //    {
                //        throw new Exception($"订单币种为{orderEventArg.Currency.GetDescription()}，和应收币种不一致!");
                //    }
                //}

                #region 添加应收
                var entity = new Receivable()
                {
                    ID = id,
                    Payer = this.PayInfo.Payer,
                    PayerID = this.PayInfo.PayerID,
                    PayerAnonymous = this.PayInfo.PayerAnonymous,
                    Payee = this.PayInfo.Payee,
                    PayeeID = this.PayInfo.PayeeID,
                    PayeeAnonymous = this.PayInfo.PayeeAnonymous,
                    Business = this.PayInfo.Conduct,
                    Catalog = this.PayInfo.Catalog,
                    Subject = this.PayInfo.Subject,
                    TinyID = tinyID,
                    ItemID = itemID,
                    ApplicationID = applicationID,

                    Currency = currency,
                    Price = price ?? 0m,

                    Quantity = quantity,

                    Currency1 = Currency.CNY,
                    Rate1 = rate,
                    Price1 = (price * rate) ?? 0m,

                    Currency11 = Currency.USD,
                    Rate11 = rateUsd,
                    Price11 = ((price * rateSettle * rateUsd) ?? 0m).Round(),

                    SettlementCurrency = currencySettle,
                    SettlementPrice = ((price * rateSettle) ?? 0m).Round(),
                    SettlementRate = rateSettle,

                    OrderID = orderID,
                    WaybillID = waybillID,
                    AdminID = this.PayInfo.Inputer.ID,
                    Summay = string.Empty,

                    CreateDate = currTime,
                    OriginalIndex = dateIndex,
                    OriginalDate = originDate.Date,
                    ChangeDate = originDate.Date,
                    ChangeIndex = dateIndex,
                    Source = source,
                    TrackingNumber = trackingNum,
                    Data = data,
                };

                entity.Enter(reponsitory);
                #endregion

                #region 添加应收日志
                if (originPrice != null)
                {
                    var entityLog = new Log_Receivable()
                    {
                        ID = PKeySigner.Pick(PKeyTypes.Logs_Receivable),
                        OriginID = id,
                        Payer = this.PayInfo.Payer,
                        PayerID = this.PayInfo.PayerID,
                        PayerAnonymous = this.PayInfo.PayerAnonymous,
                        Payee = this.PayInfo.Payee,
                        PayeeID = this.PayInfo.PayeeID,
                        PayeeAnonymous = this.PayInfo.PayeeAnonymous,
                        Business = this.PayInfo.Conduct,
                        Catalog = this.PayInfo.Catalog,
                        Subject = this.PayInfo.Subject,
                        TinyID = tinyID,
                        ItemID = itemID,
                        ApplicationID = applicationID,

                        Currency = currency,
                        Price = originPrice ?? 0m,

                        Quantity = quantity,

                        Currency1 = Currency.CNY,
                        Rate1 = rate,
                        Price1 = (originPrice * rate) ?? 0m,

                        Currency11 = Currency.USD,
                        Rate11 = rateUsd,
                        Price11 = ((originPrice * rateSettle * rateUsd) ?? 0m).Round(),

                        SettlementCurrency = currencySettle,
                        SettlementPrice = ((originPrice * rateSettle) ?? 0m).Round(),
                        SettlementRate = rateSettle,

                        OrderID = orderID,
                        WaybillID = waybillID,
                        AdminID = this.PayInfo.Inputer.ID,
                        Summay = string.Empty,

                        CreateDate = currTime,
                        OriginalIndex = dateIndex,
                        OriginalDate = originDate.Date,
                        ChangeDate = originDate.Date,
                        ChangeIndex = dateIndex,
                        Source = source,
                        TrackingNumber = trackingNum,
                        Data = data,
                    };
                    entityLog.Enter(reponsitory);
                }
                #endregion

                #region 添加实收（现金支付）
                //实收金额大于0，添加实收
                if (rightPrice > 0)
                {
                    //PaymentManager.Erp(this.PayInfo.Inputer.ID).Received.For(entity.ID).Record(currency, rightPrice ?? 0);
                    string flowId = PKeySigner.Pick(PKeyType.FlowAccount);

                    //添加流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.Cash,
                        AdminID = this.PayInfo.Inputer.ID,
                        Business = this.PayInfo.Conduct,
                        CreateDate = currTime,
                        OrderID = orderID,
                        Payee = this.PayInfo.Payee,
                        Payer = this.PayInfo.Payer,

                        Price = price ?? 0m,
                        Currency = (int)currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = (price * rate) ?? 0m,
                        DateIndex = dateIndex,
                    });

                    //核销流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = flowId,
                        Type = (int)AccountType.Cash,
                        AdminID = this.PayInfo.Inputer.ID,
                        Business = this.PayInfo.Conduct,
                        CreateDate = currTime,
                        OrderID = orderID,
                        Payee = this.PayInfo.Payee,
                        Payer = this.PayInfo.Payer,

                        Price = (price ?? 0m) * -1,
                        Currency = (int)currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = (price ?? 0m) * rate * -1,
                        DateIndex = dateIndex,
                    });

                    //添加实收
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                    {
                        ID = id,
                        CreateDate = currTime,
                        WaybillID = waybillID,
                        Price = price ?? 0m,
                        AdminID = this.PayInfo.Inputer.ID,
                        OrderID = orderID,
                        ReceivableID = entity.ID,
                        AccountType = (int)AccountType.Cash,
                        FlowID = flowId,

                        Currency1 = (int)Currency.CNY,
                        Rate1 = rate,
                        Price1 = (price ?? 0m) * rate,
                        Source = source,
                    });
                }
                #endregion

                #region 添加申请信息
                var voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
                {
                    OrderID = orderID,
                    ApplicationID = applicationID,
                    Type = VoucherType.Receipt,
                    Payer = this.PayInfo.Payer,
                    Payee = this.PayInfo.Payee,
                    CreatorID = this.PayInfo.Inputer.ID,
                    Currency = currencySettle,
                    CreateDate = currTime,
                };

                voucherEntity.Enter(reponsitory);
                #endregion

                #region 统一提交
                reponsitory.Submit();
                #endregion

                #region 调用外部事件，更新订单状态
                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                    List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();
                    if (!orderID.StartsWith("LsOrder"))
                    {
                        whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                        {
                            Source = SourceType.Receivable,
                            OrderID = orderID,
                            OperatorID = this.PayInfo.Inputer.ID,
                            Status = statistics.Where(item => !item.OrderID.Contains("LsOrder"))
                            .Where(GetExpressionVoucherStatistics(statistics, this.PayInfo.Payer, this.PayInfo.Payee, orderID, applicationID))
                            .ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                        });
                    }
                    else
                    {
                        if (statistics.Where(item => item.OrderID.Contains("LsOrder")).Where(GetExpressionVoucherStatistics(statistics, this.PayInfo.Payer, this.PayInfo.Payee, orderID, applicationID)).ToList().Sum(item => item.Remains) == 0)
                        {
                            lsEventArgs.Add(new LsPayConfirmedEventArgs()
                            {
                                LsOrderID = orderID,
                                OperatorID = this.PayInfo.Inputer.ID,
                            });
                        }
                    }

                    if (whsEventArgs.Count > 0)
                    {
                        this.Fire(null, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                    }

                    if (lsEventArgs.Count > 0)
                    {
                        this.Fire(null, new ConfirmedEventArgs<LsPayConfirmedEventArgs>(lsEventArgs.ToArray()));
                    }
                }
                #endregion
            }

            return id;
        }

        #region bak
        //override public string Record(Currency currency, decimal? price, string orderID = null, string waybillID = null, string id = null, string tinyID = null, string supplierSign = null, decimal? rightPrice = null, string itemID = null, string applicationID = null, string AgentID = null)
        //{
        //    id = id ?? PKeySigner.Pick(PKeyType.Receivables);

        //    if (SubjectManager.Current[this.PayInfo.Conduct][this.PayInfo.Catalog].Subjects.All(item => item.Name != this.PayInfo.Subject))
        //    {
        //        throw new Exception("该科目不存在!");
        //    }

        //    if (price < 0)
        //    {
        //        throw new Exception("金额不能小于0!");
        //    }

        //    //应收
        //    using (var reponsitory = new PvbCrmReponsitory())
        //    using (var statistics = new VouchersStatisticsView(reponsitory))
        //    {
        //        var orderEventArg = new OrderEventArgs(orderID);
        //        this.Fire(this, orderEventArg);

        //        if (orderEventArg.Currency == Currency.Unknown)
        //        {
        //            throw new Exception("订单币种不能为未知!");
        //        }

        //        //下单时的币种，为结算币种
        //        var currencySettle = orderEventArg.Currency;
        //        if (rightPrice > 0)
        //        {
        //            currencySettle = currency;

        //            this.PayInfo.Payer = null;
        //            this.PayInfo.PayerAnonymous = WsFixedCarrier.Current.Name;
        //        }

        //        var rate = ExchangeRates.Floating[currency, Currency.CNY];
        //        var rateSettle = ExchangeRates.Floating[currency, currencySettle];

        //        var originDate = GetOriginalDate(this.PayInfo.Payer, this.PayInfo.Payee, this.PayInfo.Conduct, this.PayInfo.Catalog, orderEventArg.OrderCreateDate ?? DateTime.Now);
        //        int dateIndex = GetDateIndexByDateTime(originDate);


        //        //没有实收金额，应收币种必须和订单币种一致
        //        //if (!(rightPrice.HasValue && rightPrice.Value > 0) && orderID.StartsWith("Order"))
        //        //{
        //        //    if (currency != orderEventArg.Currency)
        //        //    {
        //        //        throw new Exception($"订单币种为{orderEventArg.Currency.GetDescription()}，和应收币种不一致!");
        //        //    }
        //        //}

        //        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receivables()
        //        {
        //            ID = id,
        //            Payer = this.PayInfo.Payer,
        //            PayerAnonymous = this.PayInfo.PayerAnonymous,
        //            Payee = this.PayInfo.Payee,
        //            PayeeAnonymous = this.PayInfo.PayeeAnonymous,
        //            Business = this.PayInfo.Conduct,
        //            Catalog = this.PayInfo.Catalog,
        //            Subject = this.PayInfo.Subject,
        //            TinyID = tinyID,
        //            SupplierSign = supplierSign,
        //            ItemID = itemID,
        //            ApplicationID = applicationID,

        //            Currency = (int)currency,
        //            Price = price ?? 0m,

        //            Currency1 = (int)Currency.CNY,
        //            Rate1 = rate,
        //            Price1 = (price * rate) ?? 0m,

        //            SettlementCurrency = (int)currencySettle,
        //            SettlementPrice = ((price * rateSettle) ?? 0m).Round(),
        //            SettlementRate = rateSettle,

        //            OrderID = orderID,
        //            WaybillID = waybillID,
        //            CreateDate = DateTime.Now,
        //            AdminID = this.PayInfo.Inputer.ID,
        //            Summay = string.Empty,

        //            OriginalIndex = dateIndex,
        //            OriginalDate = originDate.Date,
        //            ChangeDate = originDate.Date,
        //            ChangeIndex = dateIndex,
        //            Status = (int)Underly.GeneralStatus.Normal,
        //        });

        //        //实收金额大于0，添加实收
        //        if (rightPrice > 0)
        //        {
        //            PaymentManager.Erp(this.PayInfo.Inputer.ID).Received.For(id).Record(currency, rightPrice ?? 0);
        //        }

        //        #region 添加账单信息
        //        var voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
        //        {
        //            OrderID = orderID,
        //            ApplicationID = applicationID,
        //            Type = VoucherType.Receipt,
        //            Payer = this.PayInfo.Payer,
        //            Payee = this.PayInfo.Payee,
        //            CreatorID = this.PayInfo.Inputer.ID,
        //            AgentID = AgentID,
        //            Currency = orderEventArg.Currency,
        //        };

        //        voucherEntity.Enter();
        //        #endregion

        //        #region 调用外部事件，更新订单状态
        //        List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
        //        List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();
        //        if (!orderID.StartsWith("LsOrder"))
        //        {
        //            whsEventArgs.Add(new WhsPayConfirmedEventArgs()
        //            {
        //                Source = SourceType.Receivable,
        //                OrderID = orderID,
        //                OperatorID = this.PayInfo.Inputer.ID,
        //                Status = statistics.Where(item => !item.OrderID.Contains("LsOrder"))
        //                .Where(GetExpressionVoucherStatistics(statistics, this.PayInfo.Payer, this.PayInfo.Payee, orderID, applicationID))
        //                .ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
        //            });
        //        }
        //        else
        //        {
        //            if (statistics.Where(item => item.OrderID.Contains("LsOrder")).Where(GetExpressionVoucherStatistics(statistics, this.PayInfo.Payer, this.PayInfo.Payee, orderID, applicationID)).ToList().Sum(item => item.Remains) == 0)
        //            {
        //                lsEventArgs.Add(new LsPayConfirmedEventArgs()
        //                {
        //                    LsOrderID = orderID,
        //                    OperatorID = this.PayInfo.Inputer.ID,
        //                });
        //            }
        //        }

        //        if (whsEventArgs.Count > 0)
        //        {
        //            this.Fire(null, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
        //        }

        //        if (lsEventArgs.Count > 0)
        //        {
        //            this.Fire(null, new ConfirmedEventArgs<LsPayConfirmedEventArgs>(lsEventArgs.ToArray()));
        //        }
        //        #endregion
        //    }

        //    return id;
        //}
        #endregion

        #endregion

        #region 仓储费

        /// <summary>
        /// 仓储费
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <remarks>如果没有进行添加，有的话进行累加</remarks>
        override public string RecordStorage(Currency currency, decimal price)
        {
            string receivableId = string.Empty;

            if (SubjectManager.Current[this.PayInfo.Conduct][this.PayInfo.Catalog].Subjects.All(item => item.Name != this.PayInfo.Subject))
            {
                throw new Exception("该科目不存在!");
            }

            if (price <= 0)
            {
                throw new Exception("金额不能小于等于0!");
            }

            //应收
            using (var reponsitory = new PvbCrmReponsitory())
            using (var receivables = new ReceivablesView())
            {
                var receivable =
                   receivables.SingleOrDefault(item => item.Payer == PayInfo.Payer && item.Payee == PayInfo.Payee
                                                       && (item.OrderID == null || item.OrderID == string.Empty) && item.Currency == currency
                                                       && item.Business == Underly.Business.WarehouseServicing.GetDescription() && item.Catalog == "杂费" && item.Subject == "仓储费");

                receivableId = receivable?.ID;     //应收ID

                //新增
                if (receivable == null || string.IsNullOrWhiteSpace(receivable.ID))
                {
                    var rate = ExchangeRates.Universal[currency, Currency.CNY];
                    var currencySettle = Currency.CNY;
                    var rateSettle = ExchangeRates.Universal[currency, currencySettle];
                    var rateUsd = ExchangeRates.Universal[currencySettle, Currency.CNY] / ExchangeRates.Universal[Currency.USD, Currency.CNY];

                    var originDate = GetOriginalDate(this.PayInfo.Payer, this.PayInfo.Payee, this.PayInfo.Conduct, this.PayInfo.Catalog, DateTime.Now);
                    int dateIndex = GetDateIndexByDateTime(originDate);

                    receivableId = PKeySigner.Pick(PKeyType.Receivables);

                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receivables()
                    {
                        ID = receivableId,
                        Payer = this.PayInfo.Payer,
                        Payee = this.PayInfo.Payee,
                        Business = this.PayInfo.Conduct,
                        Catalog = this.PayInfo.Catalog,
                        Subject = this.PayInfo.Subject,
                        OrderID = null,

                        Currency = (int)currency,
                        Price = price,

                        Currency1 = (int)Currency.CNY,
                        Rate1 = rate,
                        Price1 = price * rate,

                        Currency11 = (int)Currency.USD,
                        Rate11 = rateUsd,
                        Price11 = (price * rateSettle * rateUsd).Round(),

                        SettlementCurrency = (int)currencySettle,
                        SettlementPrice = base.Round(price * rateSettle),
                        SettlementRate = rateSettle,

                        CreateDate = DateTime.Now,
                        AdminID = this.PayInfo.Inputer.ID,

                        OriginalIndex = dateIndex,
                        OriginalDate = originDate.Date,
                        ChangeDate = originDate.Date,
                        ChangeIndex = dateIndex,
                        Status = (int)Underly.GeneralStatus.Normal,
                    });
                }
                //累加
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                    {
                        Price = receivable.Price + price,
                        Price1 = receivable.Price1 + (price * receivable.Rate1),
                        Price11 = receivable.Price11 + (price * receivable.Rate1 * receivable.Rate11),
                        SettlementPrice = receivable.SettlementPrice + (price * receivable.Rate1),
                    }, item => item.ID == receivable.ID);
                }
            }

            return receivableId;
        }
        #endregion

        #endregion

        #region 私有函数

        /// <summary>
        /// 获取账期还款日
        /// </summary>
        /// <param name="payer">付款人（客户）</param>
        /// <param name="payee">收款人（公司）</param>
        /// <returns></returns>
        internal DateTime GetOriginalDate(string payer, string payee, string conduct, string catalog, DateTime orderDateTime)
        {
            /*
                示例

                日期：2019-01-05
                月数：2
                还款日：10

                还款时间：
                月结：2019-03-10
                约定期限：2019-03-15
            */

            var dateTime = orderDateTime;
            //获取账期
            using (var debtView = new DebtTermsTopView<PvbCrmReponsitory>())
            using (var monthSealedView = new MonthSealedBillsOrigin())
            {
                bool isSealed = monthSealedView[payer, conduct, Int32.Parse(orderDateTime.ToString("yyyyMM"))];
                if (isSealed)
                {
                    dateTime = DateTime.Now;
                }

                var debtTerms = debtView.SingleOrDefault(item => item.Payer == payer && item.Payee == payee && item.Business == conduct && item.Catalog == catalog);
                if (debtTerms == null)
                {
                    return dateTime;
                }

                //月结
                if (debtTerms.SettlementType == SettlementType.Month)
                {
                    dateTime = dateTime.AddMonths(debtTerms.Months);
                    dateTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM") + "-" + debtTerms.Days);
                }
                //约定期限
                else if (debtTerms.SettlementType == SettlementType.DueTime)
                {
                    dateTime = dateTime.AddMonths(debtTerms.Months).AddDays(debtTerms.Days);
                }


                return dateTime;
            }
        }

        internal DateTime GetOriginalDate(string payer, string payee, string conduct, string catalog, DateTime orderDateTime, Services.Models.DebtTerm[] array)
        {
            /*
                示例

                日期：2019-01-05
                月数：2
                还款日：10

                还款时间：
                月结：2019-03-10
                约定期限：2019-03-15
            */

            var dateTime = orderDateTime;
            //获取账期
            var debtTerms = array.SingleOrDefault(item => item.Payer == payee && item.Payee == payer && item.Business == conduct && item.Catalog == catalog);
            if (debtTerms == null)
            {
                return dateTime;
            }

            //月结
            if (debtTerms.SettlementType == SettlementType.Month)
            {
                dateTime = dateTime.AddMonths(debtTerms.Months);
                dateTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM") + "-" + debtTerms.Days);
            }
            //约定期限
            else if (debtTerms.SettlementType == SettlementType.DueTime)
            {
                dateTime = dateTime.AddMonths(debtTerms.Months).AddDays(debtTerms.Days);
            }

            return dateTime;
        }

        /// <summary>
        /// 根据日期获取账期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        internal int GetDateIndexByDateTime(DateTime dt)
        {
            return int.Parse(dt.ToString("yyyyMM"));

            int dateIndex = 0;

            //获取封账日
            int closedDay = SettingsManager<IPaysSettings>.Current.ClosedDay;
            int days = dt.Day;
            if (days <= closedDay)
            {
                DateTime dateTime = dt.AddMonths(-1);
                dateIndex = int.Parse(DateTime.Parse($"{dateTime.Year}-{dateTime.Month}").ToString("yyyyMM"));
            }
            else
            {
                dateIndex = int.Parse(DateTime.Parse($"{dt.Year}-{dt.Month}").ToString("yyyyMM"));
            }

            return dateIndex;
        }

        /// <summary>
        /// 获取对账单列表
        /// </summary>
        private Expression<Func<VoucherStatistic, bool>> GetExpressionVoucherStatistics(VouchersStatisticsView vouchers, string payer, string payee, string orderID, string applicationID, Currency currency = Currency.Unknown)
        {
            Expression<Func<VoucherStatistic, bool>> predication = item => true;


            if (!string.IsNullOrWhiteSpace(payer))
            {
                predication = predication.And(item => item.Payer == payer && item.Payee == payee && (item.OrderID == orderID || item.OrderID == null));
            }
            //匿名支付
            else
            {
                predication = predication.And(item => item.Payer == null && item.Payee == payee && (item.OrderID == orderID));
            }

            if (!string.IsNullOrWhiteSpace(applicationID))
            {
                predication = predication.And(item => item.ApplicationID == applicationID);
            }

            if (currency != Currency.Unknown)
            {
                predication = predication.And(item => item.Currency == currency);
            }

            return predication;
        }
        #endregion
    }
}
