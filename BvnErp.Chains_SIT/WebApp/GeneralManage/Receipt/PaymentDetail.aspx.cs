using Needs.Wl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;

namespace WebApp.GeneralManage.Receipt
{
    public partial class PaymentDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            data();
        }

        protected void data()
        {
            var orderId = Request.QueryString["ID"];
            var feeType = Request.QueryString["FeeType"];

            //货款
            var payNoticeIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.PaymentNoticeItem.Where(p => p.OrderID == orderId && p.Status == PaymentNoticeStatus.Paid).Select(p => p.ID).ToList();

            var payNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderPaymentNotice
                .Where(t => t.PayFeeType == (FinanceFeeType)Enum.Parse(typeof(FinanceFeeType), feeType) && payNoticeIds.Contains(t.ID)).ToList();

            var totalAmount = payNotices.Sum(o => (decimal?)o.Amount * o.ExchangeRate).GetValueOrDefault();

            var result = new List<dynamic>();

            result.AddRange(payNotices.Select(payNotice => new
            {
                PayDate = payNotice.PayDate.ToShortDateString(),
                Payor = payNotice.Payer.ByName,
                Payee = payNotice.PayeeName,
                PayAmount = (payNotice.Amount * payNotice.ExchangeRate).GetValueOrDefault().ToString("0.00")
            }));
            result.Add(new
            {
                PayDate = "总计",
                Payor = "",
                Payee = "",
                PayAmount = totalAmount.ToString("0.00")
            });
            this.Model.Payments = result.Json();
        }
    }
}
