using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Admins
{
    public partial class Selected_Admins : BasePage
    {
        protected string clientid
        {
            get
            {
                return Request.QueryString["id"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = new EnterprisesRoll()[clientid];
            }
        }
        protected object data()
        {
            var clientid = Request.QueryString["id"];

            var purchasers = Erp.Current.Crm.Clients[clientid].Sales;


            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                purchasers = purchasers.Where(item => item.ID == name || item.RealName.Contains(name) || item.SelCode.Contains(name));
            }
            return new
            {
                rows = purchasers.OrderBy(item => item.RealName).ToArray().Select(item => new
                {
                    item.ID,
                    item.RealName,
                    item.RoleName,
                    item.StaffID,
                    item.SelCode,
                    Status = item.Status.GetDescription(),
                    IsDefault = item.IsDefault
                })
            };
        }
        /// <summary>
        /// 绑定管理员
        /// </summary>
        protected void Binding()
        {
            var adminid = Request["adminid"];
            var clientid = Request["clientid"];
            new TradingClientsRoll()[clientid].AdminBinding(Erp.Current.ID, adminid);
            if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(clientid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "AdminBinding", "客户" + clientid + "绑定管理员" + adminid, "");
            }
        }
        #region 绑定合作公司与销售
        ///// <summary>
        ///// 绑定管理员
        ///// </summary>
        //protected void Binding()
        //{
        //    var adminid = Request["adminid"];
        //    var clientid = Request["clientid"];
        //    var client = new TradingClientsRoll()[clientid];
        //    client.CompanyID = "";
        //    client.AdminBinding(adminid);
        //    if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(clientid))
        //    {
        //        Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
        //                                 nameof(Yahv.Systematic.Crm),
        //                                "AdminBinding", "客户" + clientid + "绑定管理员" + adminid, "");
        //    }
        //}
        #endregion

        /// <summary>
        /// 解除和管理员的绑定关系
        /// </summary>
        protected void Unbind()
        {
            var adminids = Request["adminids"];
            var clientid = Request["clientid"];
            new TradingClientsRoll()[clientid].AdminUnbind(adminids.Split(','));
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
            new TradingClientsRoll()[clientid].SetDefault(adminid);
            if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(clientid))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "SetDefault", "客户" + clientid + "，设置默认销售员：" + adminid, "");
            }
        }
    }
}