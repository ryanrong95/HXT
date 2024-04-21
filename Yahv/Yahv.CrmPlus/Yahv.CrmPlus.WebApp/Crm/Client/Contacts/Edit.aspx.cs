using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Contacts
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = Request.Form["Name"];
            string department = Request.Form["Department"];
            string position = Request.Form["Positon"];
            string mobile = Request.Form["Mobile"].Trim();
            string address = Request.Form["Address"];
            string tel = Request.Form["Tel"];
            string gender = Request.Form["gender"];
            string wx = Request.Form["Wx"].Trim();
            string QQ = Request.Form["QQ"];
            string skype = Request.Form["Skype"];
            string character = Request.Form["Character"];
            string taboo = Request.Form["Taboo"].Trim();
            string email = Request.Form["Email"];
            string ID = Request.QueryString["id"];

            var entity = new Yahv.CrmPlus.Service.Models.Origins.Contact();
            entity.EnterpriseID = ID;
            entity.RelationType = Underly.RelationType.Trade;
            entity.Name = name;
            entity.Mobile = mobile;
            entity.Tel = tel;
            entity.Skype = skype;
            entity.Positon = position;
            entity.Department = department;
            entity.Gender = gender;
            entity.QQ = QQ;
            entity.Wx = wx;
            entity.Taboo = taboo;
            entity.Character = character;
            entity.Email = email;
            entity.OwnerID = Erp.Current.ID;
            #region 名片
            var card = Request.Form["CardForJson"];
            entity.Cards = card == null ? null : card.JsonTo<List<CallFile>>();
            #endregion
            entity.EnterSuccess += Entity_EnterSuccess; ;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Contact;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增联系人:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}