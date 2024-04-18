using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.ApiSettings;
using Newtonsoft.Json;
using System.Net.Http;
using WebApp.App_Utils;
using System.Net;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Enums;

namespace WebApp.Finance.FundTransferApplies
{
    /// <summary>
    /// 金库账户编辑界面
    /// </summary>
    public partial class PayingEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //金库
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).Select(item => new { Value = item.ID, Text = item.Name }).Json();
            this.Model.PaymentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>()
               .Select(item => new { Value = item.Key, Text = item.Value }).Json();

        }

        protected void getAccounts()
        {
            string VaultID = Request.Form["VaultID"];
            string Currency = "CNY";
            //string IsCash = Request.Form["IsCash"];

            var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Where(t => t.FinanceVaultID == VaultID&&t.Currency== Currency);

            //if (!string.IsNullOrEmpty(IsCash) && IsCash == "true")
            //{
            //    result = result.Where(t => t.IsCash == true);
            //}
            //else
            //{
            //    result = result.Where(t => t.IsCash == false);
            //}

            if (result != null)
            {
                Response.Write(result.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            this.Model.AllData = "".Json();
            string id = Request.QueryString["ID"];
            Needs.Ccs.Services.Models.FundTransferApplies apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies[id];
            if (apply != null)
            {
                this.Model.AllData = new
                {
                    ID = id,
                    OutVault = apply.OutAccount.FinanceVaultID,
                    OutAccount = apply.OutAccount.ID,
                    OutMoney = apply.OutAmount,
                    FromSeqNo = apply.FromSeqNo,
                    InVault = apply.InAccount.FinanceVaultID,
                    InAccount = apply.InAccount.ID,
                    InMoney = apply.InAmount,
                    Summary = apply.Summary,
                }.Json();
            }
        }

      

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            var id = Request.Form["ID"];
            var OutSeqNo = Request.Form["OutSeqNo"].Trim();
            var FromSeqNo = Request.Form["FromSeqNo"].Trim();
            var DiscountInterest = Request.Form["DiscountInterest"];
            var InAmount = Request.Form["InAmount"];
            var InSeqNo = Request.Form["InSeqNo"].Trim();
            var Poundage = Request.Form["Poundage"];
            var PoundageSeqNo = Request.Form["PoundageSeqNo"].Trim();
            var PayType = Request.Form["PayType"];
            var PayDate = Request.Form["PayDate"];
            var QRCodeFee = Request.Form["QRCodeFee"];

            Needs.Ccs.Services.Models.FundTransferApplies apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies[id];

            if (!string.IsNullOrEmpty(FromSeqNo))
            {
                apply.FromSeqNo = FromSeqNo;
                var bill = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(item => item.Code == FromSeqNo).FirstOrDefault();
                if (bill == null)
                {
                    Response.Write((new { success = false, message = "承兑汇票不存在，请先维护票面信息!" }).Json());
                    return;
                }
                else
                {
                    DateTime ExchangeDate = Convert.ToDateTime(PayDate);
                    decimal ExchangePrice = Convert.ToDecimal(InAmount);
                    bill.UpdateBillStatus(ExchangeDate, ExchangePrice);
                }
            }
            else
            {
                apply.FromSeqNo = "";
            }

            apply.OutSeqNo = OutSeqNo;
            if (!string.IsNullOrEmpty(DiscountInterest))
            {
                apply.DiscountInterest = Convert.ToDecimal(DiscountInterest);
            }
            apply.InAmount = Convert.ToDecimal(InAmount);
            apply.InSeqNo = InSeqNo;

            if (!string.IsNullOrEmpty(Poundage))
            {
                apply.Poundage = Convert.ToDecimal(Poundage);
            }
            apply.PoundageSeqNo = PoundageSeqNo;

            if (!string.IsNullOrEmpty(QRCodeFee))
            {
                apply.QRCodeFee = Convert.ToDecimal(QRCodeFee);
            }

            apply.PaymentType = (Needs.Ccs.Services.Enums.PaymentType)Convert.ToInt16(PayType);
            apply.PaymentDate = Convert.ToDateTime(PayDate);
            apply.ApplyStatus = Needs.Ccs.Services.Enums.FundTransferApplyStatus.Done;

            apply.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);           

            apply.EnterSuccess += FinanceVault_EnterSuccess;
            apply.EnterError += FinanceVault_EnterError;
            apply.Post2Center += PostTransfer2Center;
            apply.TransferCompleted += Flow;
            apply.Enter();

            apply.Log("财务【"+ Needs.Wl.Admin.Plat.AdminPlat.Current.RealName+"】完成了资金调拨付款，调入金额："+apply.InAmount);
                 
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceVault_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceVault_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }


        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var list = new Needs.Ccs.Services.Views.LogsView().Where(t => t.MainID == ID).OrderByDescending(t=>t.CreateDate).AsQueryable();
            Func<Needs.Ccs.Services.Models.Logs, object> convert = item => new
            {
                ID = item.ID,               
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary
            };
            Response.Write(new { rows = list.Select(convert).ToArray(), }.Json());
        }

        private void Flow(object sender, FinanceTransferApplyEventArgs e)
        {
            Needs.Ccs.Services.Models.FundTransferApplies apply = e.FinanceTransferApply;
            try
            {
                DateTime payDate = apply.PaymentDate == null ? DateTime.Now : apply.PaymentDate.Value;
                if (apply.QRCodeFee != null)
                {
                    //如果快捷支付手续费不为0，付款分两笔
                   
                    // 一笔是实际付到公司账上的钱
                    FinancePayment payOut = new FinancePayment();
                    payOut.SeqNo = apply.OutSeqNo;
                    payOut.PayeeName = apply.OutAccount.AccountName;
                    payOut.BankName = apply.OutAccount.BankName;
                    payOut.BankAccount = apply.OutAccount.BankAccount;
                    payOut.Payer = apply.Admin;
                    payOut.PayFeeType = FinanceFeeType.FundTransfer;
                    payOut.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    payOut.FinanceAccount = apply.OutAccount;
                    payOut.Amount = apply.OutAmount - apply.QRCodeFee.Value;
                    payOut.Currency = "CNY";
                    payOut.ExchangeRate = 1.0M;
                    payOut.PayType = apply.PaymentType;
                    payOut.PayDate = payDate;
                    payOut.Enter();

                    //一笔是 快捷支付手续费
                    FinancePayment payOut2 = new FinancePayment();
                    payOut2.SeqNo = "QRFee"+apply.OutSeqNo;
                    payOut2.PayeeName = apply.OutAccount.AccountName;
                    payOut2.BankName = apply.OutAccount.BankName;
                    payOut2.BankAccount = apply.OutAccount.BankAccount;
                    payOut2.Payer = apply.Admin;
                    payOut2.PayFeeType = FinanceFeeType.FundTransfer;
                    payOut2.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    apply.OutAccount.Balance = apply.OutAccount.Balance - apply.OutAmount + apply.QRCodeFee.Value;
                    payOut2.FinanceAccount = apply.OutAccount;
                    payOut2.Amount = apply.QRCodeFee.Value;
                    payOut2.Currency = "CNY";
                    payOut2.ExchangeRate = 1.0M;
                    payOut2.PayType = apply.PaymentType;
                    payOut2.PayDate = payDate;
                    payOut2.Enter();
                }
                else if (apply.DiscountInterest == null||apply.DiscountInterest==0)
                {
                    ///付款
                    FinancePayment payOut = new FinancePayment();
                    payOut.SeqNo = apply.OutSeqNo;
                    payOut.PayeeName = apply.OutAccount.AccountName;
                    payOut.BankName = apply.OutAccount.BankName;
                    payOut.BankAccount = apply.OutAccount.BankAccount;
                    payOut.Payer = apply.Admin;
                    payOut.PayFeeType = FinanceFeeType.FundTransfer;
                    payOut.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    payOut.FinanceAccount = apply.OutAccount;
                    payOut.Amount = apply.OutAmount;
                    payOut.Currency = "CNY";
                    payOut.ExchangeRate = 1.0M;
                    payOut.PayType = apply.PaymentType;
                    payOut.PayDate = payDate;
                    payOut.Enter();
                }               
                else
                {
                    //如果贴现利息不为0，付款分两笔
                    FinancePayment payOut = new FinancePayment();
                    payOut.SeqNo = apply.OutSeqNo;
                    payOut.PayeeName = apply.OutAccount.AccountName;
                    payOut.BankName = apply.OutAccount.BankName;
                    payOut.BankAccount = apply.OutAccount.BankAccount;
                    payOut.Payer = apply.Admin;
                    payOut.PayFeeType = FinanceFeeType.FundTransfer;
                    payOut.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    payOut.FinanceAccount = apply.OutAccount;
                    payOut.Amount = apply.OutAmount-apply.DiscountInterest.Value;
                    payOut.Currency = "CNY";
                    payOut.ExchangeRate = 1.0M;
                    payOut.PayType = apply.PaymentType;
                    payOut.PayDate = payDate;
                    payOut.Enter();

                    FinancePayment payOut2 = new FinancePayment();
                    payOut2.SeqNo = apply.FromSeqNo;
                    payOut2.PayeeName = apply.OutAccount.AccountName;
                    payOut2.BankName = apply.OutAccount.BankName;
                    payOut2.BankAccount = apply.OutAccount.BankAccount;
                    payOut2.Payer = apply.Admin;
                    payOut2.PayFeeType = FinanceFeeType.FundTransfer;
                    payOut2.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    apply.OutAccount.Balance = apply.OutAccount.Balance - apply.OutAmount + apply.DiscountInterest.Value;
                    payOut2.FinanceAccount = apply.OutAccount;
                    payOut2.Amount = apply.DiscountInterest.Value;
                    payOut2.Currency = "CNY";
                    payOut2.ExchangeRate = 1.0M;
                    payOut2.PayType = apply.PaymentType;
                    payOut2.PayDate = payDate;
                    payOut2.Enter();
                }
                

                ///手续费
                if (apply.Poundage != 0&&apply.Poundage!=null)
                {
                    FinancePayment Poundage = new FinancePayment();
                    Poundage.SeqNo = apply.PoundageSeqNo;
                    Poundage.PayeeName = apply.OutAccount.AccountName;
                    Poundage.Payer = apply.Admin;
                    Poundage.PayFeeType = FinanceFeeType.Poundage;
                    Poundage.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    apply.OutAccount.Balance -= apply.OutAmount;
                    Poundage.FinanceAccount = apply.OutAccount;
                    Poundage.BankName = apply.OutAccount.BankName;
                    Poundage.BankAccount = apply.OutAccount.BankAccount;
                    Poundage.Amount = apply.Poundage.Value;
                    Poundage.Currency = "CNY";
                    Poundage.ExchangeRate = 1.0M;
                    Poundage.PayType = apply.PaymentType;
                    Poundage.PayDate = payDate;
                    Poundage.Enter();
                }

                ///收款
                FinanceReceipt recIn = new FinanceReceipt();
                recIn.SeqNo = apply.InSeqNo;
                recIn.Payer = apply.OutAccount.AccountName;
                recIn.FeeType = FinanceFeeType.FundTransfer;
                recIn.ReceiptType = apply.PaymentType;
                recIn.ReceiptDate = DateTime.Now;
                recIn.Currency = "CNY";
                recIn.Rate = 1.0M;
                recIn.Amount = apply.InAmount;
                recIn.Vault = new FinanceVault { ID = apply.InAccount.FinanceVaultID };
                recIn.Account = apply.InAccount;
                recIn.Admin = apply.Admin;
                recIn.Enter();
            }
            catch(Exception EX)
            {

            }
           
        }

        public void PostTransfer2Center(object sender, FinanceTransferApplyEventArgs e)
        {
            try
            {
                CenterFundTransfer centerFee = new CenterFundTransfer(e.FinanceTransferApply);

                SendStrcut sendStrcut = new SendStrcut();
                sendStrcut.sender = "FSender001";
                sendStrcut.option = CenterConstant.Enter;
                sendStrcut.model = centerFee;
                //提交中心
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL;
                if (e.FinanceTransferApply.FromSeqNo != null && e.FinanceTransferApply.FromSeqNo != "")
                {
                    requestUrl += FinanceApiSetting.AcceptanceCharge;
                }
                else
                {
                    requestUrl += FinanceApiSetting.FundTransferUrl;
                }
                string apiclient = JsonConvert.SerializeObject(sendStrcut);

                Logs log = new Logs();
                log.Name = "资金调拨同步";
                log.MainID = e.FinanceTransferApply.ID;
                log.AdminID = e.FinanceTransferApply.Admin.ID;
                log.Json = apiclient;
                log.Summary = "";
                log.Enter();

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);
            }
            catch(Exception ex)
            {

            }
            
        }
    }
}