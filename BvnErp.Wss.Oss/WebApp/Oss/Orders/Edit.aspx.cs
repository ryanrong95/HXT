using Needs.Utils.Descriptions;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Oss.Orders
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        /// <summary>
        /// 订单
        /// </summary>
        Order Order
        {
            get
            {
                var oid = Request["orderid"];
                return Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[oid];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Order.Invoice

            if (!IsPostBack)
            {
                this.Model = this.Order;
            }

        }

        /// <summary>
        /// 删除产品项
        /// </summary>
        protected void del_item()
        {
            var sid = Request["itemid"];
            this.Order.Items[sid].Abandon();
        }

        protected object get_items()
        {
            var items = this.Order.Items;
            var ways = this.Order.Waybills;

            return items.Select(entity => new
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                ServiceID = entity.ServiceID,
                CustomerCode = entity.CustomerCode,
                From = entity.From,
                FormName = entity.From.GetDescription(),
                Origin = entity.Origin,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                Weight = entity.Weight,
                Status = entity.Status,
                StatusName = entity.Status.GetDescription(),
                ProductName = entity.Product.Name,
                ManufacturerName = entity.Product.Manufacturer.Name,
                SupplierName = entity.Supplier.Name,
                SendedCount = ways.Where(item => item.OrderItemID == entity.ID).Sum(item => item.Count),
                SubTotal = entity.UnitPrice * entity.Quantity,
         
            });
        }

        protected object get_premiums()
        {
            var items = this.Order.Premiums;

            return items.Select(entity => new
            {
                entity.ID,
                entity.OrderID,
                entity.OrderItemID,
                entity.Name,
                entity.Count,
                entity.Price,
                entity.Total,
                entity.Summary,
            });
        }
    }
}