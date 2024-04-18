using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl;
using NPOI.SS.Formula.Functions;

namespace WebApp.Finance.Receipt
{
    /// <summary>
    /// 财务收款记录详情界面
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadInitData();
        }

        protected void LoadInitData()
        {
            //?ID=FinReceipt20190313000022
            string ID = Request.QueryString["ID"];
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
                Admin = receipt.Admin.ByName,
                Summary = receipt.Summary ?? string.Empty,
                SeqNo = receipt.SeqNo,
                DiscountInterest = DiscountInterest,
                ReceiptTypeValue = receipt.ReceiptType,
            }.Json();
            this.Model.FeeType = (int)receipt.FeeType;
        }

        protected void data()
        {
            string financeReceiptID = Request.QueryString["ID"];
            var receiptNoticeID = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ReceiptNotices.FirstOrDefault(f => f.ID == financeReceiptID)?.ID;

            //实收款记录
            var orderReceiptDetail = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiveds.AsQueryable()
                .Where(r => r.ReceiptNoticeID == receiptNoticeID);

            Func<Needs.Ccs.Services.Models.OrderReceipt, object> convert = orderReceipt => new
            {
                OrderID = orderReceipt.OrderID,
                Type = orderReceipt.FeeTypeShowName,
                Amount = orderReceipt.Amount,
                Date = orderReceipt.CreateDate.ToShortDateString(),
                Admin = orderReceipt.Admin.RealName
            };

            Response.Write(new
            {
                rows = orderReceiptDetail.Select(convert).ToList(),
                total = orderReceiptDetail.Count()
            }.Json());
        }
    }
}
