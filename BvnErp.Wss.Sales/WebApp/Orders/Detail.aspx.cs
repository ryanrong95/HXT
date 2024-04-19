using NtErp.Wss.Sales.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders
{
    public partial class Detail : Needs.Web.Sso.Forms.ErpPage
    {

        protected Order Order
        {
            get
            {
                var id = Request["id"];
                return new NtErp.Wss.Sales.Services.Views.OrdersView().SingleOrDefault(t => t.ID == id);
            }
        }

        protected NtErp.Services.Models.Admin[] CustomerService
        {
            get
            {
                var userid = this.Order.UserID;
                var arry = new NtErp.Services.Views.MapsAdminClientView()
                        .Where(t => t.ClientID == userid).Select(t => t.AdminID).ToArray();
                var admins = new NtErp.Services.Views.AdminsAlls().Where(t => arry.Contains(t.ID)).ToArray();

                return admins;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            

            if (!IsPostBack)
            {
                this.Model = this.Order;
            }
        }

        /// <summary>
        /// 删除产品项
        /// </summary>
        protected void pdt_del()
        {
            try
            {
                var serviceID = Request["sid"];
                var detail = this.Order.Details.SingleOrDefault(t => t.ServiceOutputID == serviceID && t.Status == NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal);
                if (detail == null)
                {
                    //this.Alert(this.Hidden1.Value, Request.Url);
                    Response.Write(this.Hidden1.Value);
                }
                else
                {
                    detail.AdminID = Needs.Erp.ErpPlot.Current.ID;
                    detail.AlterDate = DateTime.Now;
                    detail.Summary = Request["summary"];
                    this.Order.RemoveItem(detail);
                    Response.Write("success");
                }
            }
            catch (Exception ex)
            {
                Response.Write(this.Hidden3.Value);
            }

        }


    }
}