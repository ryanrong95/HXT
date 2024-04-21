using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.EnumsDictionaries
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    this.Model.Enum = Request.QueryString["Enum"];
            //}
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string enumname = Request.QueryString["Enum"];
            string field = Request.Form["Field"].Trim();
            string decription = Request.Form["Description"].Trim();
            EnumsDictionary entity = new EnumsDictionary();
            entity.Enum = enumname;
            entity.Field = field;
            entity.Description = decription;
            entity.IsFixed = false;
            var maxvalue = new Service.Views.Rolls.EnumsDictionariesRoll().Where(item => item.Enum == enumname).Max(item => item.Value);
            int value = int.Parse(maxvalue);
            value++;
            entity.Value = value.ToString();
            entity.CreatorID = Erp.Current.ID;
            entity.Repeat += Entity_Repeat;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("保存失败,名称已经存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as EnumsDictionary;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"Enum新增:{entity.Json()}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}