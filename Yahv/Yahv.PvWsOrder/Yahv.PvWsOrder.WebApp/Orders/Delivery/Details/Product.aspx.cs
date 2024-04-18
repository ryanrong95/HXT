using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Delivery
{
    public partial class Product : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();

        }
        protected void LoadData()
        {
            var orderId = Request.QueryString["ID"];
            var order = Erp.Current.WsOrder.Orders[orderId];
            this.Model.Info = new
            {
                ClientName = order.OrderClient.Name,
                EnterCode = order.OrderClient.EnterCode,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsReciveCharge = order.OrderOutput?.IsReciveCharge,
            };

            //下单信息
            var linq = order.Orderitems.Select(t => new
            {
                OrderItemID = t.ID,
                Quantity = t.Quantity,
                DateCode = t.DateCode,
                Origin = t.OriginGetDescription,
                Unit = t.Unit.GetDescription(),
                UnitPrice = t.UnitPrice,
                Currency = t.Currency.GetDescription(),
                TotalPrice = t.TotalPrice,
                GrossWeight = t.GrossWeight,
                Volume = t.Volume,
                PartNumber = t.Product.PartNumber,
                Manufacturer = t.Product.Manufacturer,
                Condition = string.IsNullOrEmpty(t.Terms) ? "正常" : t.Terms
            }).OrderBy(item => item.PartNumber);
            this.Model.itemData = new { rows = linq.ToArray(), total = linq.Count() };
        }
        protected object data()
        {
            return new
            {
                rows = new List<object>(),
                total = 0,
            };
        }
    }
}