using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Projects.Compeletes
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = Request.QueryString["ID"];
            var entity = new Service.Views.Rolls.SalesChances.ProjectProductRoll()[id];
            this.Model.Entity = entity;
            //标准型号
            this.Model.StandardPartNumber = new CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll().Select(x => new { PartNumber = x.PartNumber, SpnID = x.ID, Brand = x.Brand });
        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            var projectProductID = Request.QueryString["ProjectProductID"];
            var projectID = Request.QueryString["ProjectID"];

            
            var partNumber = Request.Form["PartNumber"];
            var unitPrice = Request.Form["UnitPrice"];
            var product = new ProjectCompelete();
            product.ProjectID = projectID;
            product.ProjectProductID = projectProductID;
            product.SpnID = partNumber;
            product.UnitPrice =Convert.ToDecimal(unitPrice);
            product.CreatorID = Erp.Current.ID;
            product.DataStatus = Underly.DataStatus.Normal;
            product.EnterSuccess += Entity_EnterSuccess;
            product.Enter();

        }


        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as  ProjectCompelete;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增竞品ID:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

    }
}