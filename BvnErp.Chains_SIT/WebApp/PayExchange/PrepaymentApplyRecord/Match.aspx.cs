using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PayExchange.PrepaymentApplyRecord
{
    public partial class Match : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        public void LoadData()
        {
            string payExchangeApplyID = Request.QueryString["ID"];
            string amount = Request.QueryString["Amount"];//预付汇金额
            string OrderID = Request.QueryString["OrderID"];
            if (!string.IsNullOrEmpty(payExchangeApplyID))
            {
                this.Model.ID = payExchangeApplyID;
            }
            else
            {
                this.Model.ID = "";
            }
            if (!string.IsNullOrEmpty(amount))
            {
                this.Model.Amount = amount;
            }
            else
            {
                this.Model.Amount = "";
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                this.Model.OrderID = OrderID;
            }
            else
            {
                this.Model.OrderID = "";
            }
        }
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string id = Request.QueryString["ID"];
            string clientID = Request.QueryString["ClientID"];
            string supplierEnglishName = Request.QueryString["SupplierID"];
            string currency = Request.QueryString["Currency"];
            //decimal amount = Convert.ToDecimal(Request.QueryString["Amount"]);//预付汇金额

            if (!string.IsNullOrEmpty(id))
            {
                this.Model.ID = id;
            }
            else
            {
                this.Model.ID = "";
            }
            var orders = new Needs.Ccs.Services.Views.OrderConsigneesSupplierView()
                .Where(item => item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed
                    && item.OrderStatus <= Needs.Ccs.Services.Enums.OrderStatus.Completed
                    && item.ClientID == clientID && item.Currency == currency)
                .Where(item => item.PayExchangeSuppliers.Where(s => s.ClientSupplier.Name.Contains(supplierEnglishName)).Count() > 0).OrderByDescending(item => item.CreateDate);

            Func<Needs.Ccs.Services.Views.OrderConsigneesSupplierViewModel, object> convert = item => new
            {
                ID = item.OrderID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                SupplierName = item.PayExchangeSuppliers.Select(ps => ps.ClientSupplier.Name),
                PaidExchangeAmount = item.PaidExchangeAmount,
                Currency = item.Currency,
                RealExchangeRate = item.ExchangeRate,
                DeclarePrice = item.DeclarePrice,
            };
            this.Paging(orders, convert);
        }
    }
}