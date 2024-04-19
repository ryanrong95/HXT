using NtErp.Wss.Sales.Services.Underly.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders.Product
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void save()
        {
            int count = 0;
            decimal price = 0;
            if (int.TryParse(Request["_count"], out count))
            {
                if (count <= 0)
                {
                    Response.Write(new
                    {
                        success = false,
                        code = -1
                    }.Json());
                    return;
                }
            }
           
            if (decimal.TryParse(Request["_price"], out price))
            {
                if (price <= 0)
                {
                    Response.Write(new
                    {
                        success = false,
                        code = -2
                    }.Json());
                    return;
                }
            }

            if (this.ServiceDetail.Quantity == count && this.ServiceDetail.Price == price)
            {
                Response.Write(new
                {
                    success = false,
                    code = -5 // 未检测到修改，已取消提交
                }.Json());
                return;
            }

            try
            {
                if (this.Order == null)
                {
                    Response.Write(new
                    {
                        success = false,
                        code = -3 // 订单不存在
                    }.Json());
                    return;
                }

                var detail = this.ServiceDetail;
                detail.AdminID = Needs.Erp.ErpPlot.Current.ID;
                detail.Summary = Request["_summary"];
                this.Order.Change(detail, count, price);
                Response.Write(new
                {
                    success = true,
                    code = 200
                }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new
                {
                    success = false,
                    code = -4, // 修改失败
                    msg = ex.Message
                }.Json());
            }
        }


    }
}