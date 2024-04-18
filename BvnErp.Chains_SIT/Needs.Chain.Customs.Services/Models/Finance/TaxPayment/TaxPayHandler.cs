using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TaxPayHandler
    {
        /// <summary>
        /// AdminID
        /// </summary>
        private string AdminID { get; set; }

        /// <summary>
        /// DecTaxFlowID
        /// </summary>
        private string DecTaxFlowID { get; set; }

        /// <summary>
        /// 扣款日期
        /// </summary>
        private DateTime PayDate { get; set; }
        
        /// <summary>
        /// 收款方
        /// </summary>
        private string PayeeName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        private string BankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        private string BankAccount { get; set; }

        /// <summary>
        /// FinanceVaultID
        /// </summary>
        private string FinanceVaultID { get; set; }

        /// <summary>
        /// FinanceAccountID
        /// </summary>
        private string FinanceAccountID { get; set; }


        public TaxPayHandler(string adminID, 
                             string decTaxFlowID, 
                             DateTime payDate, 
                             string payeeName, 
                             string bankName, 
                             string bankAccount, 
                             string financeVaultID, 
                             string financeAccountID)
        {
            this.AdminID = adminID;
            this.DecTaxFlowID = decTaxFlowID;
            this.PayDate = payDate;
            this.PayeeName = payeeName;
            this.BankName = bankName;
            this.BankAccount = bankAccount;
            this.FinanceVaultID = financeVaultID;
            this.FinanceAccountID = financeAccountID;
        }

        
        public void Execute()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var flow = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.ID == this.DecTaxFlowID).FirstOrDefault();

                if (flow.PayDate != null)
                {
                    return;
                }

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(new
                {
                    Status = (int)Enums.DecTaxStatus.Paid,
                    PayDate = this.PayDate,
                    BankName = this.BankName
                }, item => item.ID == this.DecTaxFlowID);

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                {
                    Status = (int)Enums.DecTaxStatus.Paid,
                    IsUpload = (int)Enums.UploadStatus.Uploaded,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == flow.DecTaxID);

                //财务上传缴税流水后，海关税费额度预警表， 缴费状态改为1；
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxQuotas>(new
                {
                    PayStatus = (int)Enums.TaxStatus.Paid,
                    UpdateDate = DateTime.Now,
                }, item => item.DeclarationID == flow.DecTaxID);

                //新增 FinancePayments 表数据 Begin
                var financePayment = new Layer.Data.Sqls.ScCustoms.FinancePayments();
                financePayment.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinancePayment);
                financePayment.SeqNo = "";
                financePayment.PayeeName = this.PayeeName;
                financePayment.BankName = this.BankName;
                financePayment.BankAccount = this.BankAccount;
                financePayment.PayerID = this.AdminID;

                if (flow.TaxType == (int)Enums.DecTaxType.Tariff)
                {
                    financePayment.FeeType = (int)Enums.FinanceFeeType.Tariff;
                }
                else if (flow.TaxType == (int)Enums.DecTaxType.AddedValueTax)
                {
                    financePayment.FeeType = (int)Enums.FinanceFeeType.AddedValueTax;
                }

                financePayment.FinanceVaultID = this.FinanceVaultID;
                financePayment.FinanceAccountID = this.FinanceAccountID;
                financePayment.Amount = flow.Amount;
                financePayment.Currency = Needs.Ccs.Services.Enums.Currency.CNY.ToString();
                financePayment.ExchangeRate = (decimal)1;
                financePayment.PayType = (int)Enums.PaymentType.TransferAccount;
                financePayment.PayDate = this.PayDate;
                financePayment.Status = (int)Enums.Status.Normal;
                financePayment.CreateDate = DateTime.Now;
                financePayment.UpdateDate = DateTime.Now;
                financePayment.Summary = "海关税费支付|" + flow.TaxNumber;

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.FinancePayments>(financePayment);

                //新增 FinancePayments 表数据 End

                var financeAccount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>().Where(t => t.ID == this.FinanceAccountID).FirstOrDefault();

                //新增 FinanceAccountFlows 表数据 Begin

                var financeAccountFlows = new Layer.Data.Sqls.ScCustoms.FinanceAccountFlows();
                financeAccountFlows.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinanceAccountFlow);
                financeAccountFlows.AdminID = financePayment.PayerID;
                financeAccountFlows.SeqNo = financePayment.SeqNo;
                financeAccountFlows.SourceID = financePayment.ID;
                financeAccountFlows.FinanceVaultID = financePayment.FinanceVaultID;
                financeAccountFlows.FinanceAccountID = financePayment.FinanceAccountID;
                financeAccountFlows.Type = (int)Enums.FinanceType.Payment;
                financeAccountFlows.FeeType = financePayment.FeeType;
                financeAccountFlows.PaymentType = financePayment.PayType;
                financeAccountFlows.Amount = financePayment.Amount;
                financeAccountFlows.Currency = financePayment.Currency;
                financeAccountFlows.AccountBalance = financeAccount.Balance - financePayment.Amount;
                financeAccountFlows.Status = (int)Enums.Status.Normal;
                financeAccountFlows.CreateDate = DateTime.Now;
                financeAccountFlows.UpdateDate = DateTime.Now;

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(financeAccountFlows);

                //新增 FinanceAccountFlows 表数据 End

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                new
                {
                    UpdateDate = DateTime.Now,
                    Balance = financeAccount.Balance - financePayment.Amount
                }, item => item.ID == this.FinanceAccountID);


            }
        }

    }

    public class TaxPaiesHandler
    {
        /// <summary>
        /// AdminID
        /// </summary>
        private string AdminID { get; set; }

        /// <summary>
        /// DecTaxFlowIDs
        /// </summary>
        private string[] DecTaxFlowIDs { get; set; }

        /// <summary>
        /// 扣款日期
        /// </summary>
        private DateTime PayDate { get; set; }

        /// <summary>
        /// 收款方
        /// </summary>
        private string PayeeName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        private string BankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        private string BankAccount { get; set; }

        /// <summary>
        /// FinanceVaultID
        /// </summary>
        private string FinanceVaultID { get; set; }

        /// <summary>
        /// FinanceAccountID
        /// </summary>
        private string FinanceAccountID { get; set; }

        public TaxPaiesHandler(string adminID, 
                               string[] decTaxFlowIDs,
                               DateTime payDate,
                               string payeeName,
                               string bankName,
                               string bankAccount,
                               string financeVaultID,
                               string financeAccountID)
        {
            this.AdminID = adminID;
            this.DecTaxFlowIDs = decTaxFlowIDs;
            this.PayDate = payDate;
            this.PayeeName = payeeName;
            this.BankName = bankName;
            this.BankAccount = bankAccount;
            this.FinanceVaultID = financeVaultID;
            this.FinanceAccountID = financeAccountID;
        }

        public void Execute()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var flows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => this.DecTaxFlowIDs.Contains(t.ID)).ToList();

                foreach (var flow in flows)
                {
                    if (flow.PayDate != null)
                    {
                        continue;
                    }

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(new
                    {
                        Status = (int)Enums.DecTaxStatus.Paid,
                        PayDate = this.PayDate,
                        BankName = this.BankName
                    }, item => item.ID == flow.ID);

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                    {
                        Status = (int)Enums.DecTaxStatus.Paid,
                        IsUpload = (int)Enums.UploadStatus.Uploaded,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == flow.DecTaxID);

                    //财务上传缴税流水后，海关税费额度预警表， 缴费状态改为1；
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxQuotas>(new
                    {
                        PayStatus = (int)Enums.TaxStatus.Paid,
                        UpdateDate = DateTime.Now,
                    }, item => item.DeclarationID == flow.DecTaxID);


                    //新增 FinancePayments 表数据 Begin
                    var financePayment = new Layer.Data.Sqls.ScCustoms.FinancePayments();
                    financePayment.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinancePayment);
                    financePayment.SeqNo = "";
                    financePayment.PayeeName = this.PayeeName;
                    financePayment.BankName = this.BankName;
                    financePayment.BankAccount = this.BankAccount;
                    financePayment.PayerID = this.AdminID;

                    if (flow.TaxType == (int)Enums.DecTaxType.Tariff)
                    {
                        financePayment.FeeType = (int)Enums.FinanceFeeType.Tariff;
                    }
                    else if (flow.TaxType == (int)Enums.DecTaxType.AddedValueTax)
                    {
                        financePayment.FeeType = (int)Enums.FinanceFeeType.AddedValueTax;
                    }

                    financePayment.FinanceVaultID = this.FinanceVaultID;
                    financePayment.FinanceAccountID = this.FinanceAccountID;
                    financePayment.Amount = flow.Amount;
                    financePayment.Currency = Needs.Ccs.Services.Enums.Currency.CNY.ToString();
                    financePayment.ExchangeRate = (decimal)1;
                    financePayment.PayType = (int)Enums.PaymentType.TransferAccount;
                    financePayment.PayDate = this.PayDate;
                    financePayment.Status = (int)Enums.Status.Normal;
                    financePayment.CreateDate = DateTime.Now;
                    financePayment.UpdateDate = DateTime.Now;
                    financePayment.Summary = "海关税费支付|" + flow.TaxNumber;

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.FinancePayments>(financePayment);

                    //新增 FinancePayments 表数据 End

                    var financeAccount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>().Where(t => t.ID == this.FinanceAccountID).FirstOrDefault();

                    //新增 FinanceAccountFlows 表数据 Begin

                    var financeAccountFlows = new Layer.Data.Sqls.ScCustoms.FinanceAccountFlows();
                    financeAccountFlows.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinanceAccountFlow);
                    financeAccountFlows.AdminID = financePayment.PayerID;
                    financeAccountFlows.SeqNo = financePayment.SeqNo;
                    financeAccountFlows.SourceID = financePayment.ID;
                    financeAccountFlows.FinanceVaultID = financePayment.FinanceVaultID;
                    financeAccountFlows.FinanceAccountID = financePayment.FinanceAccountID;
                    financeAccountFlows.Type = (int)Enums.FinanceType.Payment;
                    financeAccountFlows.FeeType = financePayment.FeeType;
                    financeAccountFlows.PaymentType = financePayment.PayType;
                    financeAccountFlows.Amount = financePayment.Amount;
                    financeAccountFlows.Currency = financePayment.Currency;
                    financeAccountFlows.AccountBalance = financeAccount.Balance - financePayment.Amount;
                    financeAccountFlows.Status = (int)Enums.Status.Normal;
                    financeAccountFlows.CreateDate = DateTime.Now;
                    financeAccountFlows.UpdateDate = DateTime.Now;

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(financeAccountFlows);

                    //新增 FinanceAccountFlows 表数据 End

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Balance = financeAccount.Balance - financePayment.Amount
                    }, item => item.ID == this.FinanceAccountID);




                }
            }
        }

    }

}
