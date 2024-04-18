using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.AgentProxy
{
    /// <summary>
    /// 已审核代理报关委托书
    /// </summary>
    [Obsolete]
    public partial class AuditedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 已审核委托书列表
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            var purchaser = PurchaserContext.Current;
            

            var agentProxies = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderAgentProxies.Where(item => item.File != null && item.File.FileStatus == OrderFileStatus.Audited).AsQueryable();
            if (!string.IsNullOrEmpty(orderID))
            {
                agentProxies = agentProxies.Where(item => item.ID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                agentProxies = agentProxies.Where(item => item.Client.ClientCode.Contains(clientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                agentProxies = agentProxies.Where(item => item.CreateDate >= DateTime.Parse(startDate));
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                agentProxies = agentProxies.Where(item => item.CreateDate < DateTime.Parse(endDate).AddDays(1));
            }

            Func<Needs.Ccs.Services.Models.OrderAgentProxy, object> convert = agentProxy => new
            {
                agentProxy.ID,
                agentProxy.Client.ClientCode,
                ClientName = agentProxy.Client.Company.Name,
                AgentName = purchaser.CompanyName,
                DeclarePrice = agentProxy.DeclarePrice.ToRound(2).ToString("0.00"),
                agentProxy.Currency,
                CreateDate = agentProxy.CreateDate.ToShortDateString()
            };

            this.Paging(agentProxies, convert);
        }
    }
}