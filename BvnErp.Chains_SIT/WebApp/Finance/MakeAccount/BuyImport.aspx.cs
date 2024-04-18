using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class BuyImport : Uc.PageBase
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

            var financeAccounts = new Needs.Ccs.Services.Views.BuyImportView().
                Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.ExchangeDate != null &&  item.BuyCreSta == false);

            if (!string.IsNullOrEmpty(Code))
            {
                Code = Code.Trim();
                financeAccounts = financeAccounts.Where(t => t.Code == Code);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.ExchangeDate > dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.ExchangeDate < dtEnd);
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

            financeAccounts = financeAccounts.OrderByDescending(t => t.CreateDate);

            Func<Needs.Ccs.Services.Models.AcceptanceBill, object> convert = item => new
            {
                ID = item.ID,
                Code = item.Code,
                OutAccountName = item.PayerAccount.AccountName,
                Price = item.Price,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                BankName = item.BankName,
                FundTransferAccountName = item.FundTransferAccountName,
                ExchangeDate = item.ExchangeDate == null ? "" : item.ExchangeDate.Value.ToString("yyyy-MM-dd"),
                Interest = item.ExchangePrice == null ? 0 : item.Price - item.ExchangePrice,
                AccCreSta = item.AccCreSta == true ? "是" : "否",
                BuyCreSta = item.BuyCreSta == true ? "是" : "否",
                item.Endorser,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.ReceiveBank,
                item.RequestID
            };
            this.Paging(financeAccounts, convert);
        }

     
        protected void MakeBuy()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<BuyReportItem>>();

            var result = new Needs.Ccs.Services.Models.BuyImport(model).Make();

            Response.Write((new { success = result }).Json());

        }

       

        protected void MakeBuyAll()
        {
            string Code = Request.Form["Code"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string BillStatus = Request.Form["BillStatus"];
            string CreateStartDate = Request.Form["CreateStartDate"];
            string CreateEndDate = Request.Form["CreateEndDate"];

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
                Response.Write((new { success = false, msg = "必须勾选贴现开始结束日期" }).Json());
                return;
            }

            var financeAccounts = new Needs.Ccs.Services.Views.AccImportView().
             Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.ExchangeDate != null && item.BuyCreSta == false);

            if (!string.IsNullOrEmpty(Code))
            {
                Code = Code.Trim();
                financeAccounts = financeAccounts.Where(t => t.Code == Code);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.ExchangeDate > dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.ExchangeDate < dtEnd);
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

            List<BuyReportItem> model = new List<BuyReportItem>();

            foreach (var item in financeAccounts)
            {
                model.Add(new BuyReportItem
                {
                    ID = item.ID,
                    Code = item.Code,
                    Price = item.Price,
                    Endorser = item.Endorser,
                    Interest = item.ExchangePrice == null ? 0 : item.Price - item.ExchangePrice.Value,
                    FundTransferAccountName = item.FundTransferAccountName,
                    ExchangeDate = item.ExchangeDate == null ? "" : item.ExchangeDate.Value.ToString("yyyy-MM-dd"),
                    ReceiveBank = item.ReceiveBank
                });
            }

            var result = new Needs.Ccs.Services.Models.BuyImport(model).Make();

            Response.Write((new { success = result }).Json());

        }
    }
}