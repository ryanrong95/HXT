using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class AccImportQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            this.Model.BillStatus = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.MoneyOrderStatus>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data()
        {
            string Code = Request.QueryString["Code"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string BillStatus = Request.QueryString["BillStatus"];
            string CreateStartDate = Request.QueryString["CreateStartDate"];
            string CreateEndDate = Request.QueryString["CreateEndDate"];

            var financeAccounts = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal  && item.AccCreSta == true);

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
            if (!string.IsNullOrEmpty(CreateStartDate))
            {
                CreateStartDate = CreateStartDate.Trim();
                DateTime dtCreateStart = Convert.ToDateTime(CreateStartDate);
                financeAccounts = financeAccounts.Where(t => t.CreateDate > dtCreateStart);
            }
            if (!string.IsNullOrEmpty(CreateEndDate))
            {
                CreateEndDate = CreateEndDate.Trim();
                DateTime dtCreateEnd = Convert.ToDateTime(CreateEndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.CreateDate < dtCreateEnd);
            }

            Func<Needs.Ccs.Services.Models.AcceptanceBill, object> convert = item => new
            {
                ID = item.ID,
                Code = item.Code,
                OutAccountName = item.PayerAccount.AccountName,
                Price = item.Price,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                BankName = item.BankName,
                ExchangeDate = item.ExchangeDate == null ? "" : item.ExchangeDate.Value.ToString("yyyy-MM-dd"),
                Interest = item.ExchangePrice == null ? 0 : item.Price - item.ExchangePrice,
                item.AccCreSta,
                item.BuyCreSta,
                item.Endorser,
                item.AccCreWord,
                item.AccCreNo,
                item.ReceiveBank,
                item.RequestID
            };
            this.Paging(financeAccounts, convert);
        }
    }
}