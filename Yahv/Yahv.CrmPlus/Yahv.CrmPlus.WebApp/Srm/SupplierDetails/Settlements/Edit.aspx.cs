using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Settlements
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new SupplierCreditsRoll()[Request.QueryString["id"]];
                //this.Model.ClearType = ExtendsEnum.ToArray(ClearType.Unknown).Select(item => new
                //{
                //    value = item,
                //    text = item.GetDescription()
                //});
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var entity = new SupplierCreditsRoll()[Request.QueryString["id"]];
            entity.Months = int.Parse(Request.Form["Months"]);
            entity.Days = int.Parse(Request.Form["Days"]);
            entity.Summary = Request.Form["Summary"];
            entity.ClearType = (ClearType)int.Parse(Request.Form["ClearType"]);
            entity.EnterSuccess += Entity_EnterSuccess; ;
            entity.Enter();

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Credit;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"编辑Credits:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}