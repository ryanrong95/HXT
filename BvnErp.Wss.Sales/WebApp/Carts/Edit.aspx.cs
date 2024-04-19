using Newtonsoft.Json.Linq;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Carts
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
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
        /// 当前Cart
        /// </summary>
        protected NtErp.Wss.Sales.Services.Models.Carts.Cart Cart
        {
            get
            {
                var sid = Request["sid"];
                return new NtErp.Wss.Sales.Services.Views.CartsView().SingleOrDefault(t => t.UserID == this.uid && t.ServiceOutputID == sid);
            }
        }

        protected JArray Districts
        {
            get
            {
                var arry = new JArray();
                foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Sales.Services.Underly.District)))
                {
                    var e = (NtErp.Wss.Sales.Services.Underly.District)item;
                    if (e == NtErp.Wss.Sales.Services.Underly.District.Unknown || e== NtErp.Wss.Sales.Services.Underly.District.Global)
                    {
                        continue;
                    }
                    var obj = new JObject();
                    obj.Add("value", (int)e);
                    obj.Add("text", e.GetTitle());
                    arry.Add(obj);
                }
                return arry;
            }
        }
        protected JArray Currencys
        {
            get
            {
                var arry = new JArray();
                var obj = new JObject();
                obj.Add("value", (int)NtErp.Wss.Sales.Services.Underly.Currency.CNY);
                obj.Add("text", NtErp.Wss.Sales.Services.Underly.Currency.CNY.ToString());
                arry.Add(obj);
                var obj2 = new JObject();
                obj2.Add("value", (int)NtErp.Wss.Sales.Services.Underly.Currency.HKD);
                obj2.Add("text", NtErp.Wss.Sales.Services.Underly.Currency.HKD.ToString());
                arry.Add(obj2);
                var obj3 = new JObject();
                obj3.Add("value", (int)NtErp.Wss.Sales.Services.Underly.Currency.USD);
                obj3.Add("text", NtErp.Wss.Sales.Services.Underly.Currency.USD.ToString());
                arry.Add(obj3);
                //foreach (var item in Enum.GetValues(typeof(NtErp.Wss.Sales.Services.Underly.Currency)))
                //{
                //    var e = (NtErp.Wss.Sales.Services.Underly.Currency)item;
                //    if (e == NtErp.Wss.Sales.Services.Underly.Currency.Unkown)
                //    {
                //        continue;
                //    }
                //    var obj = new JObject();
                //    obj.Add("value", (int)e);
                //    obj.Add("text", e.ToString());
                //    arry.Add(obj);
                //}
                return arry;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected void save()
        {
            if (string.IsNullOrWhiteSpace(uid))
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
                var _district = (NtErp.Wss.Sales.Services.Underly.District)Enum.Parse(typeof(NtErp.Wss.Sales.Services.Underly.District), Request["_district"]);
                var _currency = (NtErp.Wss.Sales.Services.Underly.Currency)Enum.Parse(typeof(NtErp.Wss.Sales.Services.Underly.Currency), Request["_currency"]);
                var _price = decimal.Parse(Request["_price"]);
                var _count = int.Parse(Request["_count"]);
                var _leadtime = Request["_leadtime"];
                var _package = Request["_package"];
                var _summary = Request["_summary"];
                var _batch = Request["_batch"];


                if (this.Cart != null)
                {
                    var model = this.Cart;
                    model.District = _district;
                    model.Currency = _currency;
                    model.Price = _price;
                    model.Quantity = _count;
                    model.Product.Name = _title;
                    model.Product.Supplier = _supplier;
                    model.Product.Manufacturer = _mf;
                    model.Product.PackageCase = _package;
                    model.Product.Leadtime = _leadtime;
                    model.Product.Batch = _batch;
                    model.Product.Summary = _summary;
                    model.Logs.Add(new NtErp.Wss.Sales.Services.Models.Carts.Cart.Log
                    {
                        AdminID = Needs.Erp.ErpPlot.Current.ID,
                        Type = 2,
                    });
                    model.Enter();
                }
                else
                {
                    var model = new NtErp.Wss.Sales.Services.Models.Carts.Cart
                    {
                        UserID = uid,
                        Currency = _currency,
                        District = _district,
                        Price = _price,
                        Quantity = _count,
                        Product = new NtErp.Wss.Sales.Services.Models.Carts.CartProduct
                        {
                            Source = NtErp.Wss.Sales.Services.Models.Carts.CartSource.ForCart,
                            Name = _title,
                            Supplier = _supplier,
                            Manufacturer = _mf,
                            Batch = _batch,
                            PackageCase = _package,
                            Leadtime = _leadtime,
                            Summary = _summary,

                            Count = 0,
                            B1bSign ="",

                        },
                        ProductSign = ""
                    };
                    model.Logs.Add(new NtErp.Wss.Sales.Services.Models.Carts.Cart.Log
                    {
                        AdminID = Needs.Erp.ErpPlot.Current.ID,
                        Type = 1,
                    });
                    model.Enter();
                }
                Response.Write(new { success = true }.Json());

            }
            catch (Exception ex)
            {
                Response.Write(new { success = false }.Json());
            }

        }
    }
}