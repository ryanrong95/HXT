using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Contacts
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var contact = new ContactsRoll()[Request.QueryString["id"]];
                //下拉绑定
                //1.联系人类型
                this.Model.ContactType = ExtendsEnum.ToArray<ContactType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Entity = contact;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string contactid = Request.QueryString["id"];
            var Carrier = Erp.Current.Crm.Carriers[Request.QueryString["carrierid"]];
            var entity = Carrier.Contacts[contactid] ?? new YaHv.Csrm.Services.Models.Origins.Contact();

            entity.Enterprise = Carrier.Enterprise;
            entity.EnterpriseID = Carrier.ID;
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            entity.Type = (ContactType)int.Parse(Request["Type"]);
            if (string.IsNullOrWhiteSpace(contactid))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Web.Controls.Easyui.Method.Dialog);
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}