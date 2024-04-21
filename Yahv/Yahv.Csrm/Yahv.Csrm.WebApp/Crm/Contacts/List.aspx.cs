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

namespace Yahv.Csrm.WebApp.Crm.Contacts
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new EnterprisesRoll()[Request.QueryString["id"]];
                Dictionary<string, string> list = new Dictionary<string, string>() { { "-100", "全部" } };
                this.Model.ContactType = list.Concat(ExtendsEnum.ToDictionary<ContactType>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()
                });
                //状态
                this.Model.Status = list.Concat(ExtendsEnum.ToDictionary<YaHv.Csrm.Services.Status>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()
                });
                //来源
                EnterpriseType type = (EnterpriseType)int.Parse(Request["enterprisetype"]);
                this.Model.EnterpriseType = (int)type;
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            //  EnterpriseType type = (EnterpriseType)int.Parse(Request["enterprisetype"]);
            var client = Erp.Current.Crm.Clients[id].Enterprise;
            var contacts = Erp.Current.Crm.MyContacts[client].Where(Predicate()).OrderBy(item => item.CreateDate);
            return new
            {
                rows = contacts.ToArray().Select(item => new
                {
                    item.ID,
                    Type = item.Type.GetDescription(),
                    item.Name,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.Status,
                    StatusName = item.Status.GetDescription(),
                    Admin = item.Creator == null ? null : item.Creator.RealName
                })
            };
        }
        Expression<Func<TradingContact, bool>> Predicate()
        {
            Expression<Func<TradingContact, bool>> predicate = item => true;

            var name = Request["name"];
            // var tel = Request["tel"];
            var mobile = Request["mobile"];
            var email = Request["email"];
            var status = Request["status"];
            var type = Request["type"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
            //if (!string.IsNullOrWhiteSpace(tel))
            //{
            //    predicate = predicate.And(item => item.Tel.Contains(tel));
            //}
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                predicate = predicate.And(item => item.Mobile.Contains(mobile) || item.Tel.Contains(mobile));
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                predicate = predicate.And(item => item.Mobile.Contains(email));
            }
            if (type != "-100")
            {
                predicate = predicate.And(item => item.Type == (ContactType)int.Parse(type));
            }
            if (status != "-100")
            {
                predicate = predicate.And(item => item.Status == (YaHv.Csrm.Services.Status)int.Parse(status));
            }
            return predicate;
        }
        protected void Del()
        {
            var ids = Request.Form["items"].Split(',');
            string clientid = Request.Form["clientid"];

            if (Erp.Current.IsSuper)
            {
                var entity = new ContactsRoll().Where(item => ids.Contains(item.ID));
                entity.Deleted();//修改状态为删除状态
                foreach (var it in entity)
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Crm),
                                              "ContactUpdate", "修改联系人状态（删除）：" + it.ID, "");
                }
            }
            else
            {
                var client = Erp.Current.Crm.Clients[clientid].Enterprise;
                var entity = Erp.Current.Crm.MyContacts[client].Where(item => ids.Contains(item.ID));
                foreach (var it in entity)
                {
                    it.Abandon();//删除关系
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Crm),
                                              "ContactDeleted", "删除联系人：" + it.ID, "");
                }
            }

        }
        protected void Enable()
        {
            var ids = Request.Form["items"].Split(',');
            var entity = new ContactsRoll().Where(item => ids.Contains(item.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ContactEnable", "启用联系人：" + it.ID, "");
            }
        }
        protected void Unable()
        {
            var ids = Request.Form["items"].Split(',');
            var entity = new ContactsRoll().Where(item => ids.Contains(item.ID));
            entity.Closed();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ContactClosed", "停用联系人：" + it.ID, "");
            }
        }
    }
}