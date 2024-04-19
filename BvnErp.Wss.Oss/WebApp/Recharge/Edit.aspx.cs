using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Serializers;
using Needs.Erp;
using NtErp.Wss.Oss.Services;
using Needs.Utils.Converters;
using NtErp.Wss.Oss.Services.Extends;
using NtErp.Wss.Services.Generic.Extends;

namespace WebApp.Accounts.Recharge
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var type = Request.QueryString["type"].ToEnum<UserAccountType>();
                string clientid = Request.QueryString["id"];
                if (type == UserAccountType.Cash)
                {
                    this.Model.Balances = ErpPlot.Current.Websites.MyClients[clientid].GetBalances(UserAccountType.Cash).ToArray();
                    this.Model.Records = ErpPlot.Current.Websites.MyClients[clientid].GetRecharges();
                }

                if (type == UserAccountType.Credit)
                {
                    this.Model.Balances = ErpPlot.Current.Websites.MyClients[clientid].GetBalances(UserAccountType.Credit).ToArray();
                    this.Model.Records = ErpPlot.Current.Websites.MyClients[clientid].GetApprove();
                }
               
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"].ToEnum<UserAccountType>();
            Needs.Underly.Currency currency = Request.Form[nameof(currency)].ToEnum<Needs.Underly.Currency>();
            decimal price = Convert.ToDecimal(Request.Form["price"]);
            string clientid = Request.QueryString["id"];
            if (type == UserAccountType.Cash)
            {
                ErpPlot.Current.Websites.MyClients[clientid].Recharge(currency, price, Request["code"]);
            }
            if (type == UserAccountType.Credit)
            {
                ErpPlot.Current.Websites.MyClients[clientid].Approve(currency, price, Request["code"]);
            }
            Alert(this.hMessgeSucess.Value, Request.Url, false);
        }
    }
}