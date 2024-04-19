using Newtonsoft.Json.Linq;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders.Product
{
    public partial class Add : Needs.Web.Sso.Forms.ErpPage
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        protected string uid
        {
            get
            {
                return Request["uid"];
            }
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        protected string oid
        {
            get
            {
                return Request["oid"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var order = new NtErp.Wss.Sales.Services.Views.OrdersView().SingleOrDefault(t => t.ID == oid);
                if (order != null)
                {
                    ViewState["district"]= order.District.ToString();
                    ViewState["currency"] = order.Currency.ToString();
                }
            }
        }

        protected void save()
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(oid))
            {
                Response.Write(new { success = false }.Json());
                return;
            }

            try
            {
                var sid = Request["sid"];
                var _title = Request["_title"];
                var _supplier = Request["_supplier"];
                var _mf = Request["_mf"];
                var _price = decimal.Parse(Request["_price"]);
                var _count = int.Parse(Request["_count"]);
                var _leadtime = Request["_leadtime"];
                var _package = Request["_package"];
                var _summary = Request["_summary"];
                var _batch = Request["_batch"];

                var order = new NtErp.Wss.Sales.Services.Views.OrdersView().SingleOrDefault(t => t.ID == oid);
                if (order == null)
                {
                    Response.Write(new { success = false }.Json());
                    return;
                }
                var detail = new NtErp.Wss.Sales.Services.Model.Orders.ServiceDetail
                {
                    AdminID = Needs.Erp.ErpPlot.Current.ID,
                    Price = _price,
                    Quantity = _count,
                    ServiceOutputID = Needs.Overall.PKeySigner.Pick(NtErp.Wss.Sales.Services.PKeyType.Cart),
                    Product = new NtErp.Wss.Sales.Services.Models.Orders.SaleProduct
                    {
                        Source = NtErp.Wss.Sales.Services.Models.Carts.CartSource.ForOrder,
                        ProductSign = "",
                        Name = _title,
                        Supplier = _supplier,
                        Manufacturer = _mf,
                        Batch = _batch,
                        PackageCase = _package,
                        Leadtime = _leadtime
                    }
                };
                detail.ServiceInputID = detail.ServiceOutputID;

                order.AddDetail(detail);

                Response.Write(new { success = true }.Json());

            }
            catch (Exception ex)
            {
                Response.Write(new { success = false }.Json());
            }

        }
    }
}