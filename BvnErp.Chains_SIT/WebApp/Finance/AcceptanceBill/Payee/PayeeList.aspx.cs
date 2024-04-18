using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.AcceptanceBill
{
    public partial class PayeeList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string From = Request.QueryString["From"];
            From = From ?? string.Empty;
            this.Model.From = From;
            string WindowName = Request.QueryString["WindowName"];
            WindowName = WindowName ?? string.Empty;
            this.Model.WindowName = WindowName;
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string AccountName = Request.QueryString["AccountName"];
            string BankAccount = Request.QueryString["BankAccount"];

            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.AsQueryable()
                .Where(v => v.Status == Status.Normal && v.BankAccount != null);

            if (!string.IsNullOrEmpty(AccountName))
            {
                financeAccounts = financeAccounts.Where(t => t.AccountName.Contains(AccountName));
            }
            if (!string.IsNullOrEmpty(BankAccount))
            {
                financeAccounts = financeAccounts.Where(t => t.BankAccount.Contains(BankAccount));
            }

            Func<Needs.Ccs.Services.Models.FinanceAccount, object> convert = item => new
            {
                ID = item.ID,               
                AccountName = item.AccountName,
                BankName = item.BankName,               
                BankAccount = item.BankAccount,            
            };
            this.Paging(financeAccounts, convert);

        }

        protected void Delete()
        {
            try
            {
                string CostApplyPayeeID = Request.Form["CostApplyPayeeID"];

              

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

    }
}