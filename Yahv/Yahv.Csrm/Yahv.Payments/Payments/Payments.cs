using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq.Extends;
using Yahv.Payments.Models;
using Yahv.Payments.Models.Origins;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Voucher = Yahv.Payments.Models.Origins.Voucher;

namespace Yahv.Payments
{
    /// <summary>
    /// 实付
    /// </summary>
    public class Payments : Services.ReceivedBase
    {
        private IEnumerable<Payable> payableses;
        private PayInfo payInfo;
        private IEnumerable<Fee> fees;      //核销科目金额

        #region 构造函数
        internal Payments(PayInfo payInfo)
        {
            this.payInfo = payInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array">应付ID</param>
        /// <returns></returns>
        public Payments For(params string[] array)
        {
            if (array.Length <= 0)
            {
                throw new Exception("参数不能为空!");
            }

            //实付记账
            if (array.Any(item => item.Contains("Paybl")))
            {
                using (var view = new PayablesView())
                {
                    this.payableses = view.Where(item => array.Contains(item.ID)).ToArray();

                    //if (payableses.GroupBy(item => item.OrderID).Count() > 1)
                    //{
                    //    throw new Exception("不能对不同订单进行记账!");
                    //}

                    var first = payableses.FirstOrDefault();
                    this.payInfo.Payer = first.Payer;
                    this.payInfo.Payee = first.Payee;
                    this.payInfo.Conduct = first.Business;
                    this.payInfo.OrderID = first.OrderID;
                }
            }
            //付款确认
            //else if (array.Any(item => item.Contains("Order")))
            else
            {
                this.payInfo.OrderID = array[0];
            }

            return this;
        }

        public Payments For(params Payable[] payables)
        {
            this.payableses = payables;
            return this;
        }

        public Payments For(params Fee[] _fees)
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
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="id">实付ID</param>
        /// <param name="summay">备注</param>
        /// <returns></returns>
        public string Record(Currency currency, decimal price, string id = null, string summay = null, BankFlow bankFlow = null)
        {
            if (payableses == null || !payableses.Any())
            {
                throw new Exception("未找到应付信息!");
            }

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            var rate = ExchangeRates.Universal[currency, Currency.CNY];

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var view = new PaymentsStatisticsView())
            {
                /*
                 * 遍历循环应收进行 记录流水账、实收
                 * 如果金额不足，允许支付部分
                 */

                //应收关联应收实收统计视图
                var rbView = view.Where(item => payableses.Select(a => a.ID).Contains(item.PayableID) && item.Currency == currency);

                if (!rbView.Any())
                {
                    throw new Exception("未找到对应的剩余应收!");
                }


                //记录流水表
                string flowId = RecordFlowAccount(reponsitory, rbView, currency, rate, price, bankFlow);
                id = RecordPayment(reponsitory, rbView, flowId, id, summay, currency, rate, price);
            }

            return id;
        }

        /// <summary>
        /// 根据申请ID删除核销、应付
        /// </summary>
        /// <param name="applicationID">申请ID</param>
        public void AbolishByApplicationID(string applicationID)
        {
            using (var repository = new PvbCrmReponsitory(false))
            using (var view = new PaymentsStatisticsView(repository))
            using (var paymtView = new PaymentsView(repository))
            {
                var paymentsView = view.Where(item => item.ApplicationID == applicationID).ToArray();

                //已经核销
                if (paymentsView.Any(item => item.RightPrice > 0))
                {
                    var payableIds = paymentsView.Select(item => item.PayableID).ToArray();
                    var payments = paymtView.Where(item => payableIds.Contains(item.PayableID));

                    foreach (var pay in payments)
                    {
                        //删除流水表
                        repository.Delete<Layers.Data.Sqls.PvbCrm.FlowAccounts>(item => item.ID == pay.FlowID);
                        //删除实付核销
                        repository.Delete<Layers.Data.Sqls.PvbCrm.Payments>(item => item.ID == pay.ID);
                    }
                }

                //更新应收状态为废弃
                repository.Update<Layers.Data.Sqls.PvbCrm.Payables>(new
                {
                    Status = (int)GeneralStatus.Closed
                }, item => item.ApplicationID == applicationID);

                //提交
                repository.Submit();
            }
        }
        #endregion

        #region 减免账户
        /// <summary>
        /// 减免
        /// </summary>
        public void Reduction(Currency currency, decimal price)
        {
            if (payableses == null || !payableses.Any())
            {
                throw new Exception("未找到应付信息!");
            }

            if (payableses.Count() != 1)
            {
                throw new Exception("一次只能添加一次减免!");
            }

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var first = payableses.FirstOrDefault();
                var rate = ExchangeRates.Universal[currency, Currency.CNY];

                string flowId = PKeySigner.Pick(PKeyType.FlowAccount);
                decimal oper = -1;

                //添加流水
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                {
                    ID = flowId,
                    Type = (int)AccountType.Reduction,
                    AdminID = payInfo.Inputer.ID,
                    Business = first.Business,
                    CreateDate = DateTime.Now,
                    OrderID = first.OrderID,
                    Payee = first.Payee,
                    Payer = first.Payer,

                    Price = price * oper,
                    Currency = (int)currency,

                    Currency1 = (int)Currency.CNY,
                    ERate1 = rate,
                    Price1 = price * rate * oper,

                    VoucherID = first.VoucherID,
                });

                //添加实付
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payments()
                {
                    ID = PKeySigner.Pick(PKeyType.Payments),
                    CreateDate = DateTime.Now,
                    WaybillID = first.WaybillID,
                    Price = price,
                    AdminID = payInfo.Inputer.ID,
                    OrderID = first.OrderID,
                    PayableID = first.ID,
                    AccountType = (int)AccountType.Reduction,
                    FlowID = flowId,

                    Currency1 = (int)Currency.CNY,
                    Rate1 = rate,
                    Price1 = price * rate,
                });
            }
        }
        #endregion

        #region 财务付款确认
        public void Confirm(VoucherInput input)
        {
            using (var reponsitory = new PvbCrmReponsitory(false))
            using (var bankFlows = new BankFlowAccountsView(reponsitory))
            using (var view = new PaymentsStatisticsView(reponsitory))
            {
                var orderID = this.payableses.FirstOrDefault(item => item.OrderID != null)?.OrderID;
                var orderIds = this.payableses.Where(item => item.VoucherID == input.VoucherID).Select(item => item.OrderID).ToArray();
                var statistics = view.Where(GetExpressionVoucherStatistics(view, input, input.Currency, payablesIds: payableses.Select(item => item.ID).ToArray(), orderIDs: orderID));
                if (orderIds.Length > 1)
                {
                    statistics = view.Where(GetExpressionVoucherStatistics(view, input, input.Currency, payablesIds: payableses.Select(item => item.ID).ToArray(), orderIDs: orderIds));
                }
                var rate = ExchangeRates.Universal[input.Currency, Currency.CNY];
                var bankFlow = bankFlows[input.FormCode];       //银行收款流水
                decimal total = statistics.ToArray().Sum(item => item.Remains);

                if (input.Currency != bankFlow.Currency)
                {
                    throw new Exception("币种不一致!");
                }

                if (string.IsNullOrWhiteSpace(bankFlow.FlowAccountID))
                {
                    throw new Exception("未找到该银行流水!");
                }

                decimal remainPrice = bankFlow.Price;      //剩余金额

                //if (remainPrice < total)
                //{
                //    throw new Exception("银行流水剩余金额不足!");
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

                #region 核销账单
                //DeductionVouchers(reponsitory, statistics, input, rate: rate);

                decimal payPrice = 0;
                string flowId_Temp = string.Empty;

                foreach (var voucher in statistics)
                {
                    //允许实付大于应付

                    //if (voucher.Remains <= 0 || (string.IsNullOrWhiteSpace(voucher.OrderID) && string.IsNullOrWhiteSpace(voucher.ApplicationID)))
                    //{
                    //    continue;
                    //}

                    payPrice = remainPrice > voucher.Remains ? voucher.Remains : remainPrice;

                    //核销科目金额有值，根据核销金额进行核销
                    if (fees != null && fees.Any(item => item.LeftID == voucher.PayableID))
                    {
                        payPrice = fees.Single(item => item.LeftID == voucher.PayableID).RightPrice ?? 0;
                    }

                    if (payPrice <= 0)
                    {
                        continue;
                    }

                    flowId_Temp = PKeySigner.Pick(PKeyType.FlowAccount);

                    //添加实付
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payments()
                    {
                        ID = PKeySigner.Pick(PKeyType.Payments),
                        CreateDate = DateTime.Now,
                        WaybillID = voucher.WaybillID,
                        Price = payPrice,
                        AdminID = input.CreatorID,
                        OrderID = voucher.OrderID,
                        PayableID = voucher.PayableID,
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
                        AdminID = input.CreatorID,
                        Business = input.Business,
                        CreateDate = DateTime.Now,
                        OrderID = voucher.OrderID,
                        VoucherID = voucher.VoucherID,
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

                    remainPrice -= payPrice;
                }

                #endregion

                #region 统一提交
                reponsitory.Submit();
                #endregion

                #region 修改订单状态
                //付款核销 有可能是多订单
                if (orderIds.Length > 0)
                {
                    foreach (var orderId in orderIds)
                    {
                        List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();

                        whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                        {
                            Source = SourceType.ConfirmPayment,
                            OrderID = orderId,
                            OperatorID = input.CreatorID,
                            ApplicationID = input.ApplicationID,
                            Status =
                                statistics.Where(GetExpressionVoucherStatistics(view, input, orderIDs: orderId))
                                    .ToList()
                                    .Sum(item => item.Remains) == 0
                                    ? OrderPaymentStatus.Paid
                                    : OrderPaymentStatus.PartPaid
                        });

                        if (whsEventArgs.Count > 0)
                        {
                            this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(orderID))
                    {
                        List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();

                        whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                        {
                            Source = SourceType.ConfirmPayment,
                            OrderID = orderID,
                            OperatorID = input.CreatorID,
                            ApplicationID = input.ApplicationID,
                            Status = statistics.Where(GetExpressionVoucherStatistics(view, input, orderIDs: orderID)).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                        });

                        if (whsEventArgs.Count > 0)
                        {
                            this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                        }
                    }
                }
                #endregion
            }
        }

        #region bak
        void _Confirm(VoucherInput input)
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var view = new PaymentsStatisticsView(reponsitory))
            {
                var orderID = this.payableses.FirstOrDefault(item => item.OrderID != null).OrderID;
                var statistics = view.Where(GetExpressionVoucherStatistics(view, input, input.Currency, orderIDs: orderID));
                var rate = ExchangeRates.Universal[input.Currency, Currency.CNY];
                decimal flowPrice = input.Price;

                #region 特殊科目处理
                var statistics_Spe = statistics.Where(
                    item => item.Business == ConductConsts.代仓储 && item.Catalog == CatalogConsts.货款 &&
                            item.Subject == "代付货款").ToArray();

                if (statistics_Spe?.Length > 0)
                {
                    //判断客户余额是否足够

                    //待支付金额
                    var payPrice_Temp = statistics_Spe.Sum(item => item.Remains);
                    var voucher = GetVoucher(statistics_Spe.FirstOrDefault(), reponsitory);
                    var remains = PaymentManager.Npc[voucher.AgentID, input.Payer][input.Business].Digital[input.Currency].Available;

                    if (payPrice_Temp > remains)
                    {
                        throw new Exception("客户余额不足!");
                    }

                    //扣除用余额支付部分
                    flowPrice = input.Price - payPrice_Temp;
                }
                #endregion

                #region 添加流水
                if (flowPrice > 0)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.Cash,
                        AdminID = this.payInfo.Inputer.ID,
                        Business = input.Business,
                        CreateDate = DateTime.Now,
                        OrderID = orderID,
                        Payee = string.IsNullOrWhiteSpace(input.Payee) ? null : input.Payee,
                        Payer = string.IsNullOrWhiteSpace(input.Payer) ? null : input.Payer,

                        Price = flowPrice,
                        Currency = (int)input.Currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = flowPrice * rate,

                        Account = input.Account,
                        Bank = input.Bank,
                        FormCode = input.FormCode,
                    });
                }
                #endregion

                #region 核销账单
                DeductionVouchers(reponsitory, statistics, input, rate: rate);
                #endregion

                #region 修改订单状态
                List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();

                whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                {
                    Source = SourceType.ConfirmPayment,
                    OrderID = orderID,
                    OperatorID = input.CreatorID,
                    ApplicationID = input.ApplicationID,
                    Status = statistics.Where(GetExpressionVoucherStatistics(view, input, orderIDs: orderID)).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                });

                if (whsEventArgs.Count > 0)
                {
                    this.Fire(input, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                }

                #endregion
            }
        }

        #endregion
        #endregion

        #region 根据申请ID获取虚拟余额
        /// <summary>
        /// 根据申请ID获取虚拟余额
        /// </summary>
        /// <param name="applicationID">申请ID</param>
        /// <returns></returns>
        public decimal GetVirtualBalance(string applicationID)
        {
            using (var reponsitory = new PvbCrmReponsitory())
            using (var voucherView = new VouchersStatisticsView(reponsitory))
            using (var paymentView = new PaymentsStatisticsView(reponsitory))
            {
                //SELECT ISNULL(SUM(v.RightPrice), 0) -ISNULL(SUM(p.RightPrice), 0)
                //    FROM dbo.VouchersStatisticsView v JOIN
                //    dbo.PaymentsStatisticsView p ON v.ApplicationID = p.ApplicationID
                return (voucherView.Where(item => item.ApplicationID == applicationID).Sum(item => item.RightPrice) ?? 0) -
                       (paymentView.Where(item => item.ApplicationID == applicationID).Sum(item => item.RightPrice) ?? 0);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 记录流水
        /// </summary>
        private string RecordFlowAccount(PvbCrmReponsitory reponsitory, IQueryable<PaymentsStatistic> rbView,
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
                    AdminID = payInfo.Inputer.ID,
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
                });
            }

            //添加实际减去流水
            decimal priceFlow = rbView.ToList().Sum(item => item.Remains);
            priceFlow = priceFlow <= price ? priceFlow : price;
            decimal symbol = -1;

            //如果是代付货款，是添加流水,不是核销
            if (rbFirst.Business == ConductConsts.代仓储 && rbFirst.Catalog == CatalogConsts.货款 &&
                rbFirst.Subject == "代付货款")
            {
                symbol = 1;
            }

            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
            {
                ID = flowId,
                Type = (int)AccountType.Cash,
                AdminID = payInfo.Inputer.ID,
                Business = rbFirst.Business,
                CreateDate = DateTime.Now,
                OrderID = rbFirst.OrderID,
                Payee = rbFirst.Payee,
                Payer = rbFirst.Payer,

                Price = priceFlow * symbol,
                Currency = (int)currency,

                Currency1 = (int)Currency.CNY,
                ERate1 = rate,
                Price1 = priceFlow * rate * symbol,

                Account = bankFlow?.Account,
                Bank = bankFlow?.Bank,
                FormCode = bankFlow?.FormCode,
            });

            return flowId;
        }

        /// <summary>
        /// 记录实付
        /// </summary>
        /// <returns></returns>
        private string RecordPayment(PvbCrmReponsitory reponsitory, IQueryable<PaymentsStatistic> rbView, string flowId, string id, string summay,
            Currency currency, decimal rate, decimal price)
        {
            string result = id;
            decimal payPrice = 0;               //支付金额
            decimal remainPrice = price;        //剩余金额

            if (string.IsNullOrEmpty(id))
            {
                result = id = PKeySigner.Pick(PKeyType.Payments);
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

                //实付
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payments()
                {
                    ID = id,
                    CreateDate = DateTime.Now,
                    WaybillID = rb.WaybillID,
                    Price = payPrice,
                    AdminID = payInfo.Inputer.ID,
                    OrderID = rb.OrderID,
                    PayableID = rb.PayableID,
                    AccountType = (int)AccountType.Cash,
                    FlowID = flowId,

                    Currency1 = (int)Currency.CNY,
                    Rate1 = rate,
                    Price1 = payPrice * rate,

                    Summay = summay,
                });

                id = PKeySigner.Pick(PKeyType.Payments);
                //更新剩余金额
                remainPrice -= payPrice;
            }

            return result;
        }

        private Yahv.Payments.Models.Origins.Voucher InsertVoucher(VoucherInput entity, PvbCrmReponsitory reponsitory, string orderId)
        {
            Voucher voucherEntity;

            //添加财务通知单
            var voucher =
                reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                    .SingleOrDefault(item => item.OrderID == orderId && item.Type == (int)entity.Type);

            if (voucher == null)
            {
                voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
                {
                    Payee = entity.Payee,
                    Payer = entity.Payer,
                    Type = entity.Type,
                    OrderID = orderId,
                    CreateDate = DateTime.Now,
                    CreatorID = entity.CreatorID,
                    Currency = entity.Currency,
                };
            }
            else
            {
                voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
                {
                    Payee = voucher.Payee,
                    Payer = voucher.Payer,
                    Type = (VoucherType)voucher.Type,
                    OrderID = orderId,
                    CreateDate = DateTime.Now,
                    CreatorID = voucher.CreatorID,
                    ID = voucher.ID,
                    ApplicationID = voucher.ID,
                    DateIndex = voucher.DateIndex,
                    IsSettlement = voucher?.IsSettlement ?? false,
                    Currency = (Currency)voucher.Currency,
                };
            }

            //添加或修改
            voucherEntity.Enter();

            ////添加财务记录
            //reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.VoucherRecords()
            //{
            //    CreateDate = DateTime.Now,
            //    CreatorID = entity.CreatorID,
            //    Currency = (int)entity.Currency,
            //    ID = PKeySigner.Pick(PKeyType.VoucherRecords),
            //    Price = entity.Price,
            //    Bank = entity.Bank,
            //    FormCode = entity.FormCode,
            //    VoucherID = voucherEntity.ID,
            //    Account = entity.Account,
            //});

            return voucherEntity;
        }

        /// <summary>
        /// 核销账单金额
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="input"></param>
        private decimal DeductionVouchers(PvbCrmReponsitory reponsitory, IQueryable<PaymentsStatistic> statistics, VoucherInput input, decimal balance = 0, decimal rate = 1)
        {
            decimal remainPrice = input.Price;      //剩余金额
            decimal payPrice = 0;

            //如果有余额，先用余额支付
            if (balance > 0)
            {
                remainPrice = balance;
            }

            dynamic obj;

            foreach (var voucher in statistics)
            {
                if (voucher.Remains <= 0 || string.IsNullOrWhiteSpace(voucher.OrderID))
                {
                    continue;
                }

                payPrice = remainPrice > voucher.Remains ? voucher.Remains : remainPrice;

                if (payPrice <= 0)
                {
                    continue;
                }

                obj = GetVoucher(voucher, reponsitory);

                //添加实付
                this.For(voucher.PayableID)
                    .Record(input.Currency, payPrice, bankFlow: new BankFlow(input.Bank, input.Account, input.FormCode));


                //添加VoucherRecords（根据每条对账单核销）
                //reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.VoucherRecords()
                //{
                //    CreateDate = DateTime.Now,
                //    CreatorID = input.CreatorID,
                //    Currency = (int)input.Currency,
                //    ID = PKeySigner.Pick(PKeyType.VoucherRecords),
                //    Price = payPrice,
                //    Bank = input.Bank,
                //    FormCode = input.FormCode,
                //    VoucherID = obj.ID,
                //    Account = input.Account,
                //});

                //如果是代付货款，需要核销客户流水
                if (voucher.Business == ConductConsts.代仓储 && voucher.Catalog == CatalogConsts.货款 &&
                    voucher.Subject == "代付货款")
                {
                    //核销客户流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.Cash,
                        AdminID = this.payInfo.Inputer.ID,
                        Business = input.Business,
                        CreateDate = DateTime.Now,
                        OrderID = voucher.OrderID,
                        Payee = input.Payer,
                        Payer = obj.AgentID,

                        Price = -payPrice,
                        Currency = (int)input.Currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = -payPrice * rate,

                        Account = input.Account,
                        Bank = input.Bank,
                        FormCode = input.FormCode,
                    });
                }

                remainPrice -= payPrice;
            }

            if (balance > 0)
            {
                remainPrice = input.Price - balance;
            }

            return remainPrice;
        }

        /// <summary>
        /// 获取财务通知单ID
        /// </summary>
        /// <returns></returns>
        private dynamic GetVoucher(PaymentsStatistic voucher, PvbCrmReponsitory reponsitory)
        {
            Expression<Func<Layers.Data.Sqls.PvbCrm.Vouchers, bool>> predication = item => item.OrderID == voucher.OrderID && item.Type == (int)VoucherType.Payment;

            if (!string.IsNullOrWhiteSpace(voucher.ApplicationID))
            {
                predication = predication.And(item => item.ApplicationID == voucher.ApplicationID);
            }

            var entity = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>().Single(predication);

            if (entity == null || string.IsNullOrWhiteSpace(entity.ID))
            {
                return new
                {
                    ID = string.Empty,
                    AgentID = string.Empty,
                };
            }
            else
            {
                return new
                {
                    ID = entity.ID,
                    //AgentID = entity.AgentID,
                };
            }
        }

        private Expression<Func<PaymentsStatistic, bool>> GetExpressionVoucherStatistics(PaymentsStatisticsView vouchers, VoucherInput input, Currency currency = Currency.Unknown, string[] payablesIds = null, params string[] orderIDs)
        {
            Expression<Func<PaymentsStatistic, bool>> predication =
                item => item.Payer == input.Payer && item.Payee == input.Payee;

            if (orderIDs.Any(item => item != null))
            {
                predication = predication.And(item => orderIDs.Contains(item.OrderID));
            }

            if (!string.IsNullOrWhiteSpace(input.ApplicationID))
            {
                predication = predication.And(item => item.ApplicationID == input.ApplicationID);
            }

            if (currency != Currency.Unknown)
            {
                predication = predication.And(item => item.Currency == currency);
            }

            //根据应付ID 筛选
            if (payablesIds?.Length > 0)
            {
                predication = predication.And(item => payableses.Select(r => r.ID).Contains(item.PayableID));
            }

            return predication;
        }
        #endregion
    }
}
