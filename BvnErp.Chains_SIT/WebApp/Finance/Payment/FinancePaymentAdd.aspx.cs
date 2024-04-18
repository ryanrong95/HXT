using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment
{
    public partial class FinancePaymentAdd : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadData();
            }
        }

        protected void LoadData()
        {

            //付款人
            this.Model.PayerData = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                .Where(manager => manager.Role.Name == "集团财务出纳").Select(item => new { Value = item.Admin.ID, Text = item.Admin.ByName }).Json();

            this.Model.PaymentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
            //this.Model.PayFeeTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>()
            //    .Select(item => new { Value = item.Key, Text = item.Value }).Json();
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();
            this.Model.CurrencyData = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.Currency>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();

            this.Model.FinancePaymentData = "".Json();
            this.Model.FinanceAccountData = "".Json();
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var finance = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinancePayment.Where(item => item.ID == ID).FirstOrDefault();
            if (finance != null)
            {
                this.Model.FinancePaymentData = new
                {
                    SeqNo = finance.SeqNo,
                    Payer = finance.Payer.ID,
                    FinanceVault = finance.FinanceVault.ID,
                    FinanceAccount = finance.FinanceAccount.ID,
                    PayFeeType = finance.PayFeeType,
                    PayeeName = finance.PayeeName,
                    BankName = finance.BankName,
                    BankAccount = finance.BankAccount,
                    Amount = finance.Amount.ToRound(2),
                    Currency = finance.Currency,
                    ExchangeRate = finance.ExchangeRate,
                    PayDate = finance.PayDate,
                    PayType = finance.PayType,
                }.Json();

                this.Model.FinanceAccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                    .Where(item => item.FinanceVaultID == finance.FinanceVault.ID).Select(item => new { Value = item.ID, Text = item.AccountName }).Json();
            }
        }

        protected void GetAccounts()
        {
            string VaultID = Request.Form["VaultID"];
            string Currency = Request.Form["Currency"];

            var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (string.IsNullOrEmpty(VaultID) == false)
            {
                result = result.Where(t => t.FinanceVaultID == VaultID);
            }
            if (string.IsNullOrEmpty(Currency) == false)
            {
                result = result.Where(item => item.Currency == Currency);
            }

            if (result != null)
            {
                Response.Write(result.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
            }
        }

        protected void GetExchangeRate()
        {
            string Currency = Request.Form["Currency"];
            var exchangeRate = Needs.Wl.Admin.Plat.AdminPlat.RealTimeRates
                .Where(item => item.Code == Currency).FirstOrDefault()?.Rate;
            Response.Write((new { ExchangeRate = exchangeRate }).Json());
        }

        protected void SavePaymentNotice()
        {
            try
            {

                var accountCata = Request.Form["AccountCatalog"];
                FinanceFeeType feeType = FeeTypeTransfer.Current.C2LOutTransfer(accountCata);

                var PayeeName = Request.Form["PayeeName"];
                //var PayFeeType = Request.Form["PayFeeType"];
                var BankAccount = Request.Form["BankAccount"];
                var BankName = Request.Form["BankName"];
                var Amount = Request.Form["Amount"];
                var PayType = Request.Form["PayType"];
                var Currency = Request.Form["Currency"];
                var ExchangeRate = Request.Form["ExchangeRate"];
                var Payer = Request.Form["Payer"];
                var PayDate = Request.Form["PayDate"];
                var FinanceVault = Request.Form["FinanceVault"];
                var FinanceAccount = Request.Form["FinanceAccount"];
                var SeqNo = Request.Form["SeqNo"];

                string ID = Request.Form["ID"];
                FinancePayment finance = new FinancePayment();
                if (!string.IsNullOrEmpty(ID))
                {
                    finance = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinancePayment.Where(item => item.ID == ID).FirstOrDefault();
                    if (finance == null)
                    {
                        throw new Exception("付款对象为空");
                    }
                    else
                    {
                        finance.Difference = decimal.Parse(Amount) - finance.Amount;
                    }
                }

                var ErmAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(t => t.OriginID == Payer)?.ErmAdminID;

                finance.SeqNo = SeqNo;
                finance.Payer = Needs.Underly.FkoFactory<Admin>.Create(Payer);
                finance.Payer.ErmAdminID = ErmAdminID;
                finance.FinanceVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault[FinanceVault];
                finance.FinanceAccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[FinanceAccount];
                finance.PayFeeType = feeType;
                finance.PayeeName = PayeeName;
                finance.BankName = BankName;
                finance.BankAccount = BankAccount;
                finance.Amount = decimal.Parse(Amount);
                finance.Currency = Currency;
                finance.ExchangeRate = decimal.Parse(ExchangeRate);
                finance.PayDate = Convert.ToDateTime(PayDate);
                finance.PayType = (Needs.Ccs.Services.Enums.PaymentType)int.Parse(PayType);
                finance.EnterSuccess += FinancePayment_EnterSuccess;
                finance.EnterError += FinancePayment_EnterError;
                finance.UpdateSuccess += FinancePayment_UpdateSuccess;
                finance.Enter();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinancePayment_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinancePayment_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinancePayment_UpdateSuccess(object sender, FinancePaymentUpdateEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        protected object AccountCatalogsTree()
        {

            var treeStr = AccountCatalogsAlls.Current.JsonOut(AccountCatalogType.Output.GetDescription());  //new AccountCatalogsTree().Json();

            treeStr = treeStr.Replace("\"name\":", "\"text\":");
            return treeStr;
        }

        /// <summary>
        /// 根据金库显示账户
        /// </summary>
        /// <returns></returns>
        protected object GetAccountByVault()
        {
            return new
            {
                data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(v => v.AccountSource == AccountSource.easy && v.BankAccount != null).Select(item => new
                {
                    item.ID,
                    item.AccountName,
                    item.BankAccount,
                    item.BankName,
                }).Distinct()
            };
        }

    }
}
