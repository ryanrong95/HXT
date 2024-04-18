using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt
{
    public partial class AcceptanceBillList : Uc.PageBase
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

        protected void data1()
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

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string Code = Request.QueryString["Code"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string BillStatus = Request.QueryString["BillStatus"];

            var financeAccounts = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);

            if (!string.IsNullOrEmpty(Code))
            {
                Code = Code.Trim();
                financeAccounts = financeAccounts.Where(t => t.Code == Code);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.EndDate > dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.EndDate < dtEnd);
            }
            if (!string.IsNullOrEmpty(BillStatus))
            {
                BillStatus = BillStatus.Trim();
                financeAccounts = financeAccounts.Where(t => t.BillStatus == (Needs.Ccs.Services.Enums.MoneyOrderStatus)Convert.ToInt32(BillStatus));
            }

            Func<Needs.Ccs.Services.Models.AcceptanceBill, object> convert = item => new
            {
                ID = item.ID,
                InAccountName = item.PayeeAccount.AccountName,
                Code = item.Code,
                Price = item.Price,
                OutAccountName = item.PayerAccount.AccountName,
                Endorser = item.Endorser,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                AdminName = item.Creator.ByName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            };
            this.Paging(financeAccounts, convert);
        }

    }
}
