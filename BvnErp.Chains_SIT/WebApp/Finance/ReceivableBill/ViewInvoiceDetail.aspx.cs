using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.ReceivableBill
{
    public partial class ViewInvoiceDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载开票通知
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string orderID = Request.QueryString["OrderID"];
            //string comanyName = Request.QueryString["ComanyName"];
            //string startDate = Request.QueryString["StartDate"];
            //string endDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.InvoiceDetaiViewNew())
            {
                var view = query;

                if (!string.IsNullOrEmpty(orderID))
                {
                    orderID = orderID.Trim();
                    view = view.SearchByOrderID(orderID);
                }
                //if (!string.IsNullOrEmpty(comanyName))
                //{
                //    comanyName = comanyName.Trim();
                //    view = view.SearchByCompanyName(comanyName);
                //}
                //if (!string.IsNullOrEmpty(startDate))
                //{
                //    var from = DateTime.Parse(startDate);
                //    view = view.SearchByInvoiceTimeStartDate(from);
                //}
                //if (!string.IsNullOrEmpty(endDate))
                //{
                //    var to = DateTime.Parse(endDate);
                //    view = view.SearchByInvoiceTimeEndDate(to);
                //}

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

    }
}