using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.RefundApply
{
    public partial class AccountMatch : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInitData();
            }
        }

        protected void LoadInitData()
        {
            string ID = Request.QueryString["ReceiptID"];
            var receipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceReceipts.FirstOrDefault(s => s.ID == ID);
            decimal DiscountInterest = 0;
            var fundTransferApply = new Needs.Ccs.Services.Views.FundTransferAppliesView().Where(t => t.FromSeqNo == receipt.SeqNo).FirstOrDefault();
            if (fundTransferApply != null)
            {
                DiscountInterest = fundTransferApply.DiscountInterest == null ? 0 : fundTransferApply.DiscountInterest.Value;
            }

            this.Model.Receipt = new
            {
                Payer = receipt.Payer,
                FeeType = receipt.FeeType.GetDescription(),
                ReceiptType = receipt.ReceiptType.GetDescription(),
                ReceiptDate = receipt.ReceiptDate.ToShortDateString(),
                Amount = receipt.Amount,
                Currency = receipt.Currency,
                Rate = receipt.Rate,
                Vault = receipt.Vault.Name,
                AccountName = receipt.Account.AccountName,
                BankAccount = receipt.Account.BankAccount,
                Admin = receipt.Admin.RealName,
                Summary = receipt.Summary ?? string.Empty,
                SeqNo = receipt.SeqNo,
                DiscountInterest = DiscountInterest,
                ReceiptTypeValue = receipt.ReceiptType,
            }.Json();
            this.Model.FeeType = (int)receipt.FeeType;
        }

        protected void Approve()
        {
            try
            {
                string ApplyID = Request.Form["ApplyID"];
                string PayeeAccountID = Request.Form["PayeeAccountID"];
                string PayeeAccountNo = Request.Form["PayeeAccountNo"];
                string PayeeName = Request.Form["PayeeName"];
                string PayeeBank = Request.Form["PayeeBank"];
                var apply = new Needs.Ccs.Services.Views.RefundApplyView().Where(t => t.ID == ApplyID).FirstOrDefault();
                apply.PayeeAccountID = PayeeAccountID;
                apply.UpdateAccount();

                var Notice = new Needs.Ccs.Services.Views.PaymentNoticesView().Where(t => t.RefundApplyID == ApplyID).FirstOrDefault();
                if (Notice != null)
                {
                    Notice.PayeeName = PayeeName;
                    Notice.BankName = PayeeBank;
                    Notice.BankAccount = PayeeAccountNo;

                    Notice.UpdateAccount();
                }

                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                Logs log = new Logs();
                log.Name = "退款申请";
                log.MainID = apply.ID;
                log.AdminID = apply.Applicant.ID;
                log.Json = apply.Json();
                log.Summary = "财务【" + currentAdmin.ByName + "】匹配了退款账号：" + PayeeAccountNo;
                log.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
        }

    }
}
