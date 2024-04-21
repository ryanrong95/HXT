using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.ApprovalRecords;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.ProtectApply
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected object data()
        {
            var applytask = new ProtectedRecordsRoll();
            return this.Paging(applytask.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.MainID,
                item.ApproverID,
                item.ApplyerName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
                StatusDes = item.Status.GetDescription(),
            }));
        }


        protected void Approve()
        {
            bool result = bool.Parse(Request["result"]);
            string id = Request["id"];
            var entity =  new  ProtectedRecordsRoll()[id];
            Underly.ApplyStatus status = result ? Underly.ApplyStatus.Allowed : Underly.ApplyStatus.Voted;
            entity.Status = status;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Approve(Erp.Current.ID);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var apply = sender as Yahv.CrmPlus.Service.Models.Origins.Rolls.ProtectApply;
            Service.LogsOperating.LogOperating(Erp.Current, apply.ApproverID, $"审批通过了保护,{apply.Name}");
            Response.Write(new { success = true, data = "", message = "" }.Json());
        }


        /// <summary>
        /// 取消保护
        /// </summary>
        protected void Cancel()
        {


        }
    }
}