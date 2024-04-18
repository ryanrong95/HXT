using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Models;

namespace WebApp.Order.Classified
{
    /// <summary>
    /// 订单报价界面
    /// 用于订单报价
    /// </summary>
    public partial class Display : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyClassifiedOrders[id];

            this.Model.OrderID = order.ID;
            this.Model.OrderData = new
            {
                order.ID,
                order.Client.ClientCode,
                ClientName = order.Client.Company.Name,
                order.Currency,
                IsFullVehicle = order.IsFullVehicle?"是": "否",
                IsLoan = order.IsLoan ? "是" : "否",
                OrderType = order.Type,
                CreateDate = order.CreateDate.ToShortDateString()
            }.Json();
            this.Model.ClientRank = order.Client.ClientRank;
        }

        /// <summary>
        /// 初始化待报价产品列表
        /// </summary>
        protected void data()
        {
            try
            {
                string id = Request.QueryString["ID"];
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyClassifiedOrders[id];
                if (order.CustomsExchangeRate == null || order.RealExchangeRate == null)
                {
                    order.GenerateBill();
                }

                var productFeeExchangeRate = order.ProductFeeExchangeRate;
                //税点
                var taxpoint = 1 + order.ClientAgreement.InvoiceTaxRate;
                //代理费率、最低代理费
                decimal agencyRate = order.AgencyFeeExchangeRate * order.ClientAgreement.AgencyRate;
                decimal minAgencyFee = order.ClientAgreement.MinAgencyFee;
                bool isAverage = order.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                //平摊代理费
                var aveAgencyFee = (order.AgencyFee * taxpoint / order.Items.Count()).ToRound(4);

                Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
                {
                    item.ID,
                    Name = item.Category.Name,
                    Manufacturer = item.Manufacturer,
                    Model = item.Model,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    DeclareValue = (item.TotalPrice * productFeeExchangeRate),
                    TraiffRate = item.ImportTax.Rate,
                    Traiff = item.ImportTax.Value,
                    AddTaxRate = item.AddedValueTax.Rate,
                    AddTax = item.AddedValueTax.Value,
                    ExciseTaxRate = item.ExciseTax?.Rate ?? 0M,
                    ExciseTax = item.ExciseTax?.Value ?? 0M,
                    AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint,
                    InspectionFee = (item.InspectionFee.GetValueOrDefault() * taxpoint),
                };

                Response.Write(new
                {
                    rows = order.Items.Select(convert).ToArray(),
                    total = order.Items.Count(),
                    success = true
                }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new
                {
                    success = false,
                    message = "报价失败：" + ex.Message
                }.Json());
            }
        }

        /// <summary>
        /// 报价
        /// </summary>
        protected void Quote()
        {
            try
            {
                string id = Request.Form["ID"];
                string[] ids = Request.Form["IDs"].Split(',');
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyClassifiedOrders[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                foreach (var item in order.Items)
                {
                    if (ids.Contains(item.ID))
                    {
                        item.SampllingCheck();
                    }
                }

                order.SetAdmin(admin);
                order.Quoted += Order_QuoteSuccess;
                order.Quote();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "订单报价失败：" + ex.Message}).Json());
            }
        }

        /// <summary>
        /// 订单报价成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_QuoteSuccess(object sender, OrderQuotedEventArgs e)
        {
            Response.Write((new { success = true, message = "订单报价成功！" }).Json());
        }
    }
}