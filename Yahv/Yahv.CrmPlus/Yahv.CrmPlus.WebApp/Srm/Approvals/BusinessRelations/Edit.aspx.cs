using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.Approvals.BusinessRelations
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["subid"];
                var entity = new BusinessRelationsRoll()[id];
                this.Model.Entity = entity;
                //this.Model.Files = new FilesDescriptionRoll().Where(item => item.EnterpriseID == entity.MainID && item.SubID == id && item.Type == Underly.CrmFileType.EnterpriseRelation);
            }
        }
        string context = "";
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                bool approveresult = false;
                bool result = bool.TryParse(this.Result.Value, out approveresult);
                context = approveresult ? "审批通过" : "审批不通过";
                string id = Request["subid"];
                var entity = new BusinessRelationsRoll()[id];
                AuditStatus status = approveresult ? AuditStatus.Normal : AuditStatus.Voted;
                entity.MainType = Underly.MainType.Suppliers;
                entity.TaskType = Underly.ApplyTaskType.SupplierBusinessRelation;
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Approve(status, Erp.Current.ID);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as BusinessRelation;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"Supplier关联关系{entity.ID}，{context}");
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}