using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Account
{
    /// <summary>
    /// 金库账户列表界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }
        protected void LoadComboBoxData()
        {
            //金库
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).Select(item => new { Value = item.ID, Text = item.Name }).Json();
            this.Model.AccountSource = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AccountSource>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string FinanceVault = Request.QueryString["FinanceVault"];
            string BankAccount = Request.QueryString["BankAccount"];
            string AccountSource = Request.QueryString["AccountSource"];
            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(FinanceVault))
            {
                FinanceVault = FinanceVault.Trim();
                financeAccounts = financeAccounts.Where(t => t.FinanceVaultID== FinanceVault);
            }
            if (!string.IsNullOrEmpty(BankAccount))
            {
                BankAccount = BankAccount.Trim();
                financeAccounts = financeAccounts.Where(t => t.BankAccount.Contains(BankAccount));
            }
            if (!string.IsNullOrEmpty(AccountSource))
            {
                AccountSource = AccountSource.Trim();
                financeAccounts = financeAccounts.Where(t => t.AccountSource== (Needs.Ccs.Services.Enums.AccountSource)Convert.ToInt32(AccountSource));
            }
            var hk_caiwu = System.Configuration.ConfigurationManager.AppSettings["HK_Caiwu"];
            if (!string.IsNullOrEmpty(hk_caiwu) && Needs.Wl.Admin.Plat.AdminPlat.Current.ID == hk_caiwu) 
            {
                financeAccounts = financeAccounts.Where(t => t.FinanceVaultName.Contains("香港"));
            }

            Func<Needs.Ccs.Services.Models.FinanceAccount, object> convert = item => new
            {
                ID = item.ID,
                FinanceVaultName=item.FinanceVaultName,
                AccountName=item.AccountName,
                BankName=item.BankName,
                BankAddress=item.BankAddress,
                BankAccount=item.BankAccount,
                SwiftCode = item.SwiftCode,
                CustomizedCode = item.CustomizedCode,
                Currency=item.Currency,
                Balance=item.Balance,
                Source = item.AccountSource
            };
            this.Paging(financeAccounts, convert);
        }
        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts[id];
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}