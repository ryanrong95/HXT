using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views;
using Yahv.Payments.Views.Origins;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class CreditCatalogs : Services.ReceivedBase, IEnumerable<CreditCatalog>
    {
        IEnumerable<Receivable> receivables;
        Dictionary<string, CreditCatalog> target;
        PayInfo payInfo;

        #region 构造函数
        internal CreditCatalogs(PayInfo payInfo)
        {
            this.payInfo = payInfo;

            //调用信用记账的时候，没有业务、付款人、收款人参数
            if (string.IsNullOrWhiteSpace(payInfo.Conduct))
            {
                return;
            }

            var subjects = SubjectCollection.Current[this.payInfo.Conduct];
            this.target = subjects.Select(item => item.Key).
                Distinct().ToDictionary(item => item, item => new CreditCatalog(item, this.payInfo));

            using (var credits = new Views.Rolls.CreditsStatisticsView())
            {
                foreach (var item in credits.Where(item => item.Payee == payInfo.Payee
                    && item.Payer == payInfo.Payer
                    && item.Business == payInfo.Conduct))
                {
                    this[item.Catalog][item.Currency].Total = item.Total;
                    this[item.Catalog][item.Currency].Cost = item.Cost;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ienums"></param>
        /// <returns></returns>
        /// <remarks>模拟二次 构造器</remarks>
        public CreditCatalogs For(IEnumerable<Receivable> ienums)
        {
            this.receivables = ienums;

            if (receivables.GroupBy(item => item.OrderID).Count() > 1)
            {
                throw new Exception("不能对不同订单进行花费!");
            }

            var first = receivables.FirstOrDefault();
            this.payInfo.Payer = first.Payer;
            this.payInfo.Payee = first.Payee;
            this.payInfo.Conduct = first.Business;
            this.payInfo.OrderID = first.OrderID;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arry">应收ID</param>
        /// <returns></returns>
        /// <remarks>模拟二次 构造器</remarks>
        public CreditCatalogs For(params string[] arry)
        {
            using (var reponsitory = new Views.Rolls.ReceivablesRoll())
            {
                this.receivables = reponsitory.Where(item => arry.Contains(item.ID)).ToArray();

                if (receivables.GroupBy(item => item.OrderID).Count() > 1)
                {
                    throw new Exception("不能对不同订单进行支付!");
                }

                var first = receivables.FirstOrDefault();
                this.payInfo.Payer = first.Payer;
                this.payInfo.Payee = first.Payee;
                this.payInfo.Conduct = first.Business;
                this.payInfo.OrderID = first.OrderID;
            };
            return this;

        }
        #endregion

        #region 索引器
        /// <summary>
        /// 指定分类
        /// </summary>
        /// <param name="catalog">分类</param>
        /// <returns></returns>
        public CreditCatalog this[string catalog]
        {
            get
            {
                return this.target[catalog];
            }
        }
        #endregion

        #region 支付
        /// <summary>
        /// 支付
        /// </summary>
        public void Pay(Currency currency, decimal price)
        {
            if (this.receivables == null || this.receivables.Count() == 0)
            {
                throw new Exception("未找到对应的应收!");
            }

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            using (var reponsitory = new PvbCrmReponsitory())
            using (var view = new VouchersStatisticsView(reponsitory))
            {
                //应收关联应收实收统计视图
                var rbView = view.Where(item => receivables.Select(a => a.ID).Contains(item.ReceivableID) && item.Currency == currency);

                if (!rbView.Any())
                {
                    throw new Exception($"未找到[{currency.GetDescription()}]币种下的账单信息!");
                }


                decimal rate = 0;       //汇率
                Receivable rec_temp;

                /*
                 * 遍历循环应收进行 花费、实收
                 * 如果金额不足，允许支付部分
                 */

                decimal payPrice = 0;               //支付金额
                decimal remainPrice = price;        //剩余金额
                bool isEnough = true;           //额度是否足够

                foreach (var rb in rbView)
                {
                    if (remainPrice <= 0)
                    {
                        break;
                    }

                    if (rb.Remains <= 0)
                    {
                        continue;
                    }

                    //判断可支付金额是否足够
                    payPrice = remainPrice > rb.Remains ? rb.Remains : remainPrice;

                    //判断消费额度是否足够
                    if (PaymentManager.Npc[rb.Payee, rb.Payer][rb.Business].Credit[rb.Catalog][rb.Currency].Available < payPrice)
                    {
                        isEnough = false;
                        continue;
                    }

                    rate = ExchangeRates.Universal[rb.Currency, Currency.CNY];
                    rec_temp = receivables.FirstOrDefault(item => item.ID == rb.ReceivableID);

                    //花费
                    var flowId = PKeySigner.Pick(PKeyType.FlowAccount);
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = flowId,
                        Type = (int)AccountType.CreditCost,
                        AdminID = payInfo.Inputer.ID,
                        Business = rb.Business,
                        Catalog = rb.Catalog,
                        Payee = rb.Payee,
                        Payer = rb.Payer,
                        Subject = rb.Subject,
                        OrderID = rec_temp.OrderID,
                        WaybillID = rec_temp.WaybillID,
                        CreateDate = DateTime.Now,

                        Currency = (int)rb.Currency,
                        Price = payPrice,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = payPrice * rate,

                        OriginalDate = rb.OriginalDate,
                        OriginIndex = rb.OriginalIndex,
                        ChangeDate = rb.ChangeDate,
                        ChangeIndex = rb.ChangeIndex,
                        DateIndex = rb.ChangeIndex,
                    });

                    //实收
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receiveds()
                    {
                        ID = PKeySigner.Pick(PKeyType.Receiveds),
                        AdminID = payInfo.Inputer.ID,
                        AccountType = (int)AccountType.CreditCost,
                        Price = payPrice,
                        CreateDate = DateTime.Now,
                        ReceivableID = rb.ReceivableID,
                        OrderID = rec_temp.OrderID,
                        WaybillID = rec_temp.WaybillID,
                        FlowID = flowId,

                        Currency1 = (int)Currency.CNY,
                        Price1 = payPrice * rate,
                        Rate1 = rate,
                    });

                    //更新剩余金额
                    remainPrice -= payPrice;
                }


                #region 调用外部事件
                List<WhsPayConfirmedEventArgs> whsEventArgs = new List<WhsPayConfirmedEventArgs>();
                List<LsPayConfirmedEventArgs> lsEventArgs = new List<LsPayConfirmedEventArgs>();

                if (!this.payInfo.OrderID.StartsWith("LsOrder"))
                {
                    whsEventArgs.Add(new WhsPayConfirmedEventArgs()
                    {
                        Source = SourceType.Credits,
                        OrderID = this.payInfo.OrderID,
                        OperatorID = this.payInfo.Inputer.ID,
                        Status = view.Where(item => item.OrderID == this.payInfo.OrderID
                                                    && !item.OrderID.Contains("LsOrder")
                                                    && item.Payer == this.payInfo.Payer
                                                    && item.Payee == this.payInfo.Payee).ToList().Sum(item => item.Remains) == 0 ? OrderPaymentStatus.Paid : OrderPaymentStatus.PartPaid
                    });
                }
                else
                {
                    if (view.Where(item => item.OrderID == this.payInfo.OrderID
                                           && item.OrderID.Contains("LsOrder")
                                           && item.Payer == this.payInfo.Payer
                                           && item.Payee == this.payInfo.Payee).ToList().Sum(item => item.Remains) == 0)
                    {
                        lsEventArgs.Add(new LsPayConfirmedEventArgs()
                        {
                            LsOrderID = this.payInfo.OrderID,
                            OperatorID = this.payInfo.Inputer.ID,
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
                #endregion

                if (!isEnough)
                {
                    throw new Exception("信用额度不够!");
                }
            }
        }
        #endregion

        #region 财务还款确认
        public void Confirm(VoucherInput entity)
        {
            if (entity.Price <= 0)
            {
                throw new Exception("还款金额不能小于等于0!");
            }

            using (var reponsitory = new PvbCrmReponsitory())
            using (var creditsView = new CreditsRepayStatisticsView(reponsitory))
            using (var flowAccounts = new FlowAccountsOrigin(reponsitory))
            {
                var voucher =
                    reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>()
                        .SingleOrDefault(item => item.DateIndex == entity.DateIndex && item.Type == (int)entity.Type);


                //判断时候已经添加财务通知单
                if (voucher == null || string.IsNullOrWhiteSpace(voucher.ID))
                {
                    var voucherId = PKeySigner.Pick(PKeyType.Vouchers);

                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Vouchers()
                    {
                        //Currency = (int)entity.Currency,
                        Payee = entity.Payee,
                        ID = voucherId,
                        Payer = entity.Payer,
                        Status = (int)VoucherStatus.Confirmed,
                        Type = (int)entity.Type,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.CreatorID,
                        DateIndex = entity.DateIndex,
                    });

                    //reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.VoucherRecords()
                    //{
                    //    CreateDate = DateTime.Now,
                    //    CreatorID = entity.CreatorID,
                    //    Currency = (int)entity.Currency,
                    //    ID = PKeySigner.Pick(PKeyType.VoucherRecords),
                    //    Price = entity.Price,
                    //    Bank = entity.Bank,
                    //    FormCode = entity.FormCode,
                    //    VoucherID = voucherId,
                    //    Account = entity.Account,
                    //});

                }
                //添加记录
                else
                {
                    //reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.VoucherRecords()
                    //{
                    //    CreateDate = DateTime.Now,
                    //    CreatorID = entity.CreatorID,
                    //    Currency = (int)entity.Currency,
                    //    ID = PKeySigner.Pick(PKeyType.VoucherRecords),
                    //    Price = entity.Price,
                    //    Bank = entity.Bank,
                    //    FormCode = entity.FormCode,
                    //    VoucherID = voucher.ID,
                    //    Account = entity.Account,
                    //});
                }

                //获取账单列表,并按照分类分组
                var list =
                    creditsView.Where(
                        item =>
                            item.Payer == entity.Payer && item.Payee == entity.Payee && item.Business == entity.Business
                            && item.Currency == entity.Currency && item.ChangeIndex == entity.DateIndex).GroupBy(item => item.Catalog).Select(item => new
                            {
                                Catalog = item.Key,
                                LeftPrice = item.Sum(t => t.RightPrice),
                            }).ToList();


                //流水还款金额
                var flows = flowAccounts.Where(item => item.Type == AccountType.CreditCost && item.Price < 0 && item.Catalog != null
                                                       && item.Payer == entity.Payer
                                                       && item.Payee == entity.Payee
                                                       && item.DateIndex == entity.DateIndex
                                                       && item.Business == entity.Business).GroupBy(item => item.Catalog).Select(item => new
                                                       {
                                                           Catalog = item.Key,
                                                           RightPrice = item.Where(t => t.Catalog == item.Key).Sum(t => t.Price)
                                                       }).ToList();

                //根据分类分组还款账单
                var credits = from c in list
                              join _f in flows on c.Catalog equals _f.Catalog into joinF
                              from f in joinF.DefaultIfEmpty()
                              select new
                              {
                                  Catalog = c.Catalog,
                                  Price = c.LeftPrice + (f?.RightPrice ?? 0)
                              };

                var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];
                decimal payPrice = 0;       //支付金额
                decimal remainPrice = entity.Price;        //剩余金额

                //遍历账单，根据分类还款信用
                foreach (var credit in credits)
                {
                    if (remainPrice <= 0)
                    {
                        break;
                    }

                    if (credit.Price <= 0)
                    {
                        continue;
                    }

                    payPrice = remainPrice > credit.Price ? (credit.Price ?? 0) : remainPrice;

                    //添加流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.CreditCost,
                        AdminID = payInfo.Inputer.ID,
                        Business = entity.Business,
                        Catalog = credit.Catalog,
                        CreateDate = DateTime.Now,
                        Payee = entity.Payee,
                        Payer = entity.Payer,

                        Price = -payPrice,
                        Currency = (int)entity.Currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = -payPrice * rate,

                        Account = entity.Account,
                        Bank = entity.Bank,
                        FormCode = entity.FormCode,
                        DateIndex = entity.DateIndex,
                    });

                    remainPrice -= payPrice;
                }
            }
        }
        #endregion

        #region GetEnumerator
        public IEnumerator<CreditCatalog> GetEnumerator()
        {
            return this.target.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
