using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Layers.Data.Sqls.ScCustoms;
using Layers.Linq;
using Yahv.Linq.Extends;
using Yahv.Payments.Models;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using EnterprisesTopView = Yahv.Payments.Views.EnterprisesTopView;
using VouchersStatisticsView = Yahv.Payments.Views.VouchersStatisticsView;

namespace Yahv.Payments
{
    /// <summary>
    /// 实收
    /// </summary>
    public class Receiveds : Services.ReceivedBase
    {
        private IEnumerable<Receivable> receivables;
        private IEnumerable<Fee> fees;      //核销科目金额
        Inputer inputer;

        #region 构造函数

        public Receiveds(Inputer inputer)
        {
            this.inputer = inputer;
        }

        /// <summary>
        /// 应收记录 
        /// </summary>
        /// <param name="array">应收ID 或者 小订单ID</param>
        /// <returns></returns>
        public Receiveds For(params string[] array)
        {
            using (var view = new ReceivablesView())
            {
                receivables = view.Where(item => array.Contains(item.ID) || array.Contains(item.TinyID)).ToArray();
            }

            return this;
        }

        public Receiveds For(params Receivable[] receivables)
        {
            this.receivables = receivables;
            return this;
        }

        public Receiveds For(params Fee[] _fees)
        {
            //如果大于0,按照录入金额进行核销
            //如果没有录入值，则不进行核销
            if (_fees.Sum(item => item.RightPrice) > 0)
            {
                this.fees = _fees.Where(item => item.RightPrice > 0);
            }

            //如果小于0,用银行流水，默认核销
            return this.For(this.fees.Select(item => item.LeftID).ToArray());
        }
        #endregion

        #region 记账
        #region 减免账户
        /// <summary>
        /// 减免
        /// </summary>
        public void Reduction(Currency currency, decimal price, string orderId = null)
        {
            if (receivables == null || !receivables.Any())
            {
                throw new Exception("未找到应收信息!");
            }

            if (receivables.Count() != 1)
            {
                throw new Exception("一次只能添加一次减免!");
            }

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var vouchers = new VouchersStatisticsView())
            {
                var first = receivables.FirstOrDefault();
                var rate = ExchangeRates.Universal[currency, Currency.CNY];
                var voucher = vouchers.FirstOrDefault(item => item.ReceivableID == first.ID);

                //如果减免金额大于待支付金额，按照待支付金额计算
                price = price > voucher.Remains ? voucher.Remains : price;

                string flowId = PKeySigner.Pick(PKeyType.FlowAccount);
                decimal oper = -1;


                //添加流水
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                {
                    ID = flowId,
                    Type = (int)AccountType.Reduction,
                    AdminID = inputer.ID,
                    Business = first.Business,
                    CreateDate = DateTime.Now,
                    Payee = first.Payee,
                    Payer = first.Payer,
                    Catalog = first.Catalog,
                    Subject = first.Subject,
                    OrderID = first?.OrderID,
                    WaybillID = first?.WaybillID,

                    Price = price * oper,
                    Currency = (int)currency,

                    Currency1 = (int)Currency.CNY,
                    ERate1 = rate,
                    Price1 = price * rate * oper,
                    DateIndex = first.ChangeIndex,
                });

                //添加实收
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                {
                    ID = PKeySigner.Pick(PKeyType.Receiveds),
                    CreateDate = DateTime.Now,
                    WaybillID = first?.WaybillID,
                    Price = price,
                    AdminID = inputer.ID,
                    ReceivableID = first.ID,
                    AccountType = (int)AccountType.Reduction,
                    FlowID = flowId,
                    OrderID = first?.OrderID,

                    Currency1 = (int)Currency.CNY,
                    Rate1 = rate,
                    Price1 = price * rate,
                });

                //更新仓储费ID
                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    StorageCallback(vouchers, currency, orderId, reponsitory, first);
                }

                //统一提交
                reponsitory.Submit();

                #region 调用外部事件
                List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();
                if (!string.IsNullOrWhiteSpace(first.OrderID))
                {
                    if (!first.OrderID.StartsWith("LsOrder"))
                    {
                        whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                        {
                            Source = SourceType.Reduction,
                            OrderID = first.OrderID,
                            OperatorID = inputer.ID,
                            Status = vouchers.Where(item => item.OrderID == first.OrderID
                                                        && !item.OrderID.Contains("LsOrder")
                                                        && item.Payer == first.Payer
                                                        && item.Payee == first.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                        });
                    }
                    else
                    {
                        if (vouchers.Where(item => item.OrderID == first.OrderID
                                               && item.OrderID.Contains("LsOrder")
                                               && item.Payer == first.Payer
                                               && item.Payee == first.Payee).ToList().Sum(item => item.Remains) == 0)
                        {
                            lsEventArgs.Add(new LsPayConfirmedEventArgs()
                            {
                                LsOrderID = first.OrderID,
                                OperatorID = inputer.ID,
                            });
                        }
                    }

                    if (whsEventArgs.Count > 0)
                    {
                        this.Fire(first, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                    }

                    if (lsEventArgs.Count > 0)
                    {
                        this.Fire(first, new ConfirmedEventArgs<LsPayConfirmedEventArgs>(lsEventArgs.ToArray()));
                    }
                }
                #endregion
            }
        }
        #endregion

        #region 取消减免
        /// <summary>
        /// 取消减免
        /// </summary>
        public void ReductionCancel(string receivedID)
        {
            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var receivedView = new ReceivedsView(reponsitory))
            using (var vouchers = new VouchersStatisticsView())
            {
                var received = receivedView[receivedID];
                if (received == null || string.IsNullOrWhiteSpace(received.ID))
                {
                    throw new Exception("未找到减免数据!");
                }

                if (received.AccountType != AccountType.Reduction)
                {
                    throw new Exception("您传入的数据不是减免数据!");
                }

                //根据实收获取账单信息
                var voucher = vouchers.Single(item => item.ReceivableID == received.ReceivableID);
                //删除流水
                reponsitory.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => item.ID == received.FlowID);
                //删除实收
                reponsitory.Delete<Layers.Data.Sqls.PvbCrm.Receiveds>(item => item.ID == receivedID);
                //提交
                reponsitory.Submit();

                #region 调用外部事件
                List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                if (!string.IsNullOrWhiteSpace(received.OrderID))
                {
                    if (!received.OrderID.StartsWith("LsOrder"))
                    {
                        whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                        {
                            Source = SourceType.Reduction,
                            OrderID = received.OrderID,
                            OperatorID = inputer.ID,
                            Status = vouchers.Where(item => item.OrderID == received.OrderID
                                                        && !item.OrderID.Contains("LsOrder")
                                                        && item.Payer == voucher.Payer
                                                        && item.Payee == voucher.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                        });
                    }


                    if (whsEventArgs.Count > 0)
                    {
                        this.Fire(received, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                    }
                }
                #endregion
            }
        }
        #endregion

        #region 财务收款确认
        /// <summary>
        /// 财务收款确认
        /// </summary>
        /// <param name="input"></param>
        public VoucherResult Confirm(VoucherInput input)
        {
            var result = new VoucherResult() { Success = false };

            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var statistics = new VouchersStatisticsView(reponsitory))
            using (var bankFlows = new BankFlowAccountsView(reponsitory))
            using (var subjectsView = new SubjectsView(reponsitory))
            {
                input = ChangeNameToId(input);

                var orderIDs = this.receivables.Where(item => item.OrderID != null).GroupBy(item => item.OrderID).OrderBy(item => item.Key).Select(item => item.Key);
                var vouchers = statistics.Where(GetExpressionVoucherStatistics(statistics, input, currency: input.Currency, receivableIds: receivables.Select(item => item.ID).ToArray(), orderIDs: orderIDs.ToArray())).OrderBy(item => item.OrderID);
                var rate = ExchangeRates.Universal[input.Currency, Currency.CNY];
                var bankFlow = bankFlows[input.FormCode];       //银行收款流水



                if (input.Currency != bankFlow.Currency)
                {
                    throw new Exception("币种不一致!");
                }

                if (string.IsNullOrWhiteSpace(bankFlow.FlowAccountID))
                {
                    throw new Exception("未找到该银行流水!");
                }


                decimal total = vouchers.ToArray().Sum(item => item.Remains);
                decimal remainPrice = bankFlow.Price;      //剩余金额

                //if (bankFlow.Business != input.Business)
                //{
                //    throw new Exception($"银行流水业务和核销账单业务不同!");
                //}

                //根据录入金额进行核销
                if (Math.Abs(input.Price) > 0)
                {
                    if (Math.Abs(input.Price) > bankFlow.Price)
                    {
                        throw new Exception("银行流水剩余金额不足!");
                    }
                    else
                    {
                        remainPrice = Math.Abs(input.Price);
                    }
                }


                //if (remainPrice < total)
                //{
                //    throw new Exception("银行流水剩余金额不足!");
                //}

                #region 回写仓储费ID
                StorageCallback(statistics, orderIDs.ToArray(), reponsitory, input);
                #endregion

                #region 核销账单
                //DeductionVouchers(reponsitory, vouchers, input, rate: rate);
                decimal payPrice = 0;
                Subject subject;
                string flowId_Temp;
                string payer_Actual = input.Payer;

                //匿名
                if (IsAnonymous(input.Payer) && input.Payer != input.PayerNew)
                {
                    payer_Actual = input.PayerNew;
                }

                foreach (var voucher in vouchers)
                {
                    if (bankFlow.Payee != voucher.Payee || bankFlow.Payer != voucher.Payer)
                    {
                        throw new Exception("预收账款流水不能核销其他公司!");
                    }

                    //允许实收大于应收
                    //if (voucher.Remains <= 0 || (string.IsNullOrWhiteSpace(voucher.OrderID) && string.IsNullOrWhiteSpace(voucher.ApplicationID)))
                    //{
                    //    continue;
                    //}

                    //根据应收（剩余应收）进行核销
                    //payPrice = remainPrice > voucher.Remains ? voucher.Remains : remainPrice;
                    payPrice = remainPrice;

                    //如果核销金额为0，默认核销剩余应收
                    if (input.Price == 0)
                    {
                        payPrice = voucher.Remains;
                    }

                    //核销科目金额有值，根据核销金额进行核销
                    if (fees != null && fees.Any(item => item.LeftID == voucher.ReceivableID))
                    {
                        payPrice = fees.Single(item => item.LeftID == voucher.ReceivableID).RightPrice ?? 0;
                    }

                    if (payPrice <= 0)
                    {
                        continue;
                    }

                    flowId_Temp = PKeySigner.Pick(PKeyType.FlowAccount);

                    //添加实收
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                    {
                        ID = PKeySigner.Pick(PKeyType.Receiveds),
                        CreateDate = input.CreateDate,
                        WaybillID = voucher.WaybillID,
                        Price = payPrice,
                        AdminID = inputer.ID,
                        OrderID = voucher.OrderID,
                        ReceivableID = voucher.ReceivableID,
                        AccountType = (int)input.AccountType,
                        FlowID = flowId_Temp,

                        Currency1 = (int)Currency.CNY,
                        Rate1 = rate,
                        Price1 = payPrice * rate,
                    });

                    //核销流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = flowId_Temp,
                        Type = (int)input.AccountType,
                        AdminID = inputer.ID,
                        Business = input.Business,
                        CreateDate = input.CreateDate,
                        OrderID = voucher.OrderID,
                        Payee = input.Payee,
                        Payer = payer_Actual,

                        Price = -payPrice,
                        Currency = (int)input.Currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = -payPrice * rate,

                        Account = bankFlow?.Account,
                        Bank = bankFlow?.Bank,
                        FormCode = bankFlow?.FormCode,
                    });

                    //判断科目是否需要转给客户余额
                    subject = subjectsView[SubjectType.Input, voucher.Business, voucher.Catalog, voucher.Subject];
                    if (subject != null && subject.IsToCustomer)
                    {
                        var orderEventArg = new Services.Events.OrderEventArgs(voucher.OrderID);
                        this.Fire(this, orderEventArg);

                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                        {
                            ID = PKeySigner.Pick(PKeyType.FlowAccount),
                            Type = (int)AccountType.Cash,
                            AdminID = inputer.ID,
                            Business = input.Business,
                            CreateDate = input.CreateDate,
                            OrderID = voucher.OrderID,
                            Payee = orderEventArg.ClientID,
                            Payer = input.Payee,

                            Price = payPrice,
                            Currency = (int)input.Currency,

                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = payPrice * rate,

                            Account = bankFlow?.Account,
                            Bank = bankFlow?.Bank,
                            FormCode = bankFlow?.FormCode,
                        });
                    }

                    remainPrice -= payPrice;

                    result.WriteOffPrice += payPrice;
                }
                #endregion

                #region 更新匿名

                ModifyAnonymous(input.PayerNew, reponsitory);
                #endregion

                #region 统一提交
                reponsitory.Submit();
                #endregion

                #region 修改订单状态

                if (orderIDs.Any(item => item != null))
                {
                    List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                    List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();

                    foreach (var orderId in orderIDs)
                    {
                        if (!string.IsNullOrWhiteSpace(orderId) && !orderId.StartsWith("LsOrder"))
                        {
                            whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                            {
                                Source = SourceType.Confirm,
                                OrderID = orderId,
                                OperatorID = input.CreatorID,
                                ApplicationID = input.ApplicationID,
                                Status = statistics.Where(item => !item.OrderID.Contains("LsOrder")).Where(GetExpressionVoucherStatistics(statistics, input, orderIDs: orderId)).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                            });
                        }
                        else
                        {
                            if (statistics.Where(item => item.OrderID.Contains("LsOrder")).Where(GetExpressionVoucherStatistics(statistics, input, orderIDs: orderId)).ToList().Sum(item => item.Remains) == 0)
                            {
                                lsEventArgs.Add(new LsPayConfirmedEventArgs()
                                {
                                    LsOrderID = orderId,
                                    OperatorID = input.CreatorID,
                                });
                            }
                        }
                    }

                    if (whsEventArgs.Count > 0)
                    {
                        this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                    }

                    if (lsEventArgs.Count > 0)
                    {
                        this.Fire(input, new ConfirmedEventArgs<LsPayConfirmedEventArgs>(lsEventArgs.ToArray()));
                    }
                }
                #endregion
            }

            result.Success = true;
            result.Message = "操作成功!";
            return result;
        }
        #endregion

        #region 本位币财务收款确认
        /// <summary>
        /// 本位币财务收款确认
        /// </summary>
        /// <param name="input"></param>
        //public void ConfirmRmb(VoucherInput input)
        //{
        //    using (var reponsitory = new PvbCrmReponsitory(false))
        //    using (var statistics = new Yahv.Payments.Views.VouchersCnyStatisticsView(reponsitory))
        //    using (var bankFlows = new BankFlowAccountsView(reponsitory))
        //    using (var subjectsView = new SubjectsView(reponsitory))
        //    using (var receivablesView = new ReceivablesView(reponsitory))
        //    {
        //        var orderIDs = this.receivables.Where(item => item.OrderID != null).GroupBy(item => item.OrderID).OrderBy(item => item.Key).Select(item => item.Key);
        //        var vouchers = statistics.Where(GetExpressionVoucherCnyStatistics(statistics, input, currency: input.Currency, receivableIds: receivables.Select(item => item.ID).ToArray(), orderIDs: orderIDs.ToArray())).OrderBy(item => item.OrderID);
        //        var bankFlow = bankFlows[input.FormCode];       //银行收款流水

        //        if (input.Currency != Currency.CNY)
        //        {
        //            throw new Exception("必须为人民币!");
        //        }

        //        if (bankFlow.Currency != Currency.CNY)
        //        {
        //            throw new Exception("必须为人民币!");
        //        }

        //        if (string.IsNullOrWhiteSpace(bankFlow.FlowAccountID))
        //        {
        //            throw new Exception("未找到该银行流水!");
        //        }


        //        decimal total = vouchers.ToArray().Sum(item => item.Remains);
        //        decimal remainPrice = bankFlow.Price;      //剩余金额

        //        //根据录入金额进行核销
        //        if (Math.Abs(input.Price) > 0)
        //        {
        //            if (Math.Abs(input.Price) > bankFlow.Price)
        //            {
        //                throw new Exception("银行流水剩余金额不足!");
        //            }
        //            else
        //            {
        //                remainPrice = Math.Abs(input.Price);
        //            }
        //        }


        //        //if (remainPrice < total)
        //        //{
        //        //    throw new Exception("银行流水剩余金额不足!");
        //        //}

        //        #region 回写仓储费ID
        //        StorageCallback(statistics, orderIDs.ToArray(), reponsitory, input);
        //        #endregion

        //        #region 核销账单
        //        //DeductionVouchers(reponsitory, vouchers, input, rate: rate);
        //        decimal payPrice = 0;
        //        Subject subject;
        //        string flowId_Temp;
        //        string payer_Actual = input.Payer;

        //        //匿名
        //        if (IsAnonymous(input.Payer) && input.Payer != input.PayerNew)
        //        {
        //            payer_Actual = input.PayerNew;
        //        }

        //        decimal rate = 1;       //发生币种对人民币的汇率
        //        foreach (var voucher in vouchers)
        //        {
        //            //settleCurrency = receivables.FirstOrDefault(item => item.ID == voucher.ReceivableID).SettlementCurrency;
        //            rate = receivables.FirstOrDefault(item => item.ID == voucher.ReceivableID).Rate1;

        //            //根据应收（剩余应收）进行核销
        //            payPrice = remainPrice > voucher.Remains ? voucher.Remains : remainPrice;

        //            //核销科目金额有值，根据核销金额进行核销
        //            if (fees != null && fees.Any(item => item.LeftID == voucher.ReceivableID))
        //            {
        //                payPrice = fees.Single(item => item.LeftID == voucher.ReceivableID).RightPrice ?? 0;
        //            }

        //            if (payPrice <= 0)
        //            {
        //                continue;
        //            }

        //            flowId_Temp = PKeySigner.Pick(PKeyType.FlowAccount);

        //            //添加实收
        //            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
        //            {
        //                ID = PKeySigner.Pick(PKeyType.Receiveds),
        //                CreateDate = input.CreateDate,
        //                //WaybillID = voucher.WaybillID,
        //                AdminID = inputer.ID,
        //                OrderID = voucher.OrderID,
        //                ReceivableID = voucher.ReceivableID,
        //                AccountType = (int)input.AccountType,
        //                FlowID = flowId_Temp,

        //                Price = (payPrice / rate).Round(),

        //                Currency1 = (int)Currency.CNY,
        //                Rate1 = 1,
        //                Price1 = payPrice,
        //            });

        //            //核销流水
        //            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
        //            {
        //                ID = flowId_Temp,
        //                Type = (int)input.AccountType,
        //                AdminID = inputer.ID,
        //                Business = input.Business,
        //                CreateDate = input.CreateDate,
        //                OrderID = voucher.OrderID,
        //                Payee = input.Payee,
        //                Payer = payer_Actual,

        //                Price = -payPrice,
        //                Currency = (int)Currency.CNY,

        //                Currency1 = (int)Currency.CNY,
        //                ERate1 = 1,
        //                Price1 = -payPrice,

        //                Account = bankFlow?.Account,
        //                Bank = bankFlow?.Bank,
        //                FormCode = bankFlow?.FormCode,
        //            });

        //            //判断科目是否需要转给客户余额
        //            subject = subjectsView[SubjectType.Input, voucher.Business, voucher.Catalog, voucher.Subject];
        //            if (subject != null && subject.IsToCustomer)
        //            {
        //                var orderEventArg = new Services.Events.OrderEventArgs(voucher.OrderID);
        //                this.Fire(this, orderEventArg);

        //                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
        //                {
        //                    ID = PKeySigner.Pick(PKeyType.FlowAccount),
        //                    Type = (int)AccountType.Cash,
        //                    AdminID = inputer.ID,
        //                    Business = input.Business,
        //                    CreateDate = input.CreateDate,
        //                    OrderID = voucher.OrderID,
        //                    Payee = orderEventArg.ClientID,
        //                    Payer = input.Payee,

        //                    Price = payPrice,
        //                    Currency = (int)Currency.CNY,

        //                    Currency1 = (int)Currency.CNY,
        //                    ERate1 = 1,
        //                    Price1 = payPrice,

        //                    Account = bankFlow?.Account,
        //                    Bank = bankFlow?.Bank,
        //                    FormCode = bankFlow?.FormCode,
        //                });
        //            }

        //            remainPrice -= payPrice;
        //        }
        //        #endregion

        //        #region 更新匿名

        //        ModifyAnonymous(input.PayerNew, reponsitory);
        //        #endregion

        //        #region 统一提交
        //        reponsitory.Submit();
        //        #endregion

        //        #region 修改订单状态

        //        if (orderIDs.Any(item => item != null))
        //        {
        //            List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
        //            List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();

        //            foreach (var orderId in orderIDs)
        //            {
        //                if (!string.IsNullOrWhiteSpace(orderId) && !orderId.StartsWith("LsOrder"))
        //                {
        //                    whsEventArgs.Add(new WhsPayConfirmedEventArgs()
        //                    {
        //                        Source = SourceType.Confirm,
        //                        OrderID = orderId,
        //                        OperatorID = input.CreatorID,
        //                        ApplicationID = input.ApplicationID,
        //                        Status = statistics.Where(item => !item.OrderID.Contains("LsOrder")).Where(GetExpressionVoucherCnyStatistics(statistics, input, orderIDs: orderId)).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
        //                    });
        //                }
        //                else
        //                {
        //                    if (statistics.Where(item => item.OrderID.Contains("LsOrder")).Where(GetExpressionVoucherCnyStatistics(statistics, input, orderIDs: orderId)).ToList().Sum(item => item.Remains) == 0)
        //                    {
        //                        lsEventArgs.Add(new LsPayConfirmedEventArgs()
        //                        {
        //                            LsOrderID = orderId,
        //                            OperatorID = input.CreatorID,
        //                        });
        //                    }
        //                }
        //            }

        //            if (whsEventArgs.Count > 0)
        //            {
        //                this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
        //            }

        //            if (lsEventArgs.Count > 0)
        //            {
        //                this.Fire(input, new ConfirmedEventArgs<LsPayConfirmedEventArgs>(lsEventArgs.ToArray()));
        //            }
        //        }
        //        #endregion
        //    }
        //}
        #endregion

        #region 使用优惠券

        /// <summary>
        /// 使用优惠券
        /// </summary>
        internal void Coupon(Coupon coupon)
        {
            if (receivables == null || !receivables.Any())
            {
                throw new Exception("未找到应收信息!");
            }

            if (coupon == null || string.IsNullOrWhiteSpace(coupon.ID))
            {
                throw new Exception("未找到优惠券信息!");
            }

            if (receivables.Count() != 1)
            {
                throw new Exception("一次只能添加一个优惠券!");
            }

            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var vouchers = new VouchersStatisticsView())
            {
                var first = receivables.FirstOrDefault();
                var rate = ExchangeRates.Universal[coupon.Currency, Currency.CNY];
                decimal remains = vouchers.SingleOrDefault(item => item.ReceivableID == first.ID).Remains;      //未支付金额

                string flowId = PKeySigner.Pick(PKeyType.FlowAccount);
                //优惠券金额大于未支付金额时，选择未支付金额
                decimal price = coupon.Price < remains ? (coupon.Price ?? 0) : remains;

                //据实优惠券，赋值未支付的金额
                if (coupon.Type == CouponType.Fact)
                {
                    price = remains;
                }

                //添加流水
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                {
                    ID = flowId,
                    Type = (int)AccountType.Coupon,
                    AdminID = inputer.ID,
                    Business = first.Business,
                    CreateDate = DateTime.Now,
                    OrderID = first.OrderID,
                    Payee = first.Payee,
                    Payer = first.Payer,

                    Price = -price,
                    Currency = (int)coupon.Currency,

                    Currency1 = (int)Currency.CNY,
                    ERate1 = rate,
                    Price1 = -price * rate,
                });

                //添加实收
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                {
                    ID = PKeySigner.Pick(PKeyType.Receiveds),
                    CreateDate = DateTime.Now,
                    WaybillID = first.WaybillID,
                    Price = price,
                    AdminID = inputer.ID,
                    OrderID = first.OrderID,
                    ReceivableID = first.ID,
                    AccountType = (int)AccountType.Coupon,
                    FlowID = flowId,
                    CouponID = coupon.ID,

                    Currency1 = (int)Currency.CNY,
                    Rate1 = rate,
                    Price1 = price * rate,
                });

                //提交
                reponsitory.Submit();

                #region 调用外部事件
                List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();

                if (!string.IsNullOrWhiteSpace(first.OrderID) && !first.OrderID.StartsWith("LsOrder"))
                {
                    whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                    {
                        Source = SourceType.Coupon,
                        OrderID = first.OrderID,
                        OperatorID = inputer.ID,
                        Status = vouchers.Where(item => item.OrderID == first.OrderID
                                                    && !item.OrderID.Contains("LsOrder")
                                                    && item.Payer == first.Payer
                                                    && item.Payee == first.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                    });
                }
                else
                {
                    if (vouchers.Where(item => item.OrderID == first.OrderID
                                           && item.OrderID.Contains("LsOrder")
                                           && item.Payer == first.Payer
                                           && item.Payee == first.Payee).ToList().Sum(item => item.Remains) == 0)
                    {
                        lsEventArgs.Add(new LsPayConfirmedEventArgs()
                        {
                            LsOrderID = first.OrderID,
                            OperatorID = inputer.ID,
                        });
                    }
                }

                if (whsEventArgs.Count > 0)
                {
                    this.Fire(first, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                }

                if (lsEventArgs.Count > 0)
                {
                    this.Fire(first, new ConfirmedEventArgs<LsPayConfirmedEventArgs>(lsEventArgs.ToArray()));
                }
                #endregion
            }
        }
        #endregion

        #region 余额支付
        /// <summary>
        /// 余额支付
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="price"></param>
        [Obsolete("暂时不支持")]
        public void Pay()
        {
            if (receivables == null || !receivables.Any())
            {
                throw new Exception("未找到应收!");
            }

            var first = receivables.FirstOrDefault();
            decimal balance = PaymentManager.Npc[first.Payer, first.Payee].Digital[first.SettlementCurrency].Available;
            var rate = ExchangeRates.Universal[first.SettlementCurrency, Currency.CNY];

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var view = new VouchersStatisticsView(reponsitory))
            {
                /*
                 * 遍历循环应收进行 记录流水账、实收
                 * 如果金额不足，允许支付部分
                 * 减去流水、添加实收
                 */

                //应收关联应收实收统计视图
                var rbView = view.Where(item => receivables.Select(a => a.ID).Contains(item.ReceivableID) && item.Currency == first.SettlementCurrency);

                if (!rbView.Any())
                {
                    throw new Exception("未找到对应的剩余应收!");
                }

                decimal total = rbView.ToList().Sum(item => item.Remains);

                if (balance < total)
                {
                    throw new Exception("余额不足!");
                }

                //记录流水表
                string flowId = RecordFlowAccount(reponsitory, rbView, first.SettlementCurrency, rate, total, new BankFlow("", "", ""));
                RecordReceived(reponsitory, rbView, flowId, null, null, first.SettlementCurrency, rate, total);

                #region 调用外部事件 更新订单状态
                var orderIDs = this.receivables.Where(item => item.OrderID != null).GroupBy(item => item.OrderID).OrderBy(item => item.Key).Select(item => item.Key);

                List<WhsPayConfirmedEventArgs> whsEventArgs = (from id in orderIDs
                                                               where !string.IsNullOrWhiteSpace(id)
                                                               select new WhsPayConfirmedEventArgs()
                                                               {
                                                                   Source = SourceType.ConfirmCollecting,
                                                                   OrderID = id,
                                                                   OperatorID = inputer.ID,
                                                                   Status = view.Where(item => id == item.OrderID).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                                                               }).ToList();
                if (whsEventArgs.Count > 0)
                {
                    this.Fire(null, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                }
                #endregion
            }
        }
        #endregion

        #region 废弃
        /// <summary>
        /// 废弃账单
        /// </summary>
        /// <param name="orderID">订单ID</param>
        public void Abolish(string orderID)
        {
            using (var repository = new PvbCrmReponsitory(false))
            using (var view = new VouchersStatisticsView(repository))
            using (var receivedView = new ReceivedsView(repository))
            {
                var vouchers = view.Where(item => item.OrderID == orderID).ToArray();

                //产生实收
                if (vouchers.Any(item => item.RightPrice > 0))
                {
                    var receivableIds = vouchers.Select(item => item.ReceivableID).ToArray();
                    var receiveds = receivedView.Where(item => receivableIds.Contains(item.ReceivableID));

                    foreach (var received in receiveds)
                    {
                        //删除流水表
                        repository.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => item.ID == received.FlowID);
                        //删除实收表
                        repository.Delete<Layers.Data.Sqls.PvbCrm.Receiveds>(item => item.ID == received.ID);
                    }
                }

                //更新应收状态为废弃
                repository.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                {
                    Status = (int)GeneralStatus.Closed
                }, item => item.OrderID == orderID);

                //提交
                repository.Submit();
            }
        }

        /// <summary>
        /// 根据申请ID删除核销、应收
        /// </summary>
        /// <param name="applicationID">申请ID</param>
        public void AbolishByApplicationID(string applicationID)
        {
            using (var repository = new PvbCrmReponsitory(false))
            using (var view = new VouchersStatisticsView(repository))
            using (var receivedView = new ReceivedsView(repository))
            {
                var vouchers = view.Where(item => item.ApplicationID == applicationID).ToArray();

                //产生实收
                if (vouchers.Any(item => item.RightPrice > 0))
                {
                    var receivableIds = vouchers.Select(item => item.ReceivableID).ToArray();
                    var receiveds = receivedView.Where(item => receivableIds.Contains(item.ReceivableID));

                    foreach (var received in receiveds)
                    {
                        //删除流水表
                        repository.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => item.ID == received.FlowID);
                        //删除实收表
                        repository.Delete<Layers.Data.Sqls.PvbCrm.Receiveds>(item => item.ID == received.ID);
                    }
                }

                //更新应收状态为废弃
                repository.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                {
                    Status = (int)GeneralStatus.Closed
                }, item => item.ApplicationID == applicationID);

                //提交
                repository.Submit();
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除应收
        /// </summary>
        public void Delete()
        {
            if (receivables == null || !receivables.Any())
            {
                throw new ArgumentException($"未找到应收ID");
            }

            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var receivedView = new ReceivedsView(reponsitory))
            using (var flowsView = new Yahv.Payments.Views.FlowAccountsTopView(reponsitory))
            {
                //遍历循环应收列表
                foreach (var receivable in receivables)
                {
                    //产生实收
                    if (receivedView.Any(item => item.ReceivableID == receivable.ID))
                    {
                        //获取实收里边的流水核销ID
                        var flowIds =
                            receivedView.Where(item => item.ReceivableID == receivable.ID)
                                .Select(item => item.FlowID)
                                .ToArray();

                        //删除流水表
                        reponsitory.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => flowIds.Contains(item.ID));
                        //删除实收表
                        reponsitory.Delete<Layers.Data.Sqls.PvbCrm.Receiveds>(item => item.ReceivableID == receivable.ID);



                        //有现金记账
                        if (receivedView.Any(
                                item => item.ReceivableID == receivable.ID && item.AccountType == AccountType.Cash))
                        {
                            //获取现金记账
                            var flowId = flowsView.FirstOrDefault(item =>
                                    item.OrderID == receivable.OrderID
                                    && item.Type == AccountType.Cash
                                    && item.Payer == receivable.Payer
                                    && item.Payee == receivable.Payee
                                    && item.Business == receivable.Business
                                    && item.Price == receivable.Price)?.ID;

                            //删除现金流水
                            reponsitory.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => item.ID == flowId);
                        }
                    }


                    //更新应收状态为废弃
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                    {
                        Status = (int)GeneralStatus.Closed
                    }, item => item.ID == receivable.ID);
                }

                reponsitory.Submit();
            }


        }
        #endregion
        #endregion

        #region 华芯通

        #region 华芯通财务收款确认[非货款]

        /// <summary>
        /// 华芯通财务收款确认
        /// </summary>
        /// <param name="input"></param>
        public void Confirm_XinDaTong(VoucherInput input, XdtFee[] array)
        {
            IEnumerable<string> tinyIDs = null;
            IEnumerable<string> orderIDs = null;
            try
            {
                using (var reponsitory = new PvbCrmReponsitory())
                using (var statistics = new VouchersStatisticsView(reponsitory))
                using (var bankFlows = new BankFlowAccountsView(reponsitory))
                using (var subjectsView = new SubjectsView(reponsitory))
                {
                    input = ChangeNameToId(input);

                    tinyIDs = this.receivables.Where(item => item.TinyID != null).GroupBy(item => item.TinyID).OrderBy(item => item.Key).Select(item => item.Key);
                    orderIDs = this.receivables.Where(item => item.OrderID != null).GroupBy(item => item.OrderID).OrderBy(item => item.Key).Select(item => item.Key);
                    var vouchers = statistics.OrderBy(item => item.TinyID).Where(item => tinyIDs.Contains(item.TinyID) && item.Payer == input.Payer && item.Payee == input.Payee && item.Currency == input.Currency);
                    var rate = ExchangeRates.Universal[input.Currency, Currency.CNY];

                    var bankFlow = bankFlows[input.FormCode];       //银行收款流水
                    input.AccountType = AccountType.BankStatement;      //默认银行流水

                    //检查余额是否充足
                    if (bankFlow.Price <= 0 || input.Price > bankFlow.Price)
                    {
                        throw new Exception($"流水号：{input.FormCode}，余额不足!");
                    }

                    //如果核销金额为0，默认为银行流水金额总额来进行核销
                    if (input.Price == 0)
                    {
                        input.Price = bankFlow.Price;
                    }

                    //核销的实收数组，总额不能超过银行流水总额
                    if (array?.Length > 0)
                    {
                        if (bankFlow.Price < array.Sum(item => item.Price))
                        {
                            throw new Exception($"银行剩余金额不足!");
                        }
                    }

                    List<Layers.Data.Sqls.PvbCrm.Receiveds> listRec;
                    List<Layers.Data.Sqls.PvbCrm.FlowAccounts> listFlow;

                    //核销账单
                    DeductionVouchers_Xdt(reponsitory, vouchers, input, bankFlow, subjectsView, array, out listRec, out listFlow, rate: rate);

                    //提交
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvbCrm.Receiveds), ToDataTable(listRec));
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvbCrm.FlowAccounts), ToDataTable(listFlow));

                    #region 调用外部事件 更新订单状态
                    List<WhsPayConfirmedEventArgs> whsEventArgs = (from id in orderIDs
                                                                   where !string.IsNullOrWhiteSpace(id)
                                                                   select new WhsPayConfirmedEventArgs()
                                                                   {
                                                                       Source = SourceType.ConfirmCollecting,
                                                                       OrderID = id,
                                                                       OperatorID = input.CreatorID,
                                                                       ApplicationID = input.ApplicationID,
                                                                       Status = statistics.Where(item => id == item.OrderID && item.Payer == input.Payer && item.Payee == input.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                                                                   }).ToList();

                    if (whsEventArgs.Count > 0)
                    {
                        this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                    }
                    #endregion

                    Oplogs.Oplog(this.inputer.ID, typeof(Receiveds).FullName, "Pays", $"华芯通财务收款确认[非货款]", $"Confirm_XinDaTong(OrderID:{string.Join(",", orderIDs)},TinyID:{string.Join(",", tinyIDs)},array:{array?.Length})", "");
                }
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.inputer.ID, typeof(Receiveds).FullName, ex, remark: $"Confirm_XinDaTong(OrderID:{string.Join(",", orderIDs)},TinyID:{string.Join(",", tinyIDs)},array:{array?.Length})");
                throw ex;
            }
        }

        /// <summary>
        /// 华芯通财务收款确认
        /// </summary>
        /// <param name="input"></param>
        //public void _bak_Confirm_XinDaTong(VoucherInput input)
        //{
        //    using (var reponsitory = new PvbCrmReponsitory())
        //    using (var statistics = new VouchersStatisticsView(reponsitory))
        //    using (var balanceView = new Yahv.Payments.Views.FlowAccountsTopView(reponsitory))
        //    {
        //        input = ChangeNameToId(input);

        //        var tinyIDs = this.receivables.Where(item => item.TinyID != null).GroupBy(item => item.TinyID).OrderBy(item => item.Key).Select(item => item.Key);
        //        var orderIDs = this.receivables.Where(item => item.OrderID != null).GroupBy(item => item.OrderID).OrderBy(item => item.Key).Select(item => item.Key);
        //        var vouchers = statistics.OrderBy(item => item.TinyID).Where(item => tinyIDs.Contains(item.TinyID) && item.Payer == input.Payer && item.Payee == input.Payee && item.Currency == input.Currency);
        //        var rate = ExchangeRates.Floating[input.Currency, Currency.CNY];

        //        decimal remainPrice = input.Price;      //剩余金额
        //        decimal payPrice;
        //        var balanceFlow = balanceView[input.Currency, input.FormCode];
        //        string orderId = orderIDs.First();

        //        //没有余额
        //        if (balanceFlow.Price <= 0)
        //        {
        //            #region 添加流水
        //            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
        //            {
        //                ID = PKeySigner.Pick(PKeyType.FlowAccount),
        //                Type = (int)AccountType.Cash,
        //                AdminID = inputer.ID,
        //                Business = input.Business,
        //                CreateDate = DateTime.Now,
        //                OrderID = orderId,
        //                Payee = input.Payee,
        //                Payer = input.Payer,

        //                Price = input.Price,
        //                Currency = (int)input.Currency,

        //                Currency1 = (int)Currency.CNY,
        //                ERate1 = rate,
        //                Price1 = input.Price * rate,

        //                Account = input.Account,
        //                Bank = input.Bank,
        //                FormCode = input.FormCode,
        //            });
        //            #endregion

        //            #region 核销账单
        //            remainPrice = DeductionVouchers(reponsitory, vouchers, input);
        //            #endregion
        //        }
        //        else
        //        {
        //            #region 核销账单
        //            remainPrice = DeductionVouchers(reponsitory, vouchers, input, balance: balanceFlow.Price);

        //            //余额不够支付，通过录入金额扣除
        //            if (remainPrice > 0)
        //            {
        //                //核销
        //                payPrice = remainPrice < input.Price ? remainPrice : input.Price;


        //                //添加流水
        //                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
        //                {
        //                    ID = PKeySigner.Pick(PKeyType.FlowAccount),
        //                    Type = (int)AccountType.Cash,
        //                    AdminID = inputer.ID,
        //                    Business = input.Business,
        //                    CreateDate = DateTime.Now,
        //                    OrderID = orderId,
        //                    Payee = input.Payee,
        //                    Payer = input.Payer,

        //                    Price = payPrice,
        //                    Currency = (int)input.Currency,

        //                    Currency1 = (int)Currency.CNY,
        //                    ERate1 = rate,
        //                    Price1 = payPrice * rate,

        //                    Account = input.Account,
        //                    Bank = input.Bank,
        //                    FormCode = input.FormCode,
        //                });

        //                remainPrice = DeductionVouchers(reponsitory, vouchers, new VoucherInput()
        //                {
        //                    CreateDate = input.CreateDate,
        //                    Account = input.Account,
        //                    ApplicationID = input.ApplicationID,
        //                    Bank = input.Bank,
        //                    Business = input.Business,
        //                    CreatorID = input.CreatorID,
        //                    FormCode = input.FormCode,
        //                    Payee = input.Payee,
        //                    Payer = input.Payer,
        //                    Type = input.Type,
        //                    VoucherID = input.VoucherID,

        //                    Currency = input.Currency,
        //                    Price = payPrice
        //                });
        //            }
        //            #endregion
        //        }

        //        #region 调用外部事件 更新订单状态
        //        List<WhsPayConfirmedEventArgs> whsEventArgs = (from id in orderIDs
        //                                                       where !string.IsNullOrWhiteSpace(id)
        //                                                       select new WhsPayConfirmedEventArgs()
        //                                                       {
        //                                                           Source = SourceType.ConfirmCollecting,
        //                                                           OrderID = id,
        //                                                           OperatorID = input.CreatorID,
        //                                                           ApplicationID = input.ApplicationID,
        //                                                           Status = statistics.Where(item => id == item.OrderID && item.Payer == input.Payer && item.Payee == input.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
        //                                                       }).ToList();

        //        if (whsEventArgs.Count > 0)
        //        {
        //            this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
        //        }
        //        #endregion
        //    }
        //}
        #endregion

        #region 华芯通付汇财务收款确认[货款]
        public void Confirm_Remit_XinDaTong(VoucherInput input, XdtFee[] array)
        {
            IEnumerable<string> tinyIDs = null;
            IEnumerable<string> orderIDs = null;
            try
            {
                using (var reponsitory = new PvbCrmReponsitory())
                using (var statistics = new VouchersStatisticsView(reponsitory))
                using (var bankFlows = new BankFlowAccountsView(reponsitory))
                using (var subjectsView = new SubjectsView(reponsitory))
                {
                    if (this.receivables == null || !this.receivables.Any())
                    {
                        throw new Exception("未找到账单信息!");
                    }

                    input = ChangeNameToId(input);

                    tinyIDs = this.receivables.Where(item => item.TinyID != null).GroupBy(item => item.TinyID).OrderBy(item => item.Key).Select(item => item.Key);
                    orderIDs = this.receivables.Where(item => item.OrderID != null).GroupBy(item => item.OrderID).OrderBy(item => item.Key).Select(item => item.Key);
                    var vouchers = statistics.OrderBy(item => item.TinyID).Where(item => tinyIDs.Contains(item.TinyID) && item.Payer == input.Payer && item.Payee == input.Payee && item.Currency == input.Currency);
                    var rate = ExchangeRates.Universal[input.Currency, Currency.CNY];

                    var bankFlow = bankFlows[input.FormCode];       //银行收款流水
                    decimal remainPrice = input.Price;      //核销金额
                    input.AccountType = AccountType.BankStatement;      //默认银行流水

                    //检查余额是否充足
                    if (bankFlow.Price <= 0 || input.Price > bankFlow.Price)
                    {
                        throw new Exception($"流水号：{input.FormCode}，余额不足!");
                    }

                    //如果核销金额为0，默认为银行流水金额总额来进行核销
                    if (input.Price == 0)
                    {
                        input.Price = bankFlow.Price;
                    }

                    //if (bankFlow.Business != input.Business)
                    //{
                    //    throw new Exception($"银行流水业务和核销账单业务不同!");
                    //}

                    //核销的实收数组，总额不能超过银行流水总额
                    if (array?.Length > 0)
                    {
                        if (bankFlow.Price < array.Sum(item => item.Price))
                        {
                            throw new Exception($"银行剩余金额不足!");
                        }
                    }

                    List<Layers.Data.Sqls.PvbCrm.Receiveds> listRec;
                    List<Layers.Data.Sqls.PvbCrm.FlowAccounts> listFlow;

                    //核销账单
                    DeductionVouchers_Xdt(reponsitory, vouchers, input, bankFlow, subjectsView, array, out listRec, out listFlow, rate: rate);

                    //提交
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvbCrm.Receiveds), ToDataTable(listRec));
                    reponsitory.SqlBulkCopyByDatatable(nameof(Layers.Data.Sqls.PvbCrm.FlowAccounts), ToDataTable(listFlow));

                    #region 调用外部事件 更新订单状态
                    List<WhsPayConfirmedEventArgs> whsEventArgs = (from id in orderIDs
                                                                   where !string.IsNullOrWhiteSpace(id)
                                                                   select new WhsPayConfirmedEventArgs()
                                                                   {
                                                                       Source = SourceType.ConfirmCollecting,
                                                                       OrderID = id,
                                                                       OperatorID = input.CreatorID,
                                                                       ApplicationID = input.ApplicationID,
                                                                       Status = statistics.Where(item => id == item.OrderID && item.Payer == input.Payer && item.Payee == input.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                                                                   }).ToList();

                    if (whsEventArgs.Count > 0)
                    {
                        this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                    }
                    #endregion

                    Oplogs.Oplog(this.inputer.ID, typeof(Receiveds).FullName, "Pays", $"华芯通付汇财务收款确认[货款]", $"Confirm_XinDaTong(OrderID:{string.Join(",", orderIDs)},TinyID:{string.Join(",", tinyIDs)},array:{array?.Length})", "");
                }
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.inputer.ID, typeof(Receiveds).FullName, ex, remark: $"Confirm_XinDaTong(OrderID:{string.Join(",", orderIDs)},TinyID:{string.Join(",", tinyIDs)},array:{array?.Length})");
                throw;
            }
        }
        #endregion

        #region 取消实收
        /// <summary>
        /// 取消实收
        /// </summary>
        /// <param name="formCode">银行流水</param>
        public void Abandon(string formCode)
        {
            try
            {
                formCode = formCode.Trim();

                if (string.IsNullOrWhiteSpace(formCode))
                {
                    return;
                }

                using (var reponsitory = new PvbCrmReponsitory(false))
                using (var flowView = new Yahv.Payments.Views.FlowAccountsTopView(reponsitory))
                {
                    //根据流水号获取流水ID
                    string[] flowIds = flowView.Where(item => item.FormCode == formCode && item.Price < 0).Select(item => item.ID).ToArray();

                    //清除流水表
                    //todo 优惠券或者减免 如何处理？
                    reponsitory.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => flowIds.Contains(item.ID));

                    //根据流水ID清空实收
                    reponsitory.Delete<Layers.Data.Sqls.PvbCrm.Receiveds>(item => flowIds.Contains(item.FlowID));

                    //根据流水号清空通知单明细
                    //reponsitory.Delete<Layers.Data.Sqls.PvbCrm.VoucherRecords>(item => item.FormCode == formCode);

                    reponsitory.Submit();

                    Oplogs.Oplog(this.inputer.ID, typeof(Receiveds).FullName, "Pays", $"华芯通取消实收", $"Abandon(formCode:{formCode}", "");
                }
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.inputer.ID, typeof(Receiveds).FullName, ex, remark: $"Abandon(formCode:{formCode}");
                throw;
            }
        }

        /// <summary>
        /// 取消实收
        /// </summary>
        /// <param name="tinyID">小订单ID</param>
        /// <param name="catalog">分类</param>
        /// <param name="subject">科目</param>
        /// <param name="currency">币种</param>
        public void Abandon(string tinyID, string catalog, string subject, Currency currency = Currency.CNY)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tinyID))
                {
                    return;
                }

                using (var reponsitory = new PvbCrmReponsitory(false))
                using (var receivedsView = new Yahv.Payments.Views.ReceivedsStatisticsView(reponsitory))
                {
                    var ids = new ReceivedDto[] { };

                    if (!string.IsNullOrWhiteSpace(subject))
                    {
                        ids =
                            receivedsView.Where(
                                item =>
                                    item.TinyID == tinyID && item.Catalog == catalog && item.Subject == subject && item.SettlementCurrency == currency
                                   ).Select(t => new ReceivedDto { ReceivedID = t.ReceivedID, FlowID = t.FlowID }).ToArray();
                    }
                    else
                    {
                        ids =
                           receivedsView.Where(
                               item =>
                                   item.TinyID == tinyID && item.Catalog == catalog && item.SettlementCurrency == currency
                                  ).Select(t => new ReceivedDto { ReceivedID = t.ReceivedID, FlowID = t.FlowID }).ToArray();
                    }

                    if (ids.Length <= 0)
                    {
                        return;
                    }

                    //清除流水表
                    reponsitory.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => ids.Select(i => i.FlowID).Contains(item.ID));
                    //根据流水ID清空实收
                    reponsitory.Delete<Layers.Data.Sqls.PvbCrm.Receiveds>(item => ids.Select(i => i.ReceivedID).Contains(item.ID));
                    reponsitory.Submit();

                    Oplogs.Oplog(this.inputer.ID, typeof(Receiveds).FullName, "Pays", $"华芯通取消实收", $"Abandon(tinyID:{tinyID},catalog:{catalog},subject:{subject},currency:{currency})", "");
                }
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.inputer.ID, typeof(Receiveds).FullName, ex, remark: $"Abandon(tinyID:{tinyID},catalog:{catalog},subject:{subject},currency:{currency})");
                throw;
            }
        }

        class ReceivedDto
        {
            public string ReceivedID { get; set; }
            public string FlowID { get; set; }
        }
        #endregion

        #endregion

        #region 私有函数
        /// <summary>
        /// 记录流水
        /// </summary>
        private string RecordFlowAccount(PvbCrmReponsitory reponsitory, IQueryable<VoucherStatistic> rbView,
            Currency currency, decimal rate, decimal price, BankFlow bankFlow)
        {
            string flowId = PKeySigner.Pick(PKeyType.FlowAccount);

            //添加流水金额                
            var rbFirst = rbView.FirstOrDefault();

            //如果有银行账号说明是从财务收款确认进来的，不需要添加流水了
            if (bankFlow == null)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                {
                    ID = PKeySigner.Pick(PKeyType.FlowAccount),
                    Type = (int)AccountType.Cash,
                    AdminID = inputer.ID,
                    Business = rbFirst.Business,
                    CreateDate = DateTime.Now,
                    OrderID = rbFirst.OrderID,
                    Payee = rbFirst.Payee,
                    Payer = rbFirst.Payer,

                    Price = price,
                    Currency = (int)currency,

                    Currency1 = (int)Currency.CNY,
                    ERate1 = rate,
                    Price1 = price * rate,

                    Account = bankFlow?.Account,
                    Bank = bankFlow?.Bank,
                    FormCode = bankFlow?.FormCode,
                    DateIndex = rbFirst.ChangeIndex,
                });
            }


            //添加实际减去流水
            //如果核销金额大于应收，按照应收添加流水；如果核销金额小于应收，按照核销金额添加流水
            decimal priceFlow = rbView.ToList().Sum(item => item.Remains);
            priceFlow = priceFlow <= price ? priceFlow : price;
            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
            {
                ID = flowId,
                Type = (int)AccountType.Cash,
                AdminID = inputer.ID,
                Business = rbFirst.Business,
                CreateDate = DateTime.Now,
                OrderID = rbFirst.OrderID,
                Payee = rbFirst.Payee,
                Payer = rbFirst.Payer,

                Price = priceFlow * -1,
                Currency = (int)currency,

                Currency1 = (int)Currency.CNY,
                ERate1 = rate,
                Price1 = priceFlow * rate * -1,

                Account = bankFlow?.Account,
                Bank = bankFlow?.Bank,
                FormCode = bankFlow?.FormCode,
                DateIndex = rbFirst.ChangeIndex,
            });

            return flowId;
        }

        /// <summary>
        /// 记录实收
        /// </summary>
        /// <returns></returns>
        private string RecordReceived(PvbCrmReponsitory reponsitory, IQueryable<VoucherStatistic> rbView, string flowId, string id, string summay,
            Currency currency, decimal rate, decimal price)
        {
            string result = id;
            decimal payPrice = 0;               //支付金额
            decimal remainPrice = price;        //剩余金额

            if (string.IsNullOrEmpty(id))
            {
                result = id = PKeySigner.Pick(PKeyType.Receiveds);
            }
            //遍历应收，记录实收
            foreach (var rb in rbView)
            {
                if (remainPrice <= 0)
                {
                    break;
                }

                if (rb.Remains == 0)
                {
                    continue;
                }

                //判断可支付金额是否足够
                payPrice = remainPrice > rb.Remains ? rb.Remains : remainPrice;

                //实收
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                {
                    ID = id,
                    CreateDate = DateTime.Now,
                    WaybillID = rb.WaybillID,
                    Price = payPrice,
                    AdminID = inputer.ID,
                    OrderID = rb.OrderID,
                    ReceivableID = rb.ReceivableID,
                    AccountType = (int)AccountType.Cash,
                    FlowID = flowId,

                    Currency1 = (int)Currency.CNY,
                    Rate1 = rate,
                    Price1 = payPrice * rate,

                    Summay = summay,
                });

                id = PKeySigner.Pick(PKeyType.Receiveds);
                //更新剩余金额
                remainPrice -= payPrice;
            }

            return result;
        }

        /// <summary>
        /// 回写仓储费 
        /// </summary>
        /// <remarks>更新OrderID</remarks>
        /// <param name="vouchers">仓储费的账单视图</param>  
        private void StorageCallback(VouchersStatisticsView vouchers, Currency currency, string orderId, PvbCrmReponsitory reponsitory, Receivable receivable)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return;
            }

            var query = vouchers.Where(item => item.OrderID == null
                    && item.Payer == receivable.Payer
                    && item.Payee == receivable.Payee
                    && item.Business == receivable.Business
                    && item.Currency == currency
                    && item.Subject == "仓储费");

            if (!query.Any())
            {
                return;
            }

            foreach (var voucher in query)
            {
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                {
                    OrderID = orderId,
                }, item => item.ID == voucher.ReceivableID && item.OrderID == null);


                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receiveds>(new
                {
                    OrderID = orderId,
                }, item => item.ReceivableID == voucher.ReceivableID && item.OrderID == null);

                reponsitory.Update<Layers.Data.Sqls.PvbCrm.FlowAccounts>(new
                {
                    OrderID = orderId,
                }, item => item.Type == (int)AccountType.Reduction && item.OrderID == null
                && item.Payer == receivable.Payer && item.Payee == receivable.Payee
                && item.Business == receivable.Business && item.Subject == "仓储费");
            }
        }

        private void StorageCallback(VouchersStatisticsView vouchers, string[] orderIds, PvbCrmReponsitory reponsitory, VoucherInput input)
        {
            foreach (var orderID in orderIds)
            {
                if (string.IsNullOrWhiteSpace(orderID))
                {
                    continue;
                }

                Vouchers voucher = null;

                if (string.IsNullOrWhiteSpace(input.ApplicationID))
                {
                    voucher = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                      .SingleOrDefault(item => item.OrderID == orderID
                      && item.Type == (int)input.Type && item.ApplicationID == null);
                }
                else
                {
                    voucher = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                     .SingleOrDefault(item => item.OrderID == orderID && item.ApplicationID == input.ApplicationID
                     && item.Type == (int)input.Type);
                }

                if (voucher?.IsSettlement ?? false)
                {
                    var query = vouchers.Where(item => item.OrderID == null
                                                       && item.Payer == input.Payer
                                                       && item.Payee == input.Payee
                                                       && item.Business == input.Business
                                                       && item.Currency == input.Currency);

                    if (query.Any())
                    {
                        foreach (var q in query)
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                            {
                                OrderID = orderID,
                            }, item => item.ID == q.ReceivableID && item.OrderID == null);


                            //reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receiveds>(new
                            //{
                            //    OrderID = orderID,
                            //}, item => item.ReceivableID == q.ReceivableID && item.OrderID == null);

                            //reponsitory.Update<Layers.Data.Sqls.PvbCrm.FlowAccounts>(new
                            //{
                            //    OrderID = orderID,
                            //}, item => item.Type == (int)AccountType.Reduction && item.OrderID == null
                            //           && item.Payer == input.Payer && item.Payee == input.Payee
                            //           //&& item.Subject == voucher.ExtraSubject
                            //           && item.Business == input.Business);
                        }
                    }
                }
            }
        }

        private void StorageCallback(Views.VouchersCnyStatisticsView vouchers, string[] orderIds, PvbCrmReponsitory reponsitory, VoucherInput input)
        {
            foreach (var orderID in orderIds)
            {
                if (string.IsNullOrWhiteSpace(orderID))
                {
                    continue;
                }

                Vouchers voucher = null;

                if (string.IsNullOrWhiteSpace(input.ApplicationID))
                {
                    voucher = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                      .SingleOrDefault(item => item.OrderID == orderID
                      && item.Type == (int)input.Type && item.ApplicationID == null);
                }
                else
                {
                    voucher = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                     .SingleOrDefault(item => item.OrderID == orderID && item.ApplicationID == input.ApplicationID
                     && item.Type == (int)input.Type);
                }

                if (voucher?.IsSettlement ?? false)
                {
                    var query = vouchers.Where(item => item.OrderID == null
                                                       && item.Payer.ID == input.Payer
                                                       && item.Payee.ID == input.Payee
                                                       && item.Business == input.Business
                                                       && item.Currency == input.Currency);

                    if (query.Any())
                    {
                        foreach (var q in query)
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                            {
                                OrderID = orderID,
                            }, item => item.ID == q.ReceivableID && item.OrderID == null);


                            //reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receiveds>(new
                            //{
                            //    OrderID = orderID,
                            //}, item => item.ReceivableID == q.ReceivableID && item.OrderID == null);

                            //reponsitory.Update<Layers.Data.Sqls.PvbCrm.FlowAccounts>(new
                            //{
                            //    OrderID = orderID,
                            //}, item => item.Type == (int)AccountType.Reduction && item.OrderID == null
                            //           && item.Payer == input.Payer && item.Payee == input.Payee
                            //           //&& item.Subject == voucher.ExtraSubject
                            //           && item.Business == input.Business);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将收款人付款人名称改为ID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private VoucherInput ChangeNameToId(VoucherInput input)
        {
            VoucherInput result = input;

            using (var view = new EnterprisesTopView())
            {
                result.Payer = view.SingleOrDefault(item => item.Name == input.Payer || item.ID == input.Payer)?.ID;
                result.Payee = view.SingleOrDefault(item => item.Name == input.Payee || item.ID == input.Payee)?.ID;
            }

            if (string.IsNullOrWhiteSpace(result.Payer) || string.IsNullOrWhiteSpace(result.Payee))
            {
                throw new ArgumentException("收款人或者付款人不存在!");
            }

            return result;
        }

        /// <summary>
        /// 核销账单金额
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="input"></param>
        private void DeductionVouchers_Xdt(PvbCrmReponsitory reponsitory, IQueryable<VoucherStatistic> statistics, VoucherInput input, BankFlowAccount bankFlow, SubjectsView subjectsView, XdtFee[] array, out List<Layers.Data.Sqls.PvbCrm.Receiveds> listReceivedses, out List<Layers.Data.Sqls.PvbCrm.FlowAccounts> listFlowAccountses, decimal rate = 1)
        {
            string business = ConductConsts.供应链;
            decimal remainPrice = input.Price;      //剩余金额
            decimal payPrice = 0;
            Subject subject;
            string flowId_Temp = string.Empty;
            listReceivedses = new List<Layers.Data.Sqls.PvbCrm.Receiveds>();
            listFlowAccountses = new List<FlowAccounts>();

            var voucherStats = statistics.ToList();
            VoucherStatistic voucher = null;
            VoucherStatistic[] vouchers;

            //根据传过来的实际金额进行核销
            if (array.Length > 0)
            {
                foreach (var xdtFee in array)
                {
                    vouchers =
                            voucherStats.Where(
                                item =>
                                    item.Currency == xdtFee.Currency && item.Catalog == xdtFee.Catalog &&
                                    item.Subject == xdtFee.Subject).OrderBy(item => item.Remains).ToArray();

                    if (vouchers.Length <= 0)
                    {
                        throw new Exception($"未找到对应的应收! Currency:[{xdtFee.Currency}]，Catalog:[{xdtFee.Catalog}]，Subject:[{xdtFee.Subject}]");
                    }

                    string[] receivedsIds = receivedsIds = PKeySigner.Series(PKeyType.Receiveds, vouchers.Length);      //应收批量生成ID
                    string[] flowIds = PKeySigner.Series(PKeyType.FlowAccount, vouchers.Length);       //流水表批量生成ID

                    remainPrice = Math.Abs(xdtFee.Price);
                    for (int i = 0; i < vouchers.Length; i++)
                    {
                        voucher = vouchers[i];

                        if (remainPrice <= 0)
                        {
                            break;
                        }

                        flowId_Temp = flowIds[i];
                        payPrice = remainPrice > voucher.Remains ? voucher.Remains : remainPrice;

                        if (i == vouchers.Length - 1)
                        {
                            if (remainPrice > vouchers[i].Remains)
                            {
                                payPrice = remainPrice;
                            }
                        }

                        //添加实收
                        listReceivedses.Add(new Layers.Data.Sqls.PvbCrm.Receiveds()
                        {
                            ID = receivedsIds[i],
                            CreateDate = input.CreateDate,
                            WaybillID = voucher.WaybillID,
                            Price = payPrice,
                            AdminID = inputer.ID,
                            OrderID = voucher.OrderID,
                            ReceivableID = voucher.ReceivableID,
                            AccountType = (int)input.AccountType,
                            FlowID = flowId_Temp,

                            Currency1 = (int)Currency.CNY,
                            Rate1 = rate,
                            Price1 = payPrice * rate,
                        });

                        //核销流水
                        listFlowAccountses.Add(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                        {
                            ID = flowId_Temp,
                            Type = (int)input.AccountType,
                            AdminID = inputer.ID,
                            //Business = input.Business,
                            Business = business,
                            CreateDate = input.CreateDate,
                            OrderID = voucher.OrderID,
                            Payee = input.Payee,
                            Payer = input.Payer,

                            Price = -payPrice,
                            Currency = (int)input.Currency,

                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = -payPrice * rate,

                            Account = bankFlow?.Account,
                            Bank = bankFlow?.Bank,
                            FormCode = bankFlow?.FormCode,
                        });

                        //剩余金额
                        remainPrice = remainPrice - payPrice;
                    }
                }
            }
        }

        private Expression<Func<VoucherStatistic, bool>> GetExpressionVoucherStatistics(VouchersStatisticsView vouchers, VoucherInput input, Currency currency = Currency.Unknown, string[] receivableIds = null, params string[] orderIDs)
        {
            Expression<Func<VoucherStatistic, bool>> predication = item => true;


            if (!string.IsNullOrWhiteSpace(input.Payer))
            {
                //predication = predication.And(item => item.Payer == input.Payer && item.Payee == input.Payee && (orderIDs.Contains(item.OrderID) || item.OrderID == null));

                predication = predication.And(item => item.Payee == input.Payee && (orderIDs.Contains(item.OrderID) || (item.OrderID == null && item.Payer == input.Payer)));
            }
            //匿名支付
            else
            {
                predication = predication.And(item => item.Payer == null && item.Payee == input.Payee && (orderIDs.Contains(item.OrderID)));
            }

            if (!string.IsNullOrWhiteSpace(input.ApplicationID))
            {
                predication = predication.And(item => item.ApplicationID == input.ApplicationID);
            }

            if (currency != Currency.Unknown)
            {
                predication = predication.And(item => item.Currency == currency);
            }

            //根据应收ID 筛选
            if (receivableIds?.Length > 0)
            {
                predication = predication.And(item => receivables.Select(r => r.ID).Contains(item.ReceivableID));
            }

            return predication;
        }

        private Expression<Func<VoucherCnyStatistic, bool>> GetExpressionVoucherCnyStatistics(Views.VouchersCnyStatisticsView vouchers, VoucherInput input, Currency currency = Currency.Unknown, string[] receivableIds = null, params string[] orderIDs)
        {
            Expression<Func<VoucherCnyStatistic, bool>> predication = item => true;


            if (!string.IsNullOrWhiteSpace(input.Payer))
            {
                predication = predication.And(item => item.Payee.ID == input.Payee && (orderIDs.Contains(item.OrderID) || (item.OrderID == null && item.Payer.ID == input.Payer)));
            }
            //匿名支付
            else
            {
                predication = predication.And(item => item.Payer == null && item.Payee.ID == input.Payee && (orderIDs.Contains(item.OrderID)));
            }

            if (!string.IsNullOrWhiteSpace(input.ApplicationID))
            {
                predication = predication.And(item => item.ApplicationID == input.ApplicationID);
            }

            if (currency != Currency.Unknown)
            {
                predication = predication.And(item => item.Currency == currency);
            }

            //根据应收ID 筛选
            if (receivableIds?.Length > 0)
            {
                predication = predication.And(item => receivables.Select(r => r.ID).Contains(item.ReceivableID));
            }

            return predication;
        }

        /// <summary>
        /// 更新匿名付款人
        /// </summary>
        /// <param name="payer"></param>
        /// <param name="reponsitory"></param>
        private void ModifyAnonymous(string payer, PvbCrmReponsitory reponsitory)
        {
            string anonymous = AnonymousEnterprise.Current.ID;

            //匿名付款人
            var items = this.receivables.Where(item => item.Payer == anonymous && item.Payer != payer);
            if (items != null && items.Any())
            {
                foreach (var receb in items)
                {
                    //更新应收表 匿名付款人
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                    {
                        Payer = payer
                    }, item => item.ID == receb.ID);

                    //更新Vouchers 付款人
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Vouchers>(new
                    {
                        Payer = payer
                    }, item => item.OrderID == receb.OrderID && item.Type == (int)VoucherType.Receipt && item.Payer == receb.Payer);
                }
            }
        }

        /// <summary>
        /// 是否匿名
        /// </summary>
        /// <param name="id">企业ID</param>
        /// <returns></returns>
        private bool IsAnonymous(string id)
        {
            return id == AnonymousEnterprise.Current.ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private static DataTable ToDataTable<T>(List<T> entities)
        {
            DataTable dt = new DataTable();
            var propeties = typeof(T).GetProperties().Where(item => !item.PropertyType.FullName.Contains("Layers.Data.Sqls")).ToArray();
            foreach (var prop in propeties)
            {
                var propType = prop.PropertyType;
                if ((propType.IsGenericType) && (propType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    propType = propType.GetGenericArguments()[0];
                }
                dt.Columns.Add(new DataColumn(prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name, propType));
            }

            foreach (var entity in entities)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < propeties.Count(); i++)
                {
                    PropertyInfo prop = propeties[i];
                    object value = prop.GetValue(entity);

                    dr[i] = value ?? DBNull.Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        #region bak
        ///// <summary>
        ///// 核销账单金额
        ///// </summary>
        ///// <param name="statistics"></param>
        ///// <param name="input"></param>
        //private decimal DeductionVouchers(PvbCrmReponsitory reponsitory, IQueryable<VoucherStatistic> statistics, VoucherInput input, decimal balance = 0, decimal rate = 1)
        //{
        //    decimal remainPrice = input.Price;      //剩余金额
        //    decimal payPrice = 0;
        //    string voucherId = string.Empty;

        //    //如果有余额，先用余额支付
        //    if (balance > 0)
        //    {
        //        remainPrice = balance;
        //    }

        //    dynamic obj;

        //    foreach (var voucher in statistics)
        //    {
        //        if (voucher.Remains <= 0 || string.IsNullOrWhiteSpace(voucher.OrderID))
        //        {
        //            continue;
        //        }

        //        payPrice = remainPrice > voucher.Remains ? voucher.Remains : remainPrice;

        //        if (payPrice <= 0)
        //        {
        //            continue;
        //        }

        //        //代付货款的应收，当做充值来处理，不对客户进行流水核销
        //        if (voucher.Business == ConductConsts.代仓储 && voucher.Catalog == CatalogConsts.货款 &&
        //            voucher.Subject == "代付货款")
        //        {
        //            //添加实收
        //            RecordReceived(reponsitory, statistics.Where(item => voucher.ReceivableID == item.ReceivableID && item.Currency == input.Currency), null, null, null, input.Currency, rate, payPrice);

        //            remainPrice -= payPrice;
        //            continue;
        //        }


        //        obj = GetVoucher(voucher, reponsitory);

        //        //添加实收
        //        this.For(voucher.ReceivableID)
        //            .Record(input.Currency, payPrice, bankFlow: new BankFlow(input.Bank, input.Account, input.FormCode));


        //        //添加VoucherRecords（根据每条对账单核销）
        //        //reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.VoucherRecords()
        //        //{
        //        //    CreateDate = DateTime.Now,
        //        //    CreatorID = input.CreatorID,
        //        //    Currency = (int)input.Currency,
        //        //    ID = PKeySigner.Pick(PKeyType.VoucherRecords),
        //        //    Price = payPrice,
        //        //    Bank = input.Bank,
        //        //    FormCode = input.FormCode,
        //        //    VoucherID = obj.ID,
        //        //    Account = input.Account,
        //        //});

        //        //判断是否为特殊科目，特殊科目，进行余额流转
        //        if (voucher.Business == ConductConsts.代仓储 && voucher.Catalog == CatalogConsts.货款 && voucher.Subject == "代收货款" && !string.IsNullOrWhiteSpace(obj.AgentID))
        //        {
        //            //将余额流转至客户
        //            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
        //            {
        //                ID = PKeySigner.Pick(PKeyType.FlowAccount),
        //                Type = (int)AccountType.Cash,
        //                AdminID = inputer.ID,
        //                Business = input.Business,
        //                CreateDate = DateTime.Now,
        //                OrderID = voucher.OrderID,
        //                Payee = obj.AgentID,
        //                Payer = input.Payee,

        //                Price = input.Price,
        //                Currency = (int)input.Currency,

        //                Currency1 = (int)Currency.CNY,
        //                ERate1 = rate,
        //                Price1 = input.Price * rate,

        //                Account = input.Account,
        //                Bank = input.Bank,
        //                FormCode = input.FormCode,
        //            });
        //        }

        //        remainPrice -= payPrice;
        //    }

        //    if (balance > 0)
        //    {
        //        remainPrice = input.Price - balance;
        //    }

        //    return remainPrice;
        //}
        #endregion
        #endregion
    }
}

