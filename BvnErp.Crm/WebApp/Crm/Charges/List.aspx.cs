using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Charges
{
    /// <summary>
    /// 客户费用展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string clientid = Request.QueryString["ClientID"];
            var charges = Needs.Erp.ErpPlot.Current.ClientSolutions.MyCharges;
            var data = charges.Where(item => item.Clients.ID == clientid);
            Func<Charge, object> linq = item => new
            {
                ID = item.ID,
                ClientName = item.Clients.Name,
                Name = item.Name,
                AdminName = item.Admin.RealName,
                Count = item.Count,
                Price = item.Price,
                CreateDate = item.CreateDate,
                Summary = item.Summary,
            };
            this.Paging(data,linq);                      
        }

    }
}