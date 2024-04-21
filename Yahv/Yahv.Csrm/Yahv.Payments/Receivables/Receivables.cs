using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq.Extends;
using Yahv.Payments.Models;
using Yahv.Payments.Views;
using Yahv.Payments.Views.Origins;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Events;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Settings;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using FlowAccount = Yahv.Services.Models.FlowAccount;

namespace Yahv.Payments
{
    /// <summary>
    /// 应收款项
    /// </summary>
    public class Receivables : ReceivedBase
    {

        #region 属性
        public PayInfo payInfo;
        private Receivable receivable;
        #endregion

        #region 构造函数
        internal Receivables(PayInfo payInfo)
        {
            this.payInfo = payInfo;
        }

        public Receivables For(string receivableId)
        {
            using (var view = new ReceivablesView())
            {
                receivable = view.FirstOrDefault(item => item.ID == receivableId);

                if (receivable == null || string.IsNullOrWhiteSpace(receivable.ID))
                {
                    throw new Exception("应收款项不存在!");
                }

                payInfo.Conduct = receivable.Business;
                payInfo.Catalog = receivable.Catalog;
                payInfo.Payee = receivable.Payee;
                payInfo.Payer = receivable.Payer;
                payInfo.Subject = receivable.Subject;
                payInfo.OrderID = receivable.OrderID;
            }

            return this;
        }
        #endregion

        #region 索引器
        public Recorder this[string catalog]
        {
            get
            {
                var subjects = SubjectManager.Current[this.payInfo.Conduct];

                if (subjects.All(item => item.Name != catalog))
                {
                    throw new Exception($"没有找到{catalog}分类!");
                }

                payInfo.Catalog = catalog;
                return new ReceivableRecords(this.payInfo);
            }
        }

        public Recorder this[string catalog, string subject]
        {
            get
            {
                if (SubjectManager.Current[this.payInfo.Conduct].All(item => item.Name != catalog))
                {
                    throw new Exception("该分类不存在!");
                }

                payInfo.Catalog = catalog;
                payInfo.Subject = subject;

                return new ReceivableRecords(this.payInfo);
            }
        }
        #endregion

        #region 重新记账
        /// <summary>
        /// 重新记账
        /// </summary>
        /// <param name="price">金额</param>
        public void ReRecord(decimal price)
        {
            if (string.IsNullOrWhiteSpace(receivable?.ID))
            {
                throw new Exception("应收信息不存在!");
            }

            if (price < 0)
            {
                throw new Exception("重记金额不能小于0!");
            }

            using (var reponsitory = new PvbCrmReponsitory())
            using (var view = new VouchersStatisticsView(reponsitory))
            {
                var voucher = view.FirstOrDefault(item => item.ReceivableID == receivable.ID);
                if (voucher == null)
                {
                    throw new Exception("未找到账单信息!");
                }

                var currencySettle = voucher.Currency;
                var rateSettle = ExchangeRates.Universal[receivable.Currency, currencySettle];

                //实收金额
                decimal paidPrice = voucher?.RightPrice ?? 0;
                //差额（大于0 需要减点实收，小于0不需要）
                decimal rdPrice = paidPrice - (price * rateSettle);
                //汇率
                decimal rate = ExchangeRates.Universal[receivable.Currency, Currency.CNY];
                //美元汇率
                var rateUsd = ExchangeRates.Universal[currencySettle, Currency.CNY] /
                              ExchangeRates.Universal[Currency.USD, Currency.CNY];

                var orderEventArg = new OrderEventArgs(receivable.OrderID);
                this.Fire(this, orderEventArg);

                var receivableRecords = new ReceivableRecords(null);
                //var originDate = receivableRecords.GetOriginalDate(receivable.Payer, receivable.Payee, receivable.Business, receivable.Catalog, orderEventArg.OrderCreateDate ?? DateTime.Now);
                var originDate = receivableRecords.GetOriginalDate(receivable.Payer, receivable.Payee, receivable.Business, receivable.Catalog, DateTime.Now);
                int dateIndex = receivableRecords.GetDateIndexByDateTime(originDate);

                //直接更新应收金额
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                {
                    Price = price,
                    Price1 = price * rate,
                    Rate1 = rate,
                    Price11 = (price * rateSettle * rateUsd).Round(),
                    Rate11 = rateUsd,
                    SettlementPrice = (price * rateSettle).Round(),
                    SettlementRate = rateSettle,
                    ChangeDate = originDate.Date,
                    ChangeIndex = dateIndex,
                }, item => item.ID == receivable.ID);

                //约定 如果是0，更新应收状态为废弃，扣除已实收的金额
                //判断该订单是否有其他科目未支付
                //如果有将金额自动过继到另一个费用的实收
                //如果没有直接将金额添加到流水,并且在退款账户添加对应的金额
                if (price == 0)
                {
                    //更新应收状态为废弃
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                    {
                        Status = (int)GeneralStatus.Closed
                    }, item => item.ID == receivable.ID);
                }


                decimal payPrice = 0;               //支付金额
                decimal remainPrice = rdPrice;        //剩余支付金额

                //大于0，需要减掉实收，小于0表示没有产生实收
                if (rdPrice > 0)
                {
                    //判断可支付金额是否足够
                    payPrice = remainPrice > voucher.RightPrice ? (voucher.RightPrice ?? 0) : remainPrice;


                    decimal rate1 = ExchangeRates.Universal[currencySettle, Currency.CNY];
                    //减去实收
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                    {
                        ID = PKeySigner.Pick(PKeyType.Receiveds),
                        CreateDate = DateTime.Now,
                        WaybillID = voucher.WaybillID,
                        Price = -payPrice,
                        AdminID = payInfo.Inputer.ID,
                        OrderID = voucher.OrderID,
                        ReceivableID = receivable.ID,
                        AccountType = (int)GetAccountType(receivable.ID),

                        Currency1 = (int)Currency.CNY,
                        Rate1 = rate1,
                        Price1 = -payPrice * rate1,
                    });
                }

                //修改实收
                ModifyReceived(receivable.SettlementCurrency, rdPrice);
            }
        }
        #endregion

        #region 修改本位币
        /// <summary>
        /// 修改本位币
        /// </summary>
        /// <param name="price"></param>
        public void ModifyRmb(decimal price)
        {
            if (string.IsNullOrWhiteSpace(receivable?.ID))
            {
                throw new Exception("应收信息不存在!");
            }

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            using (var reponsitory = new PvbCrmReponsitory())
            using (var view = new VouchersStatisticsView(reponsitory))
            {
                var voucher = view.FirstOrDefault(item => item.ReceivableID == receivable.ID);
                if (voucher == null)
                {
                    throw new Exception("未找到账单信息!");
                }

                var usdRate = ExchangeRates.Universal[Currency.CNY, Currency.USD];

                //更新为人民币
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                {
                    Price = price,
                    Currency = (int)Currency.CNY,

                    Price1 = price,
                    Rate1 = 1m,

                    SettlementPrice = price,
                    SettlementRate = 1m,
                    SettlementCurrency = (int)Currency.CNY,

                    Price11 = (price * usdRate).Round(),
                    Rate11 = usdRate,
                    Currency11 = (int)Currency.USD,
                }, item => item.ID == receivable.ID);
            }
        }
        #endregion

        #region 申请记账
        /// <summary>
        /// 申请记账
        /// </summary>
        /// <param name="applicationID">申请ID</param>
        /// <param name="array">费用数组</param>
        /// <returns></returns>
        public void ApplyRecord(Currency currency, string applicationID, params ApplyFee[] array)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationID) || array.Length <= 0)
                {
                    throw new Exception("参数不能为空!");
                }

                using (var reponsitory = new PvbCrmReponsitory())
                using (var vouchersView = new VouchersStatisticsView(reponsitory))
                {
                    InserVoucher(reponsitory, string.Empty, applicationID, currency);     //添加账单Vouchers

                    var rate = ExchangeRates.Universal[currency, Currency.CNY];     //汇率
                    var rateUsd = ExchangeRates.Universal[currency, Currency.USD];      //发生币种与美元汇率

                    string[] receivableIds = PKeySigner.Series(PKeyType.Receivables, array.Length);     //批量生成主键
                    DateTime currTime = DateTime.Now;
                    DateTime originDate = DateTime.Now;
                    int dateIndex;

                    List<Layers.Data.Sqls.PvbCrm.Receivables> list = new List<Layers.Data.Sqls.PvbCrm.Receivables>();

                    for (int i = 0; i < array.Length; i++)
                    {
                        var fee = array[i];
                        originDate = GetOriginalDate(this.payInfo.Payer, this.payInfo.Payee, this.payInfo.Conduct, fee.Catalog, currTime);
                        dateIndex = GetDateIndexByDateTime(originDate);

                        list.Add(new Layers.Data.Sqls.PvbCrm.Receivables()
                        {
                            ID = receivableIds[i],
                            Payer = this.payInfo.Payer,
                            PayerID = this.payInfo.PayerID,
                            PayerAnonymous = this.payInfo.PayerAnonymous,
                            Payee = this.payInfo.Payee,
                            PayeeID = this.payInfo.PayeeID,
                            PayeeAnonymous = this.payInfo.PayeeAnonymous,
                            Business = this.payInfo.Conduct,
                            Catalog = fee.Catalog,
                            Subject = fee.Subject,
                            //TinyID = tinyID,
                            //ItemID = itemID,
                            ApplicationID = applicationID,

                            Currency = (int)currency,
                            Price = fee.Price ?? 0m,

                            Quantity = null,

                            Currency1 = (int)Currency.CNY,
                            Rate1 = rate,
                            Price1 = (fee.Price * rate) ?? 0m,

                            Currency11 = (int)Currency.USD,
                            Rate11 = rateUsd,
                            Price11 = ((fee.Price * rateUsd) ?? 0m).Round(),

                            SettlementCurrency = (int)currency,
                            SettlementPrice = ((fee.Price) ?? 0m).Round(),
                            SettlementRate = 1,

                            OrderID = fee.OrderID,
                            //WaybillID = waybillID,
                            AdminID = this.payInfo.Inputer.ID,
                            Summay = string.Empty,

                            CreateDate = currTime,
                            OriginalIndex = dateIndex,
                            OriginalDate = originDate.Date,
                            ChangeDate = originDate.Date,
                            ChangeIndex = dateIndex,
                            //Source = source,
                            //TrackingNumber = trackingNum,
                            Status = (int)GeneralStatus.Normal,
                        });
                    }

                    if (list.Count > 0)
                    {
                        reponsitory.Insert<Layers.Data.Sqls.PvbCrm.Receivables>(list);
                    }


                    #region 调用外部事件，更新订单状态

                    foreach (var fee in array)
                    {
                        List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                        whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                        {
                            Source = SourceType.Receivable,
                            OrderID = fee.OrderID,
                            OperatorID = this.payInfo.Inputer.ID,
                            Status = vouchersView.Where(item => !item.OrderID.Contains("LsOrder"))
                            .Where(GetExpressionVoucherStatistics(vouchersView, this.payInfo.Payer, this.payInfo.Payee, fee.OrderID, applicationID))
                            .ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                        });

                        if (whsEventArgs.Count > 0)
                        {
                            this.Fire(null, new ConfirmedEventArgs<WhsPayConfirmedEventArgs>(whsEventArgs.ToArray()));
                        }

                    }
                    #endregion
                }

                Oplogs.Oplog(this.payInfo.Inputer.ID, typeof(Receivables).FullName, "Pays", $"申请记账", $"ApplyRecord(currency:{currency},applicationID:{applicationID},fee[]:{array.Length})", array.Json());
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.payInfo.Inputer.ID, typeof(Receivables).FullName, ex, remark: $"ApplyRecord(currency:{currency},applicationID:{applicationID},fee[]:{array.Json()})");
                throw ex;
            }
        }
        #endregion


        #region 客户确认账单
        /// <summary>
        /// 客户确认账单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="currency">币种</param>
        /// <param name="isSettlement">附加科目</param>
        public void Confirm(string orderId, Currency currency, bool isSettlement = false)
        {
            var voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
            {
                Payee = this.payInfo.Payee,
                Payer = this.payInfo.Payer,
                Type = VoucherType.Receipt,
                OrderID = orderId,
                CreatorID = this.payInfo.Inputer.ID,
                Currency = currency,
                IsSettlement = isSettlement,
            };

            voucherEntity.Enter();
        }
        #endregion

        #region 芯达通
        #region 记账

        /// <summary>
        /// 芯达通方法
        /// </summary>
        /// <param name="vastOrderID">大订单ID</param>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="rate">汇率默认1（人民币对人民币）</param>
        /// <param name="itemID">订单型号ID</param>
        /// <param name="applicationID">付汇ID</param>
        /// <param name="isOnlyClearExtras">只清除杂费</param>
        /// <param name="array">费用列表</param>
        public void XdtRecord(string vastOrderID, string tinyOrderID, decimal rate = 1, string itemID = null, string applicationID = null, bool isOnlyClearExtras = false, params XdtFee[] array)
        {
            //对列调用
            if (EnterQueue.Current.Enqueue(this, new object[] { vastOrderID, tinyOrderID, rate, itemID, applicationID, isOnlyClearExtras, array }))
            {
                return;
            }

            List<Layers.Data.Sqls.PvbCrm.Receivables> result = new List<Layers.Data.Sqls.PvbCrm.Receivables>();

            try
            {
                if (string.IsNullOrWhiteSpace(vastOrderID) || string.IsNullOrWhiteSpace(tinyOrderID))
                {
                    throw new ArgumentNullException($"参数不能为空!");
                }

                using (var reponsitory = new PvbCrmReponsitory())
                using (var vouchers = new VouchersStatisticsView(reponsitory))
                using (var rdView = new ReceivedsView(reponsitory))
                using (var flowView = new FlowAccountsTopView(reponsitory))
                using (var debtView = new DebtTermsTopView<PvbCrmReponsitory>(reponsitory))
                {
                    //非杂费
                    if (!isOnlyClearExtras)
                    {
                        if (array.Length <= 0) throw new ArgumentNullException($"参数不能为空!");
                        ClearRecord(reponsitory, vouchers, tinyOrderID, applicationID); //记账前，先清空未实收的费用
                    }
                    //杂费
                    else
                    {
                        //空数组的话，需要清空之前杂费数据
                        ClearExtrasRecord(reponsitory, vouchers, tinyOrderID, applicationID);
                        if (array.Length <= 0) return;
                    }

                    InserVoucher(reponsitory, vastOrderID, applicationID, Currency.CNY);     //添加账单Vouchers

                    Currency currencySettle = Currency.CNY;       //结算币种默认人民币
                    var rateUsd = ExchangeRates.Universal[Currency.CNY, Currency.USD];      //发生币种与美元汇率

                    VoucherStatistic voucher;       //账单信息
                    ReceivableRecords receRecords = new ReceivableRecords(this.payInfo);
                    DateTime originDate;       //还款日期
                    int dateIndex;      //期号
                    string id = string.Empty;       //应收ID
                    AccountType accountType;

                    //批量生成主键
                    string[] receivableIds = PKeySigner.Series(PKeyType.Receivables, array.Length);
                    var vouchersArray = vouchers.Where(item => item.OrderID == vastOrderID).ToList();
                    var debtArray = debtView.Where(item => item.Payer == this.payInfo.Payee && item.Payee == this.payInfo.Payer).ToArray();
                    //遍历循环费用列表
                    for (int i = 0; i < array.Length; i++)
                    {
                        var xdtFee = array[i];

                        if (xdtFee.Price <= 0) continue;

                        voucher = vouchersArray.SingleOrDefault(GetVoucherListExpression(vastOrderID, tinyOrderID, itemID ?? xdtFee.ItemID, this.payInfo.Conduct, xdtFee.Catalog, xdtFee.Subject, xdtFee.Currency, applicationID));
                        originDate = receRecords.GetOriginalDate(this.payInfo.Payer, this.payInfo.Payee, this.payInfo.Conduct, xdtFee.Catalog, DateTime.Now, debtArray);
                        dateIndex = receRecords.GetDateIndexByDateTime(originDate);
                        //originDate = DateTime.Now;
                        //dateIndex = int.Parse(DateTime.Now.ToString("yyyyMM"));
                        id = voucher?.ReceivableID ?? receivableIds[i];

                        if (voucher == null || string.IsNullOrWhiteSpace(voucher.ReceivableID))
                        {
                            //如果包含 直接跳过
                            if (result.Any(GetReceivablesrListExpression(vastOrderID, tinyOrderID, itemID ?? xdtFee.ItemID, this.payInfo.Conduct, xdtFee.Catalog, xdtFee.Subject, xdtFee.Currency)))
                            {
                                continue;
                            }

                            result.Add(new Layers.Data.Sqls.PvbCrm.Receivables()
                            {
                                ID = id,
                                Payer = this.payInfo.Payer,
                                Payee = this.payInfo.Payee,
                                Business = this.payInfo.Conduct,
                                Catalog = xdtFee.Catalog,
                                Subject = xdtFee.Subject,
                                TinyID = tinyOrderID,
                                ItemID = itemID ?? xdtFee.ItemID,
                                ApplicationID = applicationID,

                                Currency = (int)xdtFee.Currency,
                                Price = xdtFee.Price,

                                Currency1 = (int)Currency.CNY,
                                Rate1 = rate,
                                Price1 = xdtFee.Price * rate,

                                Currency11 = (int)Currency.USD,
                                Rate11 = rateUsd,
                                Price11 = (xdtFee.Price * rate * rateUsd).Round(),

                                SettlementCurrency = (int)currencySettle,
                                SettlementPrice = receRecords.Round(xdtFee.Price * rate),
                                SettlementRate = rate,

                                OrderID = vastOrderID,
                                CreateDate = DateTime.Now,
                                AdminID = this.payInfo.Inputer.ID,

                                OriginalIndex = dateIndex,
                                OriginalDate = originDate.Date,
                                ChangeDate = originDate.Date,
                                ChangeIndex = dateIndex,
                                Status = (int)Underly.GeneralStatus.Normal,
                            });
                        }
                        else
                        {
                            //更新应收金额
                            reponsitory.Update<Layers.Data.Sqls.PvbCrm.Receivables>(new
                            {
                                Price = xdtFee.Price,
                                Price1 = xdtFee.Price * rate,
                                Rate1 = rate,
                                Rate11 = rateUsd,
                                Price11 = (xdtFee.Price * rate * rateUsd).Round(),
                                SettlementPrice = (xdtFee.Price * rate).Round(),
                                SettlementRate = rate,
                                ChangeDate = originDate.Date,
                                ChangeIndex = dateIndex,
                            }, item => item.ID == voucher.ReceivableID);

                            decimal paidPrice = voucher.RightPrice ?? 0m;       //实收金额
                            decimal rdPrice = paidPrice - (xdtFee.Price * rate);        //差额（大于0 需要减点实收，小于0不需要）
                            decimal payPrice = 0;               //支付金额
                            decimal remainPrice = rdPrice;        //剩余支付金额
                            FlowAccount flow;

                            //更新实收
                            if (rdPrice > 0)
                            {
                                accountType = GetAccountType(voucher.ReceivableID);

                                //减去对应的实收
                                foreach (var rd in rdView.Where(item => item.ReceivableID == voucher.ReceivableID))
                                {
                                    if (remainPrice == 0)
                                    {
                                        break;
                                    }

                                    flow = flowView.Single(item => item.ID == rd.FlowID);

                                    //判断可支付金额是否足够
                                    payPrice = remainPrice > rd.Price ? rd.Price : remainPrice;

                                    //减去实收
                                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                                    {
                                        ID = PKeySigner.Pick(PKeyType.Receiveds),
                                        CreateDate = DateTime.Now,
                                        WaybillID = rd.WaybillID,
                                        Price = -payPrice,
                                        AdminID = this.payInfo.Inputer.ID,
                                        OrderID = rd.OrderID,
                                        ReceivableID = voucher.ReceivableID,
                                        AccountType = (int)accountType,
                                        FlowID = rd.FlowID,

                                        Currency1 = (int)rd.Currency1,
                                        Rate1 = rd.Rate1,
                                        Price1 = -payPrice * rd.Rate1,
                                    });

                                    //添加流水金额
                                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                                    {
                                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                                        Type = (int)accountType,
                                        AdminID = this.payInfo.Inputer.ID,
                                        Business = this.payInfo.Conduct,
                                        CreateDate = DateTime.Now,
                                        OrderID = vastOrderID,
                                        Payee = this.payInfo.Payee,
                                        Payer = this.payInfo.Payer,

                                        Price = payPrice,
                                        Currency = (int)currencySettle,

                                        Currency1 = (int)rd.Currency1,
                                        ERate1 = rd.Rate1,
                                        Price1 = payPrice * rd.Rate1,

                                        Account = flow.Account,
                                        Bank = flow.Bank,
                                        FormCode = flow.FormCode,
                                        DateIndex = flow.DateIndex,
                                        ChangeDate = flow.ChangeDate,
                                        ChangeIndex = flow.ChangeIndex,
                                        OriginalDate = flow.OriginalDate,
                                        OriginIndex = flow.OriginIndex,
                                    });


                                    remainPrice -= payPrice;
                                }


                                #region 退款至余额
                                //退款账户 添加退款金额（正数：退款金额 负数：实际退款的金额）
                                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                                {
                                    ID = PKeySigner.Pick(PKeyType.FlowAccount),
                                    Type = (int)AccountType.Return,
                                    AdminID = this.payInfo.Inputer.ID,
                                    Business = this.payInfo.Conduct,
                                    CreateDate = DateTime.Now,
                                    OrderID = vastOrderID,
                                    Payee = this.payInfo.Payee,
                                    Payer = this.payInfo.Payer,

                                    Price = rdPrice,
                                    Currency = (int)currencySettle,

                                    Currency1 = (int)Currency.CNY,
                                    ERate1 = rate,
                                    Price1 = rdPrice * rate
                                });
                                #endregion
                            }
                        }
                    }
                    if (result.Count > 0)
                    {
                        reponsitory.SqlBulkCopyByDatatable(nameof(Receivables), ToDataTable(result));
                        //reponsitory.Insert<Layers.Data.Sqls.PvbCrm.Receivables>(result);
                    }
                    Oplogs.Oplog(this.payInfo.Inputer.ID, typeof(Receivables).FullName, "Pays", $"芯达通记账", $"XdtRecord(vastOrderID:{vastOrderID},tinyOrderID:{tinyOrderID},rate:{rate},itemID:{itemID},applicationID:{applicationID},array:{array.Length},result:{result.Count})", "");
                }
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.payInfo.Inputer.ID, typeof(Receivables).FullName, ex, remark: $"XdtRecord(vastOrderID:{vastOrderID},tinyOrderID:{tinyOrderID},rate:{rate},itemID:{itemID},applicationID:{applicationID},array:{array.Length})");
                throw ex;
            }

        }
        #endregion

        #region 记账

        /// <summary>
        /// 芯达通方法
        /// </summary>
        /// <param name="vastOrderID">大订单ID</param>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="rate">汇率默认1（人民币对人民币）</param>
        /// <param name="itemID">订单型号ID</param>
        /// <param name="applicationID">付汇ID</param>
        /// <param name="array">费用列表</param>
        public void XdtRecord_Temp(string vastOrderID, string tinyOrderID, DateTime dateTime, decimal rate = 1, string itemID = null, string applicationID = null, params XdtFee[] array)
        {
            if (string.IsNullOrWhiteSpace(vastOrderID) || string.IsNullOrWhiteSpace(tinyOrderID) || array.Length <= 0)
            {
                throw new ArgumentNullException($"参数不能为空!");
            }


            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var vouchers = new VouchersStatisticsView(reponsitory))
            {
                ClearRecord(reponsitory, vouchers, tinyOrderID, applicationID);      //记账前，先清空未实收的费用
                InserVoucher(reponsitory, vastOrderID, applicationID, Currency.CNY);     //添加账单Vouchers

                //获取订单币种
                var orderEventArg = new OrderEventArgs(vastOrderID);
                this.Fire(this, orderEventArg);

                Currency currencySettle = Currency.CNY;       //结算币种默认人民币
                VoucherStatistic voucher;       //账单信息
                ReceivableRecords receRecords = new ReceivableRecords(this.payInfo);
                DateTime originDate;       //还款日期
                int dateIndex;      //期号
                string id = string.Empty;       //应收ID
                AccountType accountType;

                //批量生成主键
                string[] pk;
                if (array.Length == 1)
                {
                    pk = new[] { PKeySigner.Pick(PKeyType.Receivables) };
                }
                else
                {
                    pk = PKeySigner.Series(PKeyType.Receivables, array.Length);
                }

                var inserts = new List<Layers.Data.Sqls.PvbCrm.Receivables>();
                var rateUsd = ExchangeRates.Universal[Currency.CNY, Currency.USD];      //发生币种与美元汇率

                //遍历循环费用列表
                for (int i = 0; i < array.Length; i++)
                {
                    var xdtFee = array[i];
                    if (xdtFee.Price <= 0) continue;

                    voucher = vouchers.SingleOrDefault(GetVoucherExpression(vastOrderID, tinyOrderID, itemID ?? xdtFee.ItemID, this.payInfo.Conduct, xdtFee.Catalog, xdtFee.Subject, xdtFee.Currency));
                    dateIndex = receRecords.GetDateIndexByDateTime(dateTime);

                    if (voucher == null || string.IsNullOrWhiteSpace(voucher.ReceivableID))
                    {
                        inserts.Add(new Layers.Data.Sqls.PvbCrm.Receivables()
                        {
                            ID = pk[i],
                            Payer = this.payInfo.Payer,
                            Payee = this.payInfo.Payee,
                            Business = this.payInfo.Conduct,
                            Catalog = xdtFee.Catalog,
                            Subject = xdtFee.Subject,
                            TinyID = tinyOrderID,
                            ItemID = itemID ?? xdtFee.ItemID,
                            ApplicationID = applicationID,

                            Currency = (int)xdtFee.Currency,
                            Price = xdtFee.Price,

                            Currency1 = (int)Currency.CNY,
                            Rate1 = rate,
                            Price1 = xdtFee.Price * rate,

                            Currency11 = (int)Currency.USD,
                            Rate11 = rateUsd,
                            Price11 = (xdtFee.Price * rate * rateUsd).Round(),

                            SettlementCurrency = (int)currencySettle,
                            SettlementPrice = receRecords.Round(xdtFee.Price * rate),
                            SettlementRate = rate,

                            OrderID = vastOrderID,
                            CreateDate = dateTime,
                            AdminID = this.payInfo.Inputer.ID,

                            OriginalIndex = dateIndex,
                            OriginalDate = dateTime,
                            ChangeDate = dateTime,
                            ChangeIndex = dateIndex,
                            Status = (int)Underly.GeneralStatus.Normal,
                        });
                    }
                }

                if (inserts.Count > 0)
                {
                    reponsitory.Insert<Layers.Data.Sqls.PvbCrm.Receivables>(inserts);
                }
            }

        }
        #endregion
        #endregion

        #region 私有函数
        /// <summary>
        /// 更新实收
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">剩余金额</param>
        private void ModifyReceived(Currency currency, decimal price)
        {
            if (price <= 0) return;

            //根据订单获取未收款的应收
            using (var reponsitory = new PvbCrmReponsitory())
            using (var rbView = new VouchersStatisticsView())
            {
                //获取未支付的应收
                var unPaid = rbView.Where(item => item.OrderID == payInfo.OrderID && item.Currency == currency).ToList().Where(item => item.Remains > 0);
                var rate = ExchangeRates.Universal[currency, Currency.CNY];
                //根据应收ID获取实收类型（信用或者现金）
                var accountType = GetAccountType(receivable.ID);        //获取重记的实收类型

                //没有其他未支付应收
                if (unPaid == null || !unPaid.Any())
                {
                    //如果没有直接将金额添加到普通流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)accountType,
                        AdminID = payInfo.Inputer.ID,
                        Business = payInfo.Conduct,
                        CreateDate = DateTime.Now,
                        OrderID = payInfo.OrderID,
                        Payee = payInfo.Payee,
                        Payer = payInfo.Payer,

                        Price = price,
                        Currency = (int)currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = price * rate,
                    });

                    //退款账户 添加退款金额（正数：退款金额 负数：实际退款的金额）
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.Return,
                        AdminID = payInfo.Inputer.ID,
                        Business = payInfo.Conduct,
                        CreateDate = DateTime.Now,
                        OrderID = payInfo.OrderID,
                        Payee = payInfo.Payee,
                        Payer = payInfo.Payer,

                        Price = price,
                        Currency = (int)currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = price * rate,
                    });
                }
                //有其他未支付应收
                else
                {
                    decimal payPrice = 0;               //支付金额
                    decimal remainPrice = price;        //剩余金额

                    foreach (var rb in unPaid)
                    {
                        if (remainPrice == 0)
                        {
                            break;
                        }

                        //判断可支付金额是否足够
                        payPrice = remainPrice > rb.Remains ? rb.Remains : remainPrice;

                        //实收
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                        {
                            ID = PKeySigner.Pick(PKeyType.Receiveds),
                            CreateDate = DateTime.Now,
                            WaybillID = rb.WaybillID,
                            Price = payPrice,
                            AdminID = payInfo.Inputer.ID,
                            OrderID = rb.OrderID,
                            ReceivableID = rb.ReceivableID,
                            AccountType = (int)AccountType.Cash,

                            Currency1 = (int)Currency.CNY,
                            Rate1 = rate,
                            Price1 = payPrice * rate,
                        });

                        //更新剩余金额
                        remainPrice -= payPrice;
                    }


                    //如果还有剩余金额转到账户流水
                    if (remainPrice > 0)
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                        {
                            ID = PKeySigner.Pick(PKeyType.FlowAccount),
                            Type = (int)accountType,
                            AdminID = payInfo.Inputer.ID,
                            Business = payInfo.Conduct,
                            CreateDate = DateTime.Now,
                            OrderID = payInfo.OrderID,
                            Payee = payInfo.Payee,
                            Payer = payInfo.Payer,

                            Price = remainPrice,
                            Currency = (int)currency,

                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = remainPrice * rate,
                        });

                        //退款账户 添加退款金额（正数：退款金额 负数：实际退款的金额）
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                        {
                            ID = PKeySigner.Pick(PKeyType.FlowAccount),
                            Type = (int)AccountType.Return,
                            AdminID = payInfo.Inputer.ID,
                            Business = payInfo.Conduct,
                            CreateDate = DateTime.Now,
                            OrderID = payInfo.OrderID,
                            Payee = payInfo.Payee,
                            Payer = payInfo.Payer,

                            Price = payPrice,
                            Currency = (int)currency,

                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = payPrice * rate,
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 获取对账单信息
        /// </summary>
        /// <param name="vastOrderID">大订单ID</param>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="business">业务</param>
        /// <param name="catalog">分类</param>
        /// <param name="subject">科目</param>
        /// <param name="currency">币种</param>
        /// <returns></returns>
        private Expression<Func<VoucherStatistic, bool>> GetVoucherExpression(string vastOrderID, string tinyOrderID, string itemID, string business, string catalog, string subject, Currency currency)
        {
            Expression<Func<VoucherStatistic, bool>> predicate = item =>
                item.OrderID == vastOrderID
                && item.TinyID == tinyOrderID
                && item.Business == business
                && item.Catalog == catalog
                && item.Subject == subject
                && item.Currency == currency;

            if (!string.IsNullOrWhiteSpace(itemID))
            {
                predicate = predicate.And(item => item.ItemID == itemID);
            }

            return predicate;
        }

        private Func<VoucherStatistic, bool> GetVoucherListExpression(string vastOrderID, string tinyOrderID, string itemID, string business, string catalog, string subject, Currency currency, string applicationID)
        {
            Func<VoucherStatistic, bool> predicate = item =>
                item.OrderID == vastOrderID
                && item.TinyID == tinyOrderID
                && item.Business == business
                && item.Catalog == catalog
                && item.Subject == subject
                && item.Currency == currency;

            if (!string.IsNullOrWhiteSpace(itemID))
            {
                predicate = item =>
                    item.OrderID == vastOrderID
                    && item.TinyID == tinyOrderID
                    && item.Business == business
                    && item.Catalog == catalog
                    && item.Subject == subject
                    && item.Currency == currency
                    && item.ItemID == itemID;
            }

            if (!string.IsNullOrWhiteSpace(applicationID))
            {
                predicate = item =>
                    item.OrderID == vastOrderID
                    && item.TinyID == tinyOrderID
                    && item.Business == business
                    && item.Catalog == catalog
                    && item.Subject == subject
                    && item.Currency == currency
                    && item.ItemID == itemID
                    && item.ApplicationID == applicationID;
            }

            return predicate;
        }

        private Func<Layers.Data.Sqls.PvbCrm.Receivables, bool> GetReceivablesrListExpression(string vastOrderID, string tinyOrderID, string itemID, string business, string catalog, string subject, Currency currency)
        {
            Func<Layers.Data.Sqls.PvbCrm.Receivables, bool> predicate = item =>
                item.OrderID == vastOrderID
                && item.TinyID == tinyOrderID
                && item.Business == business
                && item.Catalog == catalog
                && item.Subject == subject
                && item.Currency == (int)currency;

            if (!string.IsNullOrWhiteSpace(itemID))
            {
                predicate = item =>
                     item.OrderID == vastOrderID
                     && item.TinyID == tinyOrderID
                     && item.Business == business
                     && item.Catalog == catalog
                     && item.Subject == subject
                     && item.Currency == (int)currency
                     && item.ItemID == itemID;
            }

            return predicate;
        }

        /// <summary>
        /// 根据应收ID返回实收类型
        /// </summary>
        /// <remarks>只返回现金或者信用</remarks>
        /// <param name="receivableId">应收ID</param>
        /// <returns>付款类型</returns>
        private AccountType GetAccountType(string receivableId)
        {
            using (var rdView = new ReceivedsView())
            {
                return rdView.FirstOrDefault(item => item.ReceivableID == receivableId
                && (item.AccountType == AccountType.Cash || item.AccountType == AccountType.BankStatement)).AccountType;
            }
        }

        /// <summary>
        /// 根据小订单ID和申请ID清除费用
        /// </summary>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="applicationID">申请ID</param>
        /// <param name="isOnlyClearExtras">只清除杂费记录</param>
        private void ClearRecord(PvbCrmReponsitory reponsitory, VouchersStatisticsView vouchers, string tinyOrderID, string applicationID)
        {
            string[] unPayIds = null;

            //根据小订单进行清除，货款除外(产生实收的也不删除，后续进行更新)
            if (!string.IsNullOrWhiteSpace(tinyOrderID))
            {
                //根据小订单ID获取未实收的应收ID，不包括货款
                unPayIds =
                    vouchers.Where(item => item.TinyID == tinyOrderID && item.Catalog != CatalogConsts.货款)
                        .Where(item => item.RightPrice == 0 || item.RightPrice == null)
                        .Select(item => item.ReceivableID).ToArray();
            }

            //货款根据申请ID清除
            if (!string.IsNullOrWhiteSpace(applicationID))
            {
                //根据申请ID获取未实收的货款应收ID
                unPayIds =
                    vouchers.Where(item => item.ApplicationID == applicationID && item.TinyID == tinyOrderID && item.Catalog == CatalogConsts.货款)
                        .Where(item => item.RightPrice == 0 || item.RightPrice == null)
                        .Select(item => item.ReceivableID).ToArray();
            }


            if (unPayIds != null && unPayIds.Any())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvbCrm.Receivables>(item => unPayIds.Contains(item.ID));
            }
        }

        /// <summary>
        /// 根据小订单ID和申请ID清除杂费费用
        /// </summary>
        /// <param name="tinyOrderID">小订单ID</param>
        /// <param name="applicationID">申请ID</param>
        private void ClearExtrasRecord(PvbCrmReponsitory reponsitory, VouchersStatisticsView vouchers, string tinyOrderID, string applicationID)
        {
            string[] unPayIds = null;

            //根据小订单进行清除，货款除外(产生实收的也不删除，后续进行更新)
            if (!string.IsNullOrWhiteSpace(tinyOrderID))
            {
                //根据小订单ID获取未实收的应收ID，不包括货款
                unPayIds =
                    vouchers.Where(item => item.TinyID == tinyOrderID && item.Catalog != CatalogConsts.货款)
                        .Where(item => item.RightPrice == 0 || item.RightPrice == null)
                        .Select(item => item.ReceivableID).ToArray();
            }

            //货款根据申请ID清除
            if (!string.IsNullOrWhiteSpace(applicationID))
            {
                //根据申请ID获取未实收的货款应收ID
                unPayIds =
                    vouchers.Where(item => item.ApplicationID == applicationID && item.TinyID == tinyOrderID && item.Catalog == CatalogConsts.货款)
                        .Where(item => item.RightPrice == 0 || item.RightPrice == null)
                        .Select(item => item.ReceivableID).ToArray();
            }


            if (unPayIds != null && unPayIds.Any())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvbCrm.Receivables>(item => unPayIds.Contains(item.ID) && item.Catalog == CatalogConsts.杂费);
            }
        }

        /// <summary>
        /// 添加账单
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="vastOrderID"></param>
        /// <param name="applicationID"></param>
        private void InserVoucher(PvbCrmReponsitory reponsitory, string vastOrderID, string applicationID, Currency currency, DateTime? dateTime = null)
        {
            var voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
            {
                Payee = this.payInfo.Payee,
                Payer = this.payInfo.Payer,
                Type = VoucherType.Receipt,
                OrderID = vastOrderID,
                CreatorID = this.payInfo.Inputer.ID,
                ApplicationID = applicationID,
                Currency = currency,
                CreateDate = dateTime
            };

            voucherEntity.Enter();
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
