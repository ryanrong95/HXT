using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Company.Contacts
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new CompaniesRoll()[Request.QueryString["companyid"]];
                Dictionary<string, string> gender = new Dictionary<string, string>();
                gender.Add("0", "全部");
                gender.Add("1", "女");
                gender.Add("2", "男");

                this.Model.Gender = gender.Select(item => new { value = item.Key, text = item.Value });
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            string ID = Request.QueryString["companyid"];

            var entity = new Yahv.CrmPlus.Service.Models.Origins.Contact();
            entity.EnterpriseID = ID;
            entity.RelationType = Underly.RelationType.Own;
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
            entity.OwnerID = Erp.Current.ID;
            var card = Request.Form["cardForJson"];
            if (!string.IsNullOrEmpty(card))
            {
                entity.Cards = card.JsonTo<List<CallFile>>();
            }

            entity.EnterError += Entity_EnterError;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }


        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Contact;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增联系人:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        }
    }
}