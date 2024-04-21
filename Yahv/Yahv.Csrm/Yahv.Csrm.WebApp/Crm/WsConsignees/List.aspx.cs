using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;


namespace Yahv.Csrm.WebApp.Crm.WsConsignees
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Client = new WsClientsRoll()[Request.QueryString["id"]].Enterprise;
                Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "0", "全部" } };
                statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
                statuslist.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
                statuslist.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
                statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
                //状态
                this.Model.Status = statuslist.Select(item => new
                {
                    value = item.Key,
                    text = item.Value
                });
                //来源
                EnterpriseType type = (EnterpriseType)int.Parse(Request["enterprisetype"]);
                this.Model.EnterpriseType = type;
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var consignee = Erp.Current.Whs.WsClients[id].Consignees.Where(Predicate());
            return new
            {
                rows = consignee.ToArray().Select(item => new
                {
                    item.ID,
                    item.Address,
                    District = item.District.GetDescription(),
                    Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
                    item.Postzip,
                    item.DyjCode,
                    ContactName = item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    EnterpriseID = item.EnterpriseID,
                    Admin = item.Creator == null ? null : item.Creator.RealName,
                    item.Title,
                    IsDefault = item.IsDefault ? "是" : "否",
                })
            };
        }
        Expression<Func<WsConsignee, bool>> Predicate()
        {
            Expression<Func<WsConsignee, bool>> predicate = item => true;
            ;
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
        protected void del()
        {
            var wsclient = Erp.Current.Whs.WsClients[Request["clientid"]];

            var entity = wsclient.Consignees[Request["id"]];
            entity.Abandon();
            string api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (!string.IsNullOrWhiteSpace(api))
            {
                Unify(api, entity);
            }
        }
        object Unify(string api, Consignee consignee)
        {
            var response = HttpClientHelp.CommonHttpRequest(api + "/clients/consignee?name=" + consignee.Enterprise.Name + "&receiver=" + consignee.Enterprise.Name + "&address=" + consignee.Address.Replace("中国 ", "") + "&mobile=" + consignee.Mobile, "DELETE");
            return response;
        }

    }
}