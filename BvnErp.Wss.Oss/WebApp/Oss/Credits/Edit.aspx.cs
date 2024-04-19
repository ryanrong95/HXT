using Needs.Erp;
using Needs.Underly;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using NtErp.Wss.Oss.Services;
using NtErp.Wss.Oss.Services.Extends;
using NtErp.Wss.Services.Generic.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Oss.Credits
{
    public partial class Edit : Needs.Web.Forms.ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                var client = ErpPlot.Current.Websites.MyClients[clientid];
                var debts = client.GetDebtIndexes();
                List<CreditItemGroup[]> list = new List<NtErp.Wss.Oss.Services.CreditItemGroup[]>();
                foreach (var item in debts)
                {
                    var debt = client.GetDebts(item);
                    list.Add(debt);
                }
                this.Model.Getdebts = list;
            }
        }
        /// <summary>
        /// 消费记录
        /// </summary>
        /// <returns></returns>
        protected object details()
        {
            string period = Request.QueryString[nameof(period)];
            Currency currency = Request.QueryString[nameof(currency)].ToEnum<Currency>();
            return ErpPlot.Current.Websites.MyClients[Request.QueryString["id"]].GetRepaidRecord(currency, int.Parse(period));
        }
        /// <summary>
        /// 信用还款记录
        /// </summary>
        /// <returns></returns>
        protected object detailsRefund()
        {
            string period = Request.QueryString[nameof(period)];
            Currency currency = Request.QueryString[nameof(currency)].ToEnum<Currency>();
            return ErpPlot.Current.Websites.MyClients[Request.QueryString["id"]].GetRepayingRecord(currency, int.Parse(period));
        }
        /// <summary>
        /// 当前余额,应还金额
        /// </summary>
        /// <returns></returns>
        protected object balance()
        {
            Currency currency = Request.QueryString[nameof(currency)].ToEnum<Currency>();
            string period = Request.QueryString[nameof(period)];
            var client = ErpPlot.Current.Websites.MyClients[Request.QueryString["id"]];
            return new { Balance = client.GetBalance(currency, UserAccountType.Cash), AmountPayable = client.GetDebt(currency, int.Parse(period)).Debt };
        }
        /// <summary>
        /// 还款
        /// </summary>
        protected void refund()
        {
            string period = Request.Form[nameof(period)];
            Needs.Underly.Currency currency = Request.Form[nameof(currency)].ToEnum<Currency>();
            decimal refund = decimal.Parse(Request.Form[nameof(refund)]);
            var client = ErpPlot.Current.Websites.MyClients[Request.QueryString["id"]];
            client.RepaySuccess += Client_RepaySuccess;
            client.RepayError += Client_RepayError;
            client.Repay(currency, int.Parse(period), refund);
        }

        private void Client_RepayError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            this.Response.Write(new { message = e.Message, success = false }.Json());
            this.Response.End();
            //Alert(this.hMessgeError.Value, Request.Url, false);
        }
        private void Client_RepaySuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            this.Response.Write(new { success = true }.Json());
            this.Response.End();
            //Alert(this.hMessgeSucess.Value, Request.Url, false);
        }

    }
}