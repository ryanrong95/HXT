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
    public partial class Contacts : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Client = new TradingClientsRoll()[Request.QueryString["id"]];
                this.Model.Admin = new AdminsAllRoll()[Request.QueryString["adminid"]]; this.Model.ContactType = ExtendsEnum.ToArray<ContactType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //状态
                this.Model.Status = ExtendsEnum.ToArray<YaHv.Csrm.Services.Status>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var mycontactids = Erp.Current.Srm.Admins[Request["adminid"]].Contacts.Where(item => item.EnterpriseID == id).Select(item => item.ID).ToArray();
            var query = new TradingClientsRoll()[id]?.Contacts.Where(Predicate());
            return this.Paging(query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                Type = item.Type.GetDescription(),
                item.Name,
                item.Tel,
                item.Mobile,
                item.Email,
                item.Status,
                StatusName = item.Status.GetDescription(),
                Admin = item.Creator.RealName,
                IsChecked = mycontactids.Contains(item.ID)
            }));
        }
        Expression<Func<TradingContact, bool>> Predicate()
        {
            Expression<Func<TradingContact, bool>> predicate = item => true;
            var name = Request["name"];
            var mobile = Request["mobile"];
            //var email = Request["email"];
            var type = Request["type"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                predicate = predicate.And(item => item.Mobile.Contains(mobile) || item.Tel.Contains(mobile));
            }
            //if (!string.IsNullOrWhiteSpace(email))
            //{
            //    predicate = predicate.And(item => item.Mobile.Contains(email));
            //}
            if (!string.IsNullOrWhiteSpace(type))
            {
                predicate = predicate.And(item => item.Type == (ContactType)int.Parse(type));
            }
            return predicate;
        }


        /// <summary>
        /// 绑定
        /// </summary>
        protected JMessage Bind()
        {
            var contactid = Request["contactid"];
            var adminid = Request["adminid"];
            string clientid = Request["clientid"];
            if (string.IsNullOrWhiteSpace(contactid) || string.IsNullOrWhiteSpace(adminid))
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
                Erp.Current.Crm.Admins[adminid].Binding(clientid, contactid, MapsType.Contact);
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
            var contactid = Request["contactid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(contactid) || string.IsNullOrWhiteSpace(adminid))
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
                Erp.Current.Crm.Admins[adminid].Unbind(contactid, MapsType.Contact);
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