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

namespace Yahv.Csrm.WebApp.Prm.Contacts
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

            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = new CompaniesRoll()[id]?.Contacts.Where(Predicate());
            return new
            {
                rows = query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
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
        Expression<Func<Contact, bool>> Predicate()
        {
            Expression<Func<Contact, bool>> predicate = item => true;
            var name = Request["name"];
            var tel = Request["tel"];
            var mobile = Request["mobile"];
            var email = Request["email"];
            var status = Request["status"];
            var type = Request["type"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(tel))
            {
                predicate = predicate.And(item => item.Tel.Contains(tel));
            }
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                predicate = predicate.And(item => item.Mobile.Contains(mobile));
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
            var entity = new ContactsRoll().Where(item => ids.Contains(item.ID));
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Crm),
                                          "ContactDelete", "删除联系人：" + it.ID, "");
            }
            entity.Deleted();
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
                                          "ContactUnable", "停用联系人：" + it.ID, "");
            }
        }
    }
}