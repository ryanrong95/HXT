using Needs.Utils.Serializers;
using NtErp.Wss.Oss.Services.Extends;
using NtErp.Wss.Oss.Services.Models;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Oss.OrderItemProducts
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        Order Order
        {
            get
            {
                var oid = Request["orderid"];
                return Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[oid];
            }
        }
        OrderItem OrderItem
        {
            get
            {
                var sid = Request["itemid"];
                return this.Order.Items[sid];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = this.OrderItem;
                //this.InitPage();
                //this.Model.Json();


            }
        }

        void InitPage()
        {
            var orderid = Request["orderid"];
            var order = Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[orderid];
            if (!string.IsNullOrWhiteSpace(Request["itemid"]))
            {
                var itemid = Request["itemid"];
                this.Model = this.Order.Items[itemid];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var orderid = Request["orderid"];
            var itemid = Request["itemid"];
            var productName = Request["txtProductName"];
            var supplierName = Request["txtSupplierName"];
            var manufacturerName = Request["txtManufacturerName"];
            var unitPrice = decimal.Parse(Request["txtUnitPrice"]);
            var quantity = int.Parse(Request["txtQuantity"]);
            var leadtime = Request["txtLeadtime"];
            var packageCase = Request["txtPackageCase"];
            var packaging = Request["txtPackaging"];
            var note = Request["txtNote"];
            var dateCode = Request["txtDateCode"];

            // 添加
            if (string.IsNullOrWhiteSpace(itemid))
            {
                var model = new OrderItem
                {
                    OrderID = this.Order.ID,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Supplier = new Company
                    {
                        Type = NtErp.Wss.Oss.Services.CompanyType.Supplier,
                        Name = supplierName
                    },
                    From = NtErp.Wss.Oss.Services.OrderItemFrom.VirtualItem,
                    Product = new StandardProduct
                    {
                        Name = productName,
                        PackageCase = packageCase,
                        Packaging = packaging,
                        Batch = dateCode,
                        DateCode = dateCode,
                        Manufacturer = new Company
                        {
                            Type = NtErp.Wss.Oss.Services.CompanyType.Manufactruer,
                            Name = manufacturerName
                        },
                        Description = note
                    },
                    Leadtime = leadtime,
                    Note = note,
                };
                model.Enter();
            }
            // 修改
            else
            {
                var model = new OrderItem
                {
                    ID = OrderItem.ID,
                    CustomerCode = OrderItem.CustomerCode,
                    Origin = OrderItem.Origin,
                    ServiceID = OrderItem.ServiceID,
                    Weight = OrderItem.Weight,
                    Status = OrderItem.Status,

                    OrderID = this.Order.ID,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Supplier = new Company
                    {
                        Type = NtErp.Wss.Oss.Services.CompanyType.Supplier,
                        Name = supplierName
                    },
                    From = OrderItem.From,
                    Leadtime = leadtime,
                    Note = note,
                    Product = new StandardProduct
                    {
                        Name = productName,
                        PackageCase = packageCase,
                        Packaging = packaging,
                        Batch = dateCode,
                        DateCode = dateCode,
                        Manufacturer = new Company
                        {
                            Type = NtErp.Wss.Oss.Services.CompanyType.Manufactruer,
                            Name = manufacturerName
                        },
                        Description = note
                    },
                };

                model.Enter();
            }

            Alert(this.hSuccess.Value, Request.Url, true);
        }
    }
}