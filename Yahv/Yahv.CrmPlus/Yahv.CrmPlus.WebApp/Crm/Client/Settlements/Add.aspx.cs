using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Settlements
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Companies = new CompaniesRoll().Where(item=>item.CompanyStatus==DataStatus.Normal).Select(item => new
                {
                    ID = item.ID,
                    Name = item.Name
                });
                this.Model.ClearType = ExtendsEnum.ToArray(ClearType.Unknown).Select(item => new
                {
                    value = item,
                    text = item.GetDescription()
                });
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var entity = new Credit();
            entity.MakerID = Request.Form["MakerID"];
            entity.TakerID = Request.QueryString["TakerID"];
            entity.Months = int.Parse(Request.Form["Months"]);
            entity.Days = int.Parse(Request.Form["Days"]);
            entity.Summary = Request.Form["Summary"];
            entity.Type = CreditType.GrantingParty;
            entity.ClearType = (ClearType)int.Parse(Request.Form["ClearType"]);
            entity.Repeat += Entity_Repeat;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Credit;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增Credits:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}