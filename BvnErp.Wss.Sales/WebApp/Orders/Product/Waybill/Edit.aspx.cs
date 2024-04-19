using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders.Product.Waybill
{
    public partial class Edit : Needs.Web.Forms.BasePage
    {

        /// <summary>
        /// 当前订单
        /// </summary>
        protected NtErp.Wss.Sales.Services.Order Order
        {
            get
            {
                return new NtErp.Wss.Sales.Services.Views.OrdersView().SingleOrDefault(t => t.ID == Request["orderid"]);
            }
        }
        /// <summary>
        /// 当前产品服务项
        /// </summary>
        protected NtErp.Wss.Sales.Services.Model.Orders.ServiceDetail ServiceDetail
        {
            get
            {
                return this.Order.Details.SingleOrDefault(t => t.Status == NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal && t.ServiceOutputID == Request["id"]);
            }
        }
       

        protected string Currency;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.Order == null)
                {
                    Response.Write("订单不存在！");
                    Response.End();
                }
                this.Currency = Order.Currency.GetTitle();
            }
        }

        protected void save()
        {
            string _waybillNumber = Request["_waybillNumber"],
                    _carrier = Request["_carrier"],
                    _weight = Request["_weight"], // 名称
                    _measurement = Request["_measurement"];
            int _count = int.Parse(Request["_count"]);
            int _payer = int.Parse(Request["_payer"]);
            decimal _freight = decimal.Parse(Request["_freight"]); // 金额
            try
            {
                var model = new NtErp.Wss.Sales.Services.Models.Waybill
                {
                    AdminID = Needs.Erp.ErpPlot.Current.ID,
                    WaybillNumber = _waybillNumber,
                    Carrier = _carrier,
                    Freight = _freight,
                    Measurement = _measurement,
                    Payer = (NtErp.Wss.Sales.Services.Underly.Orders.FreightPayer)_payer,
                    Weight = _weight,
                    Count = _count
                };
                this.ServiceDetail.Sent(_count, model);
                Response.Write(new
                {
                    success = true,
                    code = 200 // 金额为0
                }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new
                {
                    success = false,
                    code = -2 // 金额为0
                }.Json());
            }
        }
    }
}