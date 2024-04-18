using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Serializers;
using Needs.Ccs.Services.ApiSettings;
using Newtonsoft.Json;
using System.Net.Http;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 财务收款记录
    /// </summary>
    public class FinanceReceipt : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 收款流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 付款人（当收款类型为“预收账款”时，为客户名称）
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 财务收款类型：预收账款、资金调入、银行利息、借款、还款
        /// </summary>
        public Enums.FinanceFeeType FeeType { get; set; }

        /// <summary>
        /// 收款方式：支票、现金、转账、承兑
        /// </summary>
        public Enums.PaymentType ReceiptType { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 金库
        /// </summary>
        public FinanceVault Vault { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public FinanceAccount Account { get; set; }

        /// <summary>
        /// 收款人/财务
        /// </summary>
        public Admin Admin { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        //差额(修改后的值 - 原来的值)
        public decimal Difference { get; set; }

        public FinanceAccountFlow AccountFlow { get; set; }

        public string FinanceVaultID { get; set; } = string.Empty;

        public string FinanceAccountID { get; set; } = string.Empty;

        public string AdminID { get; set; } = string.Empty;
        /// <summary>
        /// 账户性质 公司账户 个人账户
        /// </summary>
        public AccountProperty AccountProperty { get; set; }

        public string DyjID { get; set; }
        public bool? GoodsCreStatus { get; set; }
        public string GoodsCreWord { get; set; }
        public string GoodsCreNo { get; set; }
        public string RequestID { get; set; }

        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event FinanceReceiptUpdatedHanlder Updated;
        public event DyjFinanceReceiptEnterHanlder DyjFinanceReceipt;
        public event CenterFinanceReceiptEnterHanlder CenterFinanceReceipt;

        public FinanceReceipt()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.EnterSuccess += FinanceReceipt_EnterSuccess;
            this.DyjFinanceReceipt += FinanceReceipt_PostToDyj;
            this.Updated += FinanceReceipt_Updated;
            this.AbandonSuccess += FinanceReceipt_AbandonSuccess;
            this.CenterFinanceReceipt += FinanceReceipt_PostToCenter;
        }

        public FinanceReceipt(AcceptanceBill acc) 
        {
            var oldfinanceVault = new FinanceAccountsView();
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.Payer = acc.Endorser;
            this.SeqNo = acc.Code;
            this.FeeType = FinanceFeeType.DepositReceived;
            this.ReceiptType = PaymentType.AcceptanceBill;
            this.ReceiptDate = acc.AcceptedDate.Value;
            this.Currency = "CNY";
            this.Rate = 1;
            this.Vault = new FinanceVault { ID = "FinVault20201127000001" };
            this.Amount = acc.Price;
            this.Difference = 0;
            this.Account = oldfinanceVault.Where(t => t.ID== "FinAccount20201126000001").FirstOrDefault(); 
            if (acc.ReceiveBank.Contains("华润")) 
            {
                this.Account = oldfinanceVault.Where(t => t.ID == "FinAccount20220804000002").FirstOrDefault();
            }
            this.AccountProperty = AccountProperty.Public;

            this.EnterSuccess += FinanceReceipt_EnterSuccess;
            this.DyjFinanceReceipt += FinanceReceipt_PostToDyj;
            this.Updated += FinanceReceipt_Updated;
            this.AbandonSuccess += FinanceReceipt_AbandonSuccess;
            this.CenterFinanceReceipt += FinanceReceipt_PostToCenter;

        }

        /// <summary>
        /// 持久化
        /// </summary>
        virtual public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    //主键ID（FinanceReceipt +8位年月日+6位流水号）
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinanceReceipt);
                    reponsitory.Insert(this.ToLinq());
                    this.OnEnterSuccess();                    
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    this.OnUpdated();                    
                }
            }
        }

        /// <summary>
        /// 把推送中心的方法单独出来
        /// </summary>
        public void Post2Center(string oldSeqNo)
        {
            this.OnEnter2Center(oldSeqNo);
        }

        /// <summary>
        /// Api调用，不需要传中心
        /// </summary>
        virtual public void ApiEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    //主键ID（FinanceReceipt +8位年月日+6位流水号）
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinanceReceipt);
                    reponsitory.Insert(this.ToLinq());
                    this.OnEnterSuccess();
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    this.OnUpdated();
                }
            }
        }


        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }

            if (this != null && this.DyjFinanceReceipt != null)
            {
                this.DyjFinanceReceipt(this, new DyjFinanceReceiptEnterEventArgs(this));
            }
        }

        virtual public void OnEnter2Center(string oldSeqNo)
        {
            if (this != null && this.CenterFinanceReceipt != null)
            {
                this.CenterFinanceReceipt(this, new CenterFinanceReceiptEnterEventArgs(this, oldSeqNo));
            }
        }


        virtual public void OnUpdated()
        {
            if (this != null && this.Updated != null)
            {
                //成功后触发事件
                this.Updated(this, new FinanceReceiptUpdatedEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceReceipts>(
                    new
                    {
                        Status = Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess(); ;
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        private void FinanceReceipt_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var receipt = (FinanceReceipt)e.Object;
            //将收款记录写入财务账户流水表(FinanceAccountFlows)
            var financeAccountFlows = new FinanceAccountFlow(receipt);
            financeAccountFlows.Enter();

            //更新金库账户余额
            var amount = receipt.Account.Balance + receipt.Amount;
            receipt.Account.UpdateBalance(amount);

            System.Threading.Tasks.Task.Run(() =>
            {
                //如果是“预收账款”需要生成收款通知
                if (receipt.FeeType == FinanceFeeType.DepositReceived)
                {
                    var notice = new ReceiptNotice(receipt);
                    notice.Enter();

                    FinanceReceiptToYahv financeReceiptToYahv = new FinanceReceiptToYahv(receipt);
                    financeReceiptToYahv.Execute();
                }
            });
        }

        /// <summary>
        /// 更新流水和金库账户余额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceReceipt_Updated(object sender, FinanceReceiptUpdatedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var receipt = e.FinanceReceipt;
                var flow = receipt.AccountFlow;

                //如果改为了“预收账款”需要生成收款通知
                if (receipt.FeeType == FinanceFeeType.DepositReceived)
                {
                    var notice = new ReceiptNotice(receipt);
                    notice.Enter();

                    FinanceReceiptToYahv financeReceiptToYahv = new FinanceReceiptToYahv(receipt);
                    financeReceiptToYahv.Execute();
                }

                //收款账户发生变化
                if (flow.FinanceAccount.ID != receipt.Account.ID)
                {
                    #region 更新账户余额

                    //更新原收款账户余额（减掉上次收款金额）
                    var oldAccount = flow.FinanceAccount;
                    oldAccount.Balance -= flow.Amount;
                    oldAccount.UpdateBalance(oldAccount.Balance);

                    //更新新收款账户余额（加上本次收款金额）
                    var newAccount = receipt.Account;
                    newAccount.Balance += receipt.Amount;
                    newAccount.UpdateBalance(newAccount.Balance);

                    #endregion

                    #region 更新账户流水

                    //对应流水之后的所有流水记录(按升序排列)
                    var afterCreatedFlows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                                            .Where(item => item.CreateDate > flow.CreateDate).OrderBy(item => item.CreateDate);

                    //更新对应流水之后的原账户流水
                    var accountFlows = afterCreatedFlows.Where(item => item.FinanceAccountID == flow.FinanceAccount.ID);
                    foreach (var accountFlow in accountFlows)
                    {
                        accountFlow.AccountBalance -= flow.Amount;
                        reponsitory.Update(accountFlow, item => item.ID == accountFlow.ID);
                    }

                    //TODO:有没有更好的计算余额的方式？
                    //计算对应流水的余额
                    var firstFlow = afterCreatedFlows.Where(item => item.FinanceAccountID == receipt.Account.ID).FirstOrDefault();
                    if (firstFlow == null)
                    {
                        flow.AccountBalance = receipt.Account.Balance;
                    }
                    else
                    {
                        firstFlow.AccountBalance += receipt.Amount;
                        flow.AccountBalance = firstFlow.AccountBalance - firstFlow.Amount;
                    }
                    //因为变更了收款账户，那新账户的差额实际就相当于本次的收款金额
                    receipt.Difference = receipt.Amount;
                    //更新对应流水
                    flow.Update(receipt);

                    #endregion
                }
                else
                {
                    //更新对应流水
                    flow.AccountBalance += receipt.Difference;
                    flow.Update(receipt);

                    if (receipt.Difference != 0M)
                    {
                        //更新对应账户余额
                        var account = receipt.Account;
                        account.Balance += receipt.Difference;
                        account.UpdateBalance(account.Balance);
                    }
                } 
            }
        }

        private void FinanceReceipt_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var receipt = (FinanceReceipt)e.Object;

            //删除对应的账户流水记录
            var accountFlow = new FinanceAccountFlowsView().FirstOrDefault(flow => flow.SourceID == receipt.ID);
            accountFlow?.Abandon();

            //更新金库账户余额
            var amount = receipt.Account.Balance - receipt.Amount;
            receipt.Account.UpdateBalance(amount);
        }


        /// <summary>
        /// 收款同步大赢家
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceReceipt_PostToDyj(object sender, DyjFinanceReceiptEnterEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                List<string> innerCompanies = new List<string>();
                innerCompanies.Add("杭州比一比电子科技有限公司");
                innerCompanies.Add("上海比亿电子技术有限公司");
                innerCompanies.Add("北京远大创新科技有限公司");
                innerCompanies.Add("北京艾瑞泰克电子技术有限公司");
                innerCompanies.Add("北京爱伯乐电子技术有限公司");
                innerCompanies.Add("北京奥讯达电子技术有限公司");
                innerCompanies.Add("北京北方科讯电子技术有限公司");
                innerCompanies.Add("北京创新在线电子集团有限公司");
                innerCompanies.Add("北京航天新兴科技开发有限责任公司");
                innerCompanies.Add("北京美商利华电子技术有限公司");
                innerCompanies.Add("北京欣美科电子技术有限公司");
                innerCompanies.Add("北京英赛尔科技有限公司");
                innerCompanies.Add("北京远大汇能科技有限公司");
                innerCompanies.Add("廊坊市比比商贸有限公司");
                innerCompanies.Add("深圳市佛兰德电子有限公司");
                innerCompanies.Add("深圳市科睿鑫汇供应链管理有限公司");
                innerCompanies.Add("深圳市快包电子科技有限责任公司");
                innerCompanies.Add("深圳市英赛尔电子有限公司");
                innerCompanies.Add("深圳市中电网络技术有限公司");
                innerCompanies.Add("苏州比一比电子有限公司");                

                var payer = e.DyjReceipt.Payer.Trim();
                if (e.DyjReceipt.FeeType == FinanceFeeType.DepositReceived && !innerCompanies.Contains(payer))
                {
                    var receipt = new DyjReceipt(e.DyjReceipt);
                    var data = receipt.PostDyjReceipt();
                    //收款接口返回值，保存进收款记录
                    if (!string.IsNullOrEmpty(data))
                    {
                        using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceReceipts>(new
                            {
                                DyjID = data,
                            }, item => item.ID == this.ID);
                        }
                    }
                }
            });           
        }

        /// <summary>
        /// 收款同步中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceReceipt_PostToCenter(object sender,CenterFinanceReceiptEnterEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                CenterFinanceReceipt centerFinanceReceipt = new CenterFinanceReceipt(e.CenterReceipt);

                SendStrcut sendStrcut = new SendStrcut();
                sendStrcut.sender = "FSender001";
                if (!string.IsNullOrEmpty(e.OldSeqNo))
                {
                    sendStrcut.option = CenterConstant.Update;
                    centerFinanceReceipt.OldSeqNo = e.OldSeqNo;
                }
                else
                {
                    sendStrcut.option = CenterConstant.Enter;
                }
                
                sendStrcut.model = centerFinanceReceipt;
                //提交中心
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL + FinanceApiSetting.ReceiptUrl;
                string apiclient = JsonConvert.SerializeObject(sendStrcut);

                Logs log = new Logs();
                log.Name = "收款同步";
                log.MainID = e.CenterReceipt.ID;
                log.AdminID = e.CenterReceipt.AdminID;
                log.Json = apiclient;
                log.Summary = "";
                log.Enter();

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpUtility.HttpClientHelp().HttpClient("POST", requestUrl, apiclient);               
            });
        }
    }
}
