using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.BusinessRelations
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
                string id = Request.QueryString["id"];
                var entity = new MapsEnterpriseExtendRoll()[id];
                this.Model.Entity =  new { MainName = entity.MainName, entity.SubName, BusinessRelationType=entity.BusinessRelationType.GetDescription(),};
                this.Model.Files = new FilesDescriptionRoll().Where(item => item.EnterpriseID == entity.MainID && item.SubID == id && item.Type == Underly.CrmFileType.EnterpriseRelation);
            //}
        }
        string context = "";
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                bool approveresult = false;
                bool result = bool.TryParse(this.Result.Value, out approveresult);
                context = approveresult ? "审批通过" : "审批不通过";
                string id = Request["id"];
                var entity = new MapsEnterpriseExtendRoll()[id];
                AuditStatus status = approveresult ? AuditStatus.Normal : AuditStatus.Voted;
                entity.CreatorID = Erp.Current.ID;
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
            var entity = sender as MapsEnterprise;
            if (entity.AuditStatus == AuditStatus.Normal)
            {
                (new ApplyTask
                {
                    MainID = entity.ID,
                    MainType = Underly.MainType.Clients,
                    ApplierID = entity.CreatorID,
                    ApplyTaskType = Underly.ApplyTaskType.ClientBusinessRelation,
                    Status = Underly.ApplyStatus.Allowed
                }).Approve();
            }
            else {

                (new ApplyTask
                {
                    MainID = entity.ID,
                    MainType = Underly.MainType.Clients,
                    ApplierID = entity.CreatorID,
                    ApplyTaskType = Underly.ApplyTaskType.ClientBusinessRelation,
                    Status = Underly.ApplyStatus.Voted
                }).Approve();

            }
           

            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"客户关联关系{entity.ID}，{context}");
            Easyui.Window.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}