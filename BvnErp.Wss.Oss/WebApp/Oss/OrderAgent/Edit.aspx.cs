using NtErp.Wss.Oss.Services;
using NtErp.Wss.Oss.Services.Extends;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Oss.OrderAgent
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {

        /// <summary>
        /// 订单
        /// </summary>
        protected Order Order
        {
            get
            {
                var oid = Request["orderid"];
                return Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[oid];
            }
        }
        /// <summary>
        /// 现金余额
        /// </summary>
        protected decimal Cash
        {
            get
            {
                return this.Order.Client.GetBalance(Order.Beneficiary.Currency, UserAccountType.Cash);
            }
        }
        /// <summary>
        /// 信用余额
        /// </summary>
        protected decimal Credit
        {
            get
            {
                return this.Order.Client.GetBalance(Order.Beneficiary.Currency, UserAccountType.Credit);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (this.Order.Total - this.Order.Paid <= 0)
            {
                Alert(this.Hidden1.Value, Request.Url, true);
                return;
            }

            decimal amount = decimal.Parse(Request["_amount"]);
            var order = this.Order;

            order.NotEnough += Order_NotEnough;
            order.PaySuccess += Order_PaySuccess;
            order.AgentPay(amount);

        }

        private void Order_PaySuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert(this.hSuccess.Value, Request.Url, true);
        }

        private void Order_NotEnough(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Alert(this.hNotEnough.Value, Request.Url, true);
        }
    }
}