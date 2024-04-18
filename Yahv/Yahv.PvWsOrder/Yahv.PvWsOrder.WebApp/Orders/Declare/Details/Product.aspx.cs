using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Declare.Details
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
            //有归类的订单项
            var items1 = Erp.Current.WsOrder.DeclareOrderItems.GetClassfiedItemByOrderID(orderId);
            //未归类的订单项
            var items2 = Erp.Current.WsOrder.OrderItems.Where(t => t.OrderID == orderId && t.Type == PvWsOrder.Services.Enums.OrderItemType.Modified).ToArray();
            if (items2.Count() == 0)
            {
                items2 = Erp.Current.WsOrder.OrderItems.Where(t => t.OrderID == orderId && t.Type == PvWsOrder.Services.Enums.OrderItemType.Normal).ToArray();
            }
            if (items1.Count() == items2.Count())
            {
                //订单项全部归类完成后信息
                var linq = items1.Select(t => new
                {
                    OrderItemID = t.ID,
                    Quantity = t.Quantity,
                    CustomName = t.ClassfiedName,
                    DateCode = t.DateCode,
                    Origin = t.Origin,
                    Unit = t.Unit.GetDescription(),
                    UnitPrice = t.UnitPrice,
                    Currency = t.Currency.GetDescription(),
                    TotalPrice = t.TotalPrice,
                    GrossWeight = t.GrossWeight,
                    Volume = t.Volume,
                    PartNumber = t.Product.PartNumber,
                    Manufacturer = t.Product.Manufacturer,
                }).OrderBy(item => item.PartNumber);
                this.Model.itemData = new { rows = linq.ToArray(), total = linq.Count() };
            }
            else
            {
                var linq = items2.Select(t => new
                {
                    OrderItemID = t.ID,
                    Quantity = t.Quantity,
                    CustomName = t.CustomName,
                    DateCode = t.DateCode,
                    Origin = t.Origin.ToString(),
                    Unit = t.Unit.GetDescription(),
                    UnitPrice = t.UnitPrice,
                    Currency = t.Currency.GetDescription(),
                    TotalPrice = t.TotalPrice,
                    GrossWeight = t.GrossWeight,
                    Volume = t.Volume,
                    PartNumber = t.Product.PartNumber,
                    Manufacturer = t.Product.Manufacturer,
                }).OrderBy(item => item.PartNumber);
                this.Model.itemData = new { rows = linq.ToArray(), total = linq.Count() };
            }
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