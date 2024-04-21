using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Contacts
{
    public partial class WsEdit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
                var contact = wsclient.Contact;
                //下拉绑定
                //1.联系人类型
                this.Model.ContactType = ExtendsEnum.ToArray<ContactType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Entity = contact;
                this.Model.HidSave = wsclient.ServiceManager == null;
                this.Model.Nonstandard = wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase);
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            //try
            //{
            string clientid = Request.QueryString["id"];
            var wsclient = Erp.Current.Whs.WsClients[clientid];
            if (wsclient.ServiceManager == null)//要先分配业务员
            {
                Easyui.Reload("提示!", "请先分配业务员", Web.Controls.Easyui.Sign.None);
            }
            else if (wsclient.Enterprise.Name.StartsWith("reg-",StringComparison.OrdinalIgnoreCase))
            {
                Easyui.Reload("提示!", "请先规范客户名称", Web.Controls.Easyui.Sign.None);
            }
            else
            {
                var contact = wsclient.Contact;
                var entity = new WsContact();
                entity.Name = Request.Form["Name"].Trim();
                entity.Tel = Request.Form["Tel"].Trim();
                entity.Mobile = Request.Form["Mobile"].Trim();
                entity.Email = Request.Form["Email"].Trim();
                entity.Type = (ContactType)int.Parse(Request["Type"]);
                entity.Fax = Request.Form["Fax"];
                entity.EnterpriseID = clientid;
                entity.IsDefault = true;
                if (contact == null)
                {
                    entity.CreatorID = Yahv.Erp.Current.ID;
                }
                else
                {
                    entity.CreatorID = contact.CreatorID;
                }
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
            }
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var data = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
            string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (!string.IsNullOrWhiteSpace(apiurl))
            {
                Unify(apiurl, data);
            }
            Easyui.Reload("提示!", "保存成功", Web.Controls.Easyui.Sign.None);
        }

        object Unify(string api, WsClient data)
        {
            var grade = (int)data.Grade;
            var bussiness = data.BusinessLicense == null ? null : new
            {
                CompanyID = Request.QueryString["CompanyID"],
                Name = data.BusinessLicense.CustomName,
                Type = FileType.BusinessLicense,
                Url = data.BusinessLicense.Url,
               // FileFormat = "",
                CreatorID = data.BusinessLicense.AdminID
            };
            var response = HttpClientHelp.HttpPostRaw(api + "/clients", new
            {
                Enterprise = data.Enterprise,
                EnterCode = data.EnterCode,
                Contact = data.Contact,
                CustomsCode = data.CustomsCode,
                Rank = grade,
                BusinessLicense = bussiness,
                Creator = data.ServiceManager == null ? "" : data.ServiceManager.RealName
            }.Json());
            return response;
        }
    }
}