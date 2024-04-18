using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class GoodsImport : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();

            this.Model.FinanceAccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.AccountSource == AccountSource.standard)
                .Select(item => new { Value = item.ID, Text = item.AccountName }).Json();
        }

        /// <summary>
        /// 根据金库显示账户
        /// </summary>
        /// <returns></returns>
        protected object GetAccountByVault()
        {
            var vault = Request.Form["FinanceVault"];
            return new
            {
                data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(v => v.Status == Needs.Ccs.Services.Enums.Status.Normal && v.FinanceVaultID == vault).Select(item => new
                {
                    Value = item.ID,
                    Text = item.AccountName
                }).Json()
            };
        }

        protected void data()
        {          
            string Payer = Request.QueryString["Payer"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string FinanceVault = Request.QueryString["FinanceVault"];
            string Account = Request.QueryString["Account"];

            var receipts = new Needs.Ccs.Services.Views.GoodsImportView()
                .Where(item => item.Status == Status.Normal&&item.FeeType== FinanceFeeType.DepositReceived&&item.GoodsCreStatus==false && item.Vault.Name != "承兑金库")
                .OrderByDescending(item => item.ReceiptDate)
                .AsQueryable();


            if (!string.IsNullOrEmpty(Payer))
            {
                var payer = Payer.Trim();
                receipts = receipts.Where(t => t.Payer == payer);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                receipts = receipts.Where(t => t.ReceiptDate >= Convert.ToDateTime(StartDate));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var endDate = Convert.ToDateTime(EndDate).AddDays(1);
                receipts = receipts.Where(t => t.ReceiptDate < endDate);
            }
            if (!string.IsNullOrEmpty(FinanceVault))
            {
                var financeVault = FinanceVault.Trim();
                receipts = receipts.Where(t => t.Vault.ID == financeVault);
            }
            if (!string.IsNullOrEmpty(Account))
            {
                var account = Account.Trim();
                receipts = receipts.Where(t => t.Account.ID == account);
            }

            Func<Needs.Ccs.Services.Models.FinanceReceipt, object> convert = receipt => new
            {
                receipt.ID,
                SerialNumber = receipt.SeqNo,
                Vault = receipt.Vault.Name,
                Account = receipt.Account.AccountName,
                Payer = receipt.Payer,
                FeeType = receipt.FeeType.GetDescription(),
                ReceiptType = receipt.ReceiptType.GetDescription(),
                Amount = receipt.Amount,
                ExchangeRate = receipt.Rate,
                Currency = receipt.Currency,
                ReceiptDate = receipt.ReceiptDate.ToShortDateString(),
                BankName = receipt.Account.BankName,
            };

            this.Paging(receipts, convert);
        }

        /// <summary>
        /// 生成凭证
        /// </summary>
        protected void MakeAccount()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<GoodsImportItem>>();

            var result = new Needs.Ccs.Services.Models.GoodsImport(model).Make();
            Response.Write((new { success = result }).Json());
        }


        protected void MakeAccountAll()
        {

            string Payer = Request.Form["Payer"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string FinanceVault = Request.Form["FinanceVault"];
            string Account = Request.Form["Account"];

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime from = DateTime.Parse(StartDate);
                DateTime to = DateTime.Parse(EndDate).AddDays(1);
                TimeSpan day = to.Subtract(from);
                if (day.TotalDays > 31)
                {
                    Response.Write((new { success = false, msg = "不能一次生成超一个月的数据" }).Json());
                    return;
                }
            }
            else
            {
                Response.Write((new { success = false, msg = "必须勾选开始结束日期" }).Json());
                return;
            }

            var receipts = new Needs.Ccs.Services.Views.GoodsImportView()
              .Where(item => item.Status == Status.Normal && item.FeeType == FinanceFeeType.DepositReceived && item.GoodsCreStatus == false && item.Vault.Name != "承兑金库")
              .OrderByDescending(item => item.ReceiptDate)
              .AsQueryable();


            if (!string.IsNullOrEmpty(Payer))
            {
                var payer = Payer.Trim();
                receipts = receipts.Where(t => t.Payer == payer);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                receipts = receipts.Where(t => t.ReceiptDate >= Convert.ToDateTime(StartDate));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var endDate = Convert.ToDateTime(EndDate).AddDays(1);
                receipts = receipts.Where(t => t.ReceiptDate < endDate);
            }
            if (!string.IsNullOrEmpty(FinanceVault))
            {
                var financeVault = FinanceVault.Trim();
                receipts = receipts.Where(t => t.Vault.ID == financeVault);
            }
            if (!string.IsNullOrEmpty(Account))
            {
                var account = Account.Trim();
                receipts = receipts.Where(t => t.Account.ID == account);
            }

            List<GoodsImportItem> model = new List<GoodsImportItem>();          
            foreach (var t in receipts)
            {              
                model.Add(new GoodsImportItem
                {
                    ID = t.ID,
                    Amount = t.Amount,
                    Account = t.Account.AccountName,
                    Payer = t.Payer,
                    ReceiptDate = t.ReceiptDate.ToString("yyyy-MM-dd")
                });
            }

            var result = new Needs.Ccs.Services.Models.GoodsImport(model).Make();
            Response.Write((new { success = result }).Json());
        }
    }
}