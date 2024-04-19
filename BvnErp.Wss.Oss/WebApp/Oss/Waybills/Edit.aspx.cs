using Needs.Utils.Descriptions;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Oss.Waybills
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
        /// 订单项
        /// </summary>
        protected OrderItem OrderItem
        {
            get
            {
                var sid = Request["itemid"];
                return this.Order.Items[sid];
            }
        }

        /// <summary>
        /// 已发数量
        /// </summary>
        protected int Sent
        {
            get
            {
                return this.Order.Waybills.Where(t => t.OrderItemID == this.OrderItem.ID).ToArray().Sum(t => t.Count);
            }
        }


        /// <summary>
        /// 运单项
        /// </summary>
        protected WayItemOrder WayItem
        {
            get
            {
                var wid = Request["wid"];
                return this.Order.Waybills.SingleOrDefault(t => t.ID == wid);
            }
        }

        /// <summary>
        /// 付费方
        /// </summary>
        protected string FreightMode
        {
            get
            {
                return Order.TransportTerm.FreightMode.GetDescription();
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
            int count = int.Parse(Request["_count"]);
            string waybillid = Request["_waybillNumber"];
            decimal weight = string.IsNullOrWhiteSpace(Request["_weight"]) ? 0 : decimal.Parse(Request["_weight"]);
            string carrier = Request["_carrier"];

            var wid = Request["wid"];
            var sid = Request["itemid"];
            var oid = Request["orderid"];
            var model = new WayItem
            {
                WaybillID = waybillid,
                Count = count,
                OrderID = oid,
                OrderItemID = sid,
                ID = wid,
                Source = NtErp.Wss.Oss.Services.WayItemSource.Client,
                Weight = weight
            };
            model.Enter();

            Alert(this.hSuccess.Value, Request.Url, true);

        }


    }
}