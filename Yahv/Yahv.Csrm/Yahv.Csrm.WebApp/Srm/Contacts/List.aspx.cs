using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Contacts
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
            //var query = new SuppliersRoll()[id]?.Contacts.Where(Predicate());

            var contacts = Erp.Current.Srm.Suppliers[id]?.Contacts.Where(Predicate()).OrderBy(item => item.CreateDate).ToList();

            return new
            {
                rows = contacts.Select(item => new
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
            //var tel = Request["tel"];
            var mobile = Request["mobile"];
            var email = Request["email"];
            var status = Request["status"];
            var type = Request["type"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
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
            var supplierid = Request.Form["supplierid"];
            if (Erp.Current.IsSuper)
            {
                var contacts = new ContactsRoll().Where(item => ids.Contains(item.ID));
                contacts.Deleted();
                foreach (var it in contacts)
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                               nameof(Yahv.Systematic.Srm),
                                              "ContactUpdate", "修改联系人状态（删除）：" + it.ID, "");
                }
            }
            else
            {
                var contacts = Erp.Current.Srm.Suppliers[supplierid].Contacts;
                foreach (var contact in contacts)
                {
                    contact.Abandon();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                              nameof(Yahv.Systematic.Srm),
                                             "ContactDeletes", "删除联系人：" + contact.ID, "");
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
                                           nameof(Yahv.Systematic.Srm),
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
                                           nameof(Yahv.Systematic.Srm),
                                          "ContactClosed", "停用联系人：" + it.ID, "");
            }
        }
    }
}