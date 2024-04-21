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

namespace Yahv.Csrm.WebApp.Crm.Granule
{
    public partial class Consignees : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Client = new TradingClientsRoll()[Request.QueryString["id"]];
                this.Model.Admin = new AdminsAllRoll()[Request.QueryString["adminid"]];
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var consigneids = Erp.Current.Srm.Admins[Request["adminid"]].Consignees.Where(item => item.EnterpriseID == id).Select(item => item.ID).ToArray();
            var query = new TradingClientsRoll()[id].Consignees.Where(Predicate());
            return this.Paging(query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Address,
                District = item.District.GetDescription(),
                item.Postzip,
                item.DyjCode,
                ContactName = item.Name,
                item.Tel,
                item.Mobile,
                item.Email,
                item.Status,
                StatusName = item.Status.GetDescription(),
                EnterpriseID = item.EnterpriseID,
                Admin = item.Creator.RealName,
                IsChecked = consigneids.Contains(item.ID),
            }));

        }
        Expression<Func<TradingConsignee, bool>> Predicate()
        {
            Expression<Func<TradingConsignee, bool>> predicate = item => item.Status == ApprovalStatus.Normal;
            var address = Request["address"];
            var contactname = Request["contactname"];
            var tel = Request["tel"];
            var status = Request["status"];
            if (!string.IsNullOrWhiteSpace(address))
            {
                predicate = predicate.And(item => item.Address.Contains(address));
            }
            if (!string.IsNullOrWhiteSpace(contactname))
            {
                predicate = predicate.And(item => item.Name.Contains(contactname));
            }
            if (!string.IsNullOrWhiteSpace(tel))
            {
                predicate = predicate.And(item => item.Tel.Contains(tel) || item.Mobile.Contains(tel));
            }
            return predicate;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        protected JMessage Bind()
        {
            var consigneeid = Request["consigneeid"];
            var adminid = Request["adminid"];
            string clientid = Request["clientid"];
            if (string.IsNullOrWhiteSpace(consigneeid) || string.IsNullOrWhiteSpace(adminid))
            {
                return new JMessage
                {
                    success = false,
                    code = 100,
                    data = "绑定失败"
                };
            }
            else
            {
                Erp.Current.Crm.Admins[adminid].Binding(clientid, consigneeid, MapsType.Consignee);
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "绑定成功"
                };
            }

        }
        /// <summary>
        /// 解绑
        /// </summary>
        protected JMessage UnBind()
        {
            var consigneeid = Request["consigneeid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(consigneeid) || string.IsNullOrWhiteSpace(adminid))
            {
                return new JMessage
                {
                    success = false,
                    code = 100,
                    data = "绑定失败"
                };
            }
            else
            {
                Erp.Current.Crm.Admins[adminid].Unbind(consigneeid, MapsType.Consignee);
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "操作成功"
                };
            }

        }
    }
}