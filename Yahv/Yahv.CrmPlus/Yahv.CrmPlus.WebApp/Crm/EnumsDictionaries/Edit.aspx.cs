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
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new CrmPlus.Service.Views.Rolls.EnumsDictionariesRoll()[Request.QueryString["id"]];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var entity = new Service.Views.Rolls.EnumsDictionariesRoll()[Request.QueryString["id"]];
            if (entity == null)
            {
                Easyui.Alert("提示", "不存在!", Web.Controls.Easyui.Sign.Error);
            }
            //string field = Request.Form["Field"]?.Trim();
            string decription = Request.Form["Description"].Trim();
            //entity.Field = field;
            entity.Description = decription;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as EnumsDictionary;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"Enum修改:{entity.Json()}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}