using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment.TaxPayment
{
    public partial class Pay : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string DecTaxFlowID = Request.QueryString["DecTaxFlowID"];
            string Amount = Request.QueryString["Amount"];
            string TaxTypeInt = Request.QueryString["TaxTypeInt"];

            this.Model.DecTaxFlowID = DecTaxFlowID;
            this.Model.Amount = Amount;
            this.Model.TaxTypeInt = TaxTypeInt;


            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();

        }

        /// <summary>
        /// 付款
        /// </summary>
        protected void OnePay()
        {
            try
            {
                string DecTaxFlowID = Request.Form["DecTaxFlowID"];
                string PayDate = Request.Form["PayDate"];
                string PayeeName = Request.Form["PayeeName"];
                string BankName = Request.Form["BankName"];
                string BankAccount = Request.Form["BankAccount"];
                string FinanceVaultID = Request.Form["FinanceVaultID"];
                string FinanceAccountID = Request.Form["FinanceAccountID"];

                string AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                DateTime payDateDt = DateTime.Parse(PayDate);
                Needs.Ccs.Services.Models.TaxPayHandler taxPayHandler = new Needs.Ccs.Services.Models.TaxPayHandler(
                    AdminID, DecTaxFlowID, payDateDt, PayeeName, BankName, BankAccount, FinanceVaultID, FinanceAccountID);
                taxPayHandler.Execute();

                Response.Write((new { success = true, message = "操作成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }
        }

        protected void GetAccounts()
        {
            string VaultID = Request.Form["VaultID"];
            //string Currency = Request.Form["Currency"];

            var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (string.IsNullOrEmpty(VaultID) == false)
            {
                result = result.Where(t => t.FinanceVaultID == VaultID);
            }
            //if (string.IsNullOrEmpty(Currency) == false)
            //{
            //    result = result.Where(item => item.Currency == Currency);
            //}

            //这里是给海关缴税, 币种是人民币
            result = result.Where(item => item.Currency == Needs.Ccs.Services.Enums.Currency.CNY.ToString());

            if (result != null)
            {
                Response.Write(result.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
            }
        }

    }
}