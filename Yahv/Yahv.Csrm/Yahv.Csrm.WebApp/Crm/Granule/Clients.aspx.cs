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
    public partial class Clients : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Admin = new AdminsAllRoll()[Request.QueryString["id"]];
            }
        }
        protected object data()
        {
            var clientsids = Erp.Current.Crm.Admins[Request["id"]].Clients.Select(item => item.ID).ToArray();
            var query = Erp.Current.Crm.Clients.Where(Predicate());

            return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.TaxperNumber,
                item.Grade,
                item.DyjCode,
                item.Vip,
                Type = item.AreaType.GetDescription(),
                Nature = item.Nature.GetDescription(),
                item.Enterprise.District,
                item.ClientStatus,
                StatusName = item.ClientStatus.GetDescription(),
                Admin = item.Creator.RealName,
                IsChecked = clientsids.Contains(item.ID),
            });
        }
        Expression<Func<TradingClient, bool>> Predicate()
        {
            Expression<Func<TradingClient, bool>> predicate = item => true;
            var name = Request["s_name"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            return predicate;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        protected JMessage Bind()
        {
            var clientid = Request["clientid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(clientid) || string.IsNullOrWhiteSpace(adminid))
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
                Erp.Current.Crm.Admins[adminid].Binding(clientid, adminid, MapsType.Client);
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
            var clientid = Request["clientid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(clientid) || string.IsNullOrWhiteSpace(adminid))
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
                Erp.Current.Crm.Admins[adminid].Unbind(clientid, MapsType.Client);
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