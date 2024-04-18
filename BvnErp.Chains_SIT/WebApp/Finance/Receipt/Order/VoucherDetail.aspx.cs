using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Order
{
    public partial class VoucherDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string FinanceReceiptId = Request.QueryString["FinanceReceiptId"];

            this.Model.FinanceReceiptId = FinanceReceiptId;
        }
        /*
        protected void data()
        {
            string financeReceiptId = Request.QueryString["FinanceReceiptId"];

            var vouchers = new Needs.Ccs.Services.Views.FinanceVoucherView()
                .Where(t => t.FinanceReceiptID == financeReceiptId
                         && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .OrderByDescending(t => t.UseTime).ToList();

            Response.Write(new
            {
                rows = vouchers.Select(
                        item => new
                        {
                            FinanceVoucherID = item.ID,
                            Amount = item.Amount,
                            UseTime = item.UseTime?.ToString("yyyy-MM-dd hh:mm:ss"),
                        }
                     ).ToArray(),
            }.Json());
        }
        */
    }
}