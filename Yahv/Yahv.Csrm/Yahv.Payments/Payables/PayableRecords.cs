using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Payments.Views;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 应付账款
    /// </summary>
    public class PayableRecords
    {
        public PayInfo PayInfo { get; internal set; }

        #region 构造函数
        public PayableRecords(PayInfo payInfo)
        {
            this.PayInfo = payInfo;
        }
        #endregion

        #region 记录

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="waybillID">运单ID</param>
        /// <param name="source">来源</param>
        /// <param name="trackingNum">快递单号</param>
        public string Record(Currency currency, decimal? price, string orderID = null, string waybillID = null, string id = null, string applicationID = null, string AgentID = null, decimal? rightPrice = null, string source = "", string trackingNum = "")
        {
            id = id ?? PKeySigner.Pick(PKeyType.Payables);

            //if (SubjectManager.Current[this.PayInfo.Conduct][this.PayInfo.Catalog].Subjects.All(item => item.Name != this.PayInfo.Subject) && !string.IsNullOrWhiteSpace(this.PayInfo.Subject))
            //{
            //    throw new Exception("该科目不存在!");
            //}

            if (price < 0)
            {
                throw new Exception("金额不能小于0!");
            }

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var payers = new PayersTopView(reponsitory))
            {
                if (rightPrice > 0)
                {
                    //添加付款账户
                    var payer = payers[PayInfo.Payer, Methord.Cash, currency];

                    if (payer == null || string.IsNullOrWhiteSpace(payer.ID))
                    {
                        throw new Exception($"未找到付款人对应币种的现金账户!");
                    }

                    //付款账户ID
                    this.PayInfo.PayerID = payer.ID;
                }

                DateTime currTime = DateTime.Now;       //当前时间，用于现金支付（应付、实付创建时间一致）

                #region 添加应付
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payables
                {
                    ID = id,
                    Payee = this.PayInfo.Payee,
                    Payer = this.PayInfo.Payer,
                    Business = this.PayInfo.Conduct,
                    Subject = this.PayInfo.Subject,
                    Currency = (int)currency,
                    Price = price,
                    OrderID = orderID,
                    CreateDate = currTime,
                    AdminID = this.PayInfo.Inputer.ID,
                    Summay = string.Empty,
                    PayerID = this.PayInfo.PayerID,
                    PayeeID = this.PayInfo.PayeeID,
                    WaybillID = waybillID,
                    Catalog = this.PayInfo.Catalog,
                    Status = (int)GeneralStatus.Normal,
                    ApplicationID = applicationID,
                    PayeeAnonymous = this.PayInfo.PayeeAnonymous,
                    PayerAnonymous = this.PayInfo.PayerAnonymous,
                    Source = source == "" ? null : source,
                    TrackingNumber = trackingNum == "" ? null : trackingNum,
                });
                #endregion

                #region 添加实付（现金）
                if (rightPrice > 0)
                {
                    var rate = ExchangeRates.Universal[currency, Currency.CNY];
                    //PaymentManager.Erp(this.PayInfo.Inputer.ID).Payment.For(id).Record(currency, rightPrice ?? 0);

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
                        Price1 = (price ?? 0m) * rate,
                    });

                    string flowId = PKeySigner.Pick(PKeyType.FlowAccount);

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

                        Price = -price ?? 0m,
                        Currency = (int)currency,

                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = (-price ?? 0m) * rate,
                    });

                    //实付
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payments()
                    {
                        ID = id,
                        CreateDate = currTime,
                        WaybillID = waybillID,
                        Price = price ?? 0m,
                        AdminID = this.PayInfo.Inputer.ID,
                        OrderID = orderID,
                        PayableID = id,
                        AccountType = (int)AccountType.Cash,
                        FlowID = flowId,

                        Currency1 = (int)Currency.CNY,
                        Rate1 = rate,
                        Price1 = (price ?? 0m) * rate,
                        Source = source,
                    });
                }
                #endregion

                #region 添加账单信息
                //var voucherEntity = new Yahv.Payments.Models.Origins.Voucher()
                //{
                //    OrderID = orderID,
                //    ApplicationID = applicationID,
                //    Type = VoucherType.Payment,
                //    Payer = this.PayInfo.Payer,
                //    Payee = this.PayInfo.Payee,
                //    CreatorID = this.PayInfo.Inputer.ID,
                //    Currency = currency,
                //};

                //voucherEntity.Enter(reponsitory);
                #endregion

                #region 统一提交
                reponsitory.Submit();
                #endregion
            }

            return id;
        }
        #endregion
    }

}
