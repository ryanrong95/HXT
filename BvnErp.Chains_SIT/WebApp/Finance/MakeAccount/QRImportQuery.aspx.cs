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
    public partial class QRImportQuery : Uc.PageBase
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
            //审批类型
            this.Model.FundTransferApplyStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FundTransferApplyStatus>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string FundTransferApplyStatus = Request.QueryString["FundTransferApplyStatus"];
            string ApplyNo = Request.QueryString["ApplyNo"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var financeAccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal && item.FundTranCreSta == true && item.ApplyStatus == Needs.Ccs.Services.Enums.FundTransferApplyStatus.Done).
                Where(item => item.OutAccount.AccountName == "芯达通-兴业银行快捷支付平台").
                OrderByDescending(t => t.CreateDate).AsQueryable();

            if (!string.IsNullOrEmpty(FundTransferApplyStatus))
            {
                FundTransferApplyStatus = FundTransferApplyStatus.Trim();
                var applyStatus = (Needs.Ccs.Services.Enums.FundTransferApplyStatus)Convert.ToInt16(FundTransferApplyStatus);
                financeAccounts = financeAccounts.Where(t => t.ApplyStatus == applyStatus);
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
                CreateDate = item.PaymentDate==null? item.CreateDate.ToString("yyyy-MM-dd"):item.PaymentDate.Value.ToString("yyyy-MM-dd"),
                QRCodeFee = item.QRCodeFee,
                FundTranNo = item.FundTranNo,
                FundTranWrod = item.FundTranWrod
            };
            this.Paging(financeAccounts, convert);
        }

    }
}