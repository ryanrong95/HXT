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

namespace Yahv.Csrm.WebApp.Srm.Contacts
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
                this.Model.Entity = new ContactsRoll()[Request.QueryString["id"]];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string supplierid = Request.QueryString["supplierid"];
            string contactid = Request.QueryString["id"];
            var entity = Erp.Current.Srm.Suppliers[supplierid].Contacts[contactid] ?? new TradingContact();
            entity.EnterpriseID = supplierid;
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
            var entity = sender as TradingContact;
            if (string.IsNullOrEmpty(Request.QueryString["supplierid"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "ContactInsert", "新增联系人：" + entity.ID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "ContactUpdate", "修改联系人信息：" + entity.ID, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Yahv.Web.Controls.Easyui.Method.Dialog);
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}