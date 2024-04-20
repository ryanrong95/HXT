using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Serializers;
using Needs.Web;
using System.Linq.Expressions;
using Needs.Utils.Linq;
using Needs.Utils.Descriptions;

namespace WebApp.Clients
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            Expression<Func<NtErp.Wss.Services.Generic.Models.ClientTop, bool>> expression = null;
            string queryID = Request.QueryString["ID"],
                   queryUserName = Request.QueryString["UserName"],
                   queryMobile = Request.QueryString["Mobile"],
                   queryEmail = Request.QueryString["Email"];

            if (!string.IsNullOrWhiteSpace(queryID))
            {
                expression = expression.And(item => item.ID == queryID);
            }
            if (!string.IsNullOrWhiteSpace(queryUserName))
            {
                expression = expression.And(item => item.UserName.StartsWith(queryUserName));
            }
            if (!string.IsNullOrWhiteSpace(queryMobile))
            {
                expression = expression.And(item => item.Mobile.EndsWith(queryMobile));
            }
            if (!string.IsNullOrWhiteSpace(queryEmail))
            {
                expression = expression.And(item => item.Email.Contains(queryEmail));
            }

            using (var view = Needs.Erp.ErpPlot.Current.Websites.MyClients)
            {
                using (Needs.Linq.LinqContext.Current)
                {
                    IQueryable<NtErp.Wss.Services.Generic.Models.ClientTop> data = view;
                    if (expression != null)
                    {
                        data = data.Where(expression);
                    }
                    Response.Paging(data.OrderByDescending(t => t.CreateDate), item => new
                    {
                        item.ID,
                        item.UserName,
                        item.Mobile,
                        item.Email,
                        item.Status,
                        //StatusName = item.Status,
                        CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
        }
    }
}