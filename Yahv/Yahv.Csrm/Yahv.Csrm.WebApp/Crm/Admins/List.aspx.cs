using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Admins
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ClientID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            string clientid = Request.QueryString["id"];
            var query = Erp.Current.Crm.Clients[clientid].Sales;

            return this.Paging(query.OrderBy(item => item.CreateDate), item => new
            {
                item.ID,
                item.UserName,
                item.RealName,
                item.RoleName,
                item.Company?.Name,
                CompanyID = item.Company?.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                IsDefault = item.IsDefault
            });
        }
        /// <summary>
        /// 绑定管理员
        /// </summary>
        protected void Binding()
        {
            var adminid = Request["adminid"];
            var clientid = Request["clientid"];
            //暂不设置合作公司
            //new TradingClientsRoll()[clientid].AdminBinding(Erp.Current.ID, adminid, Underly.Business.Trading_Sale);
            if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(clientid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "AdminBinding", "客户" + clientid + "绑定管理员" + adminid, "");
            }
        }
        /// <summary>
        /// 解除和管理员的绑定关系
        /// </summary>
        protected void Unbind()
        {
            var adminids = Request["adminids"];
            var clientid = Request["clientid"];
            //暂不设置合作公司
            // new TradingClientsRoll()[clientid].AdminUnbind(adminids.Split(','),Underly.Business.Trading_Sale);
            if (!string.IsNullOrEmpty(adminids) && !string.IsNullOrEmpty(clientid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "AdminUnbind", "客户" + clientid + "解除和管理员" + adminids + "的关系", "");
            }
        }
        /// <summary>
        /// 设为默认采购人
        /// </summary>
        protected void SetDefault()
        {
            var adminid = Request["adminid"];
            var clientid = Request["clientid"];
            //暂不设置合作公司
            // new TradingClientsRoll()[clientid].SetDefault(adminid, Underly.Business.Trading_Sale);
            if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(clientid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "SetDefault", "客户" + clientid + "，设置默认销售员：" + adminid, "");
            }
        }
    }
}