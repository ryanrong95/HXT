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
    public partial class FundTransImport : Uc.PageBase
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
           
        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string OutBankName = Request.QueryString["OutBankName"];
            string InBankName = Request.QueryString["InBankName"];
            string ApplyNo = Request.QueryString["ApplyNo"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.FundTranCreSta == false).
                 Where(item => item.OutAccount.AccountName != "芯达通-兴业银行快捷支付平台").
                 Where(item=>item.FromSeqNo=="").
                 Where(item => item.ApplyStatus == Needs.Ccs.Services.Enums.FundTransferApplyStatus.Done).
                OrderByDescending(t => t.CreateDate).AsQueryable();

            if (!string.IsNullOrEmpty(OutBankName))
            {
                OutBankName = OutBankName.Trim();
                financeAccounts = financeAccounts.Where(t => t.OutAccount.BankName.Contains(OutBankName));
            }
            if (!string.IsNullOrEmpty(InBankName))
            {
                InBankName = InBankName.Trim();
                financeAccounts = financeAccounts.Where(t => t.InAccount.BankName.Contains(InBankName));
            }
            if (!string.IsNullOrEmpty(ApplyNo))
            {
                ApplyNo = ApplyNo.Trim();
                financeAccounts = financeAccounts.Where(t => t.ID == ApplyNo);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.PaymentDate >= dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.PaymentDate < dtEnd);
            }
            Func<Needs.Ccs.Services.Models.FundTransferApplies, object> convert = item => new
            {
                ID = item.ID,
                OutAccountID = item.OutAccount.AccountName,
                OutAccountName = item.OutAccount.BankName,
                OutAmount = item.OutAmount,
                InAccountID = item.InAccount.AccountName,
                InAccountName = item.InAccount.BankName,
                InAmount = item.InAmount,
                CreateDate = item.PaymentDate==null?item.CreateDate.ToString("yyyy-MM-dd"):item.PaymentDate.Value.ToString("yyyy-MM-dd"),               
            };
            this.Paging(financeAccounts, convert);
        }

        /// <summary>
        /// 生成凭证
        /// </summary>
        protected void MakeAccount()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<FundTransReportItem>>();

            var result = new Needs.Ccs.Services.Models.FundTransImport(model).Make();

            Response.Write((new { success = result }).Json());

        }

        protected void MakeAccountAll()
        {
            string OutBankName = Request.Form["OutBankName"];
            string InBankName = Request.Form["InBankName"];
            string ApplyNo = Request.Form["ApplyNo"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];

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

            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies
              .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.FundTranCreSta == false).
               Where(item => item.OutAccount.AccountName != "芯达通-兴业银行快捷支付平台").
               Where(item => item.FromSeqNo == "").
               Where(item => item.ApplyStatus == Needs.Ccs.Services.Enums.FundTransferApplyStatus.Done).
              OrderByDescending(t => t.CreateDate).AsQueryable();

            if (!string.IsNullOrEmpty(OutBankName))
            {
                OutBankName = OutBankName.Trim();
                financeAccounts = financeAccounts.Where(t => t.OutAccount.BankName.Contains(OutBankName));
            }
            if (!string.IsNullOrEmpty(InBankName))
            {
                InBankName = InBankName.Trim();
                financeAccounts = financeAccounts.Where(t => t.InAccount.BankName.Contains(InBankName));
            }
            if (!string.IsNullOrEmpty(ApplyNo))
            {
                ApplyNo = ApplyNo.Trim();
                financeAccounts = financeAccounts.Where(t => t.ID == ApplyNo);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.PaymentDate > dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.PaymentDate < dtEnd);
            }

            List<FundTransReportItem> model = new List<FundTransReportItem>();

            foreach (var item in financeAccounts)
            {
                model.Add(new FundTransReportItem
                {
                    ID = item.ID,
                    OutAccountID = item.OutAccount.AccountName,
                    InAccountID = item.InAccount.AccountName,
                    OutAmount = item.OutAmount,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd")
                });
            }

            var result = new Needs.Ccs.Services.Models.FundTransImport(model).Make();
            Response.Write((new { success = result }).Json());
        }
    }
}