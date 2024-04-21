using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 应付
    /// </summary>
    public class Payables
    {
        private PayInfo payInfo;
        private Payable payable;

        #region 构造函数
        internal Payables(PayInfo payInfo)
        {
            this.payInfo = payInfo;
        }

        public Payables For(string payableId)
        {
            using (var view = new PayablesView())
            {
                payable = view[payableId];

                if (string.IsNullOrWhiteSpace(payable?.ID))
                {
                    throw new Exception("未找到应付款信息!");
                }

                payInfo.Conduct = payable.Business;
                payInfo.Catalog = payable.Catalog;
                payInfo.Payee = payable.Payee;
                payInfo.Payer = payable.Payer;
                payInfo.Subject = payable.Subject;
                payInfo.OrderID = payable.OrderID;
            }

            return this;
        }
        #endregion

        #region 索引器
        public PayableRecords this[string catalog]
        {
            get
            {
                var subjects = SubjectManager.Current[this.payInfo.Conduct];

                if (subjects.All(item => item.Name != catalog))
                {
                    throw new Exception($"没有找到{catalog}分类!");
                }

                payInfo.Catalog = catalog;

                return new PayableRecords(this.payInfo);
            }
        }

        public PayableRecords this[string catalog, string subject]
        {
            get
            {
                if (SubjectManager.Current[this.payInfo.Conduct].All(item => item.Name != catalog))
                {
                    throw new Exception("该分类不存在!");
                }

                payInfo.Catalog = catalog;
                payInfo.Subject = subject;

                return new PayableRecords(this.payInfo);

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
            if (string.IsNullOrWhiteSpace(payable?.ID))
            {
                throw new Exception("应付信息不存在!");
            }

            if (price < 0)
            {
                throw new Exception("重记金额不能小于0!");
            }

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var view = new PaymentsStatisticsView())
            using (var payView = new PaymentsView())
            {
                //实付金额
                decimal paidPrice = view.FirstOrDefault(item => item.PayableID == payable.ID)?.RightPrice ?? 0;
                //差额
                decimal rdPrice = paidPrice - price;

                //直接更新应付金额
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Payables>(new
                {
                    Price = price,
                }, item => item.ID == payable.ID);

                //约定 如果是0，更新应付状态为废弃，扣除已实付的金额
                //判断该订单是否有其他科目未支付
                //如果有将金额自动过继到另一个费用的实付，并且在退款账户添加对应的金额
                //如果没有直接将金额添加到普通流水
                if (price == 0)
                {
                    //更新应付状态为废弃
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Payables>(new
                    {
                        Status = (int)GeneralStatus.Closed
                    }, item => item.ID == payable.ID);
                }


                decimal payPrice = 0;               //支付金额
                decimal remainPrice = rdPrice;        //剩余支付金额

                //减去对应的实付
                foreach (var pay in payView.Where(item => item.PayableID == payable.ID))
                {
                    if (remainPrice == 0)
                    {
                        break;
                    }

                    //判断可支付金额是否足够
                    payPrice = remainPrice > pay.Price ? pay.Price : remainPrice;

                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payments()
                    {
                        ID = PKeySigner.Pick(PKeyType.Receiveds),
                        CreateDate = DateTime.Now,
                        WaybillID = pay.WaybillID,
                        Price = payPrice * -1,
                        AdminID = payInfo.Inputer.ID,
                        OrderID = pay.OrderID,
                        PayableID = payable.ID,
                        AccountType = (int)AccountType.Cash,
                        FlowID = pay.FlowID,

                        Currency1 = (int)pay.Currency1,
                        Rate1 = pay.Rate1,
                        Price1 = payPrice * pay.Rate1 * -1,
                    });

                    remainPrice -= payPrice;
                }

                //将实收金额，改到其他科目
                ChangePayment(payable.Currency, rdPrice);
            }
        }
        #endregion

        #region 确认账单
        /// <summary>
        /// 确认账单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="isSettlement">是否结算仓储费</param>
        public void Confirm(string orderId, bool isSettlement = false)
        {
            var voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
            {
                Payee = this.payInfo.Payee,
                Payer = this.payInfo.Payer,
                Type = VoucherType.Payment,
                OrderID = orderId,
                CreatorID = this.payInfo.Inputer.ID,
                IsSettlement = isSettlement,
            };

            voucherEntity.Enter();
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 更新实付
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">剩余金额</param>
        public void ChangePayment(Currency currency, decimal price)
        {
            if (price <= 0) return;

            //根据订单获取未收款的应收
            using (var pmView = new PaymentsStatisticsView())
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //获取未支付的应收
                var unPaid = pmView.Where(item => item.OrderID == payInfo.OrderID && item.Currency == currency).ToList().Where(item => item.Remains > 0);
                var rate = ExchangeRates.Universal[currency, Currency.CNY];

                //没有其他未支付应收
                if (unPaid == null || !unPaid.Any())
                {
                    //如果没有直接将金额添加到普通流水
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.Cash,
                        AdminID = payInfo.Inputer.ID,
                        Business = payInfo.Conduct,
                        CreateDate = DateTime.Now,
                        OrderID = payInfo.OrderID,
                        Payee = payInfo.Payee,
                        Payer = payInfo.Payer,

                        Price = price * -1,
                        Currency = (int)currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = price * rate * -1,
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
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payments()
                        {
                            ID = PKeySigner.Pick(PKeyType.Receiveds),
                            CreateDate = DateTime.Now,
                            WaybillID = rb.WaybillID,
                            Price = payPrice,
                            AdminID = payInfo.Inputer.ID,
                            OrderID = rb.OrderID,
                            PayableID = rb.PayableID,
                            AccountType = (int)AccountType.Cash,

                            Currency1 = (int)Currency.CNY,
                            Rate1 = rate,
                            Price1 = payPrice * rate,
                        });

                        //更新剩余金额
                        remainPrice -= payPrice;

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


                    //如果还有剩余金额转普通流水
                    if (remainPrice > 0)
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                        {
                            ID = PKeySigner.Pick(PKeyType.FlowAccount),
                            Type = (int)AccountType.Cash,
                            AdminID = payInfo.Inputer.ID,
                            Business = payInfo.Conduct,
                            CreateDate = DateTime.Now,
                            OrderID = payInfo.OrderID,
                            Payee = payInfo.Payee,
                            Payer = payInfo.Payer,

                            Price = remainPrice * -1,
                            Currency = (int)currency,

                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = remainPrice * rate * -1,
                        });
                    }
                }
            }
        }
        #endregion
    }
}
