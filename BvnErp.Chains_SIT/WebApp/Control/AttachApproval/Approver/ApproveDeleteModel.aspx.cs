using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.AttachApproval.Approver
{
    public partial class ApproveDeleteModel : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveDeleteModelInfo = view.GetApprovedResult(orderControlStepID);

            this.Model.ApproveDeleteModelInfo = new
            {
                OrderControlID = approveDeleteModelInfo.OrderControlID,
                OrderControlStepID = approveDeleteModelInfo.OrderControlStepID,
                ControlTypeDes = approveDeleteModelInfo.ControlType.GetDescription(),
                ApplicantName = approveDeleteModelInfo.ApplicantName,
                TinyOrderID = approveDeleteModelInfo.TinyOrderID,
                ClientName = approveDeleteModelInfo.ClientName,
                Currency = approveDeleteModelInfo.Currency,
                DeclarePrice = approveDeleteModelInfo.DeclarePrice,
                ApproveAdminName = approveDeleteModelInfo.ApproveAdminName,
                OrderControlStatusInt = (int)approveDeleteModelInfo.OrderControlStatus,
                OrderControlStatusDes = approveDeleteModelInfo.OrderControlStatus.GetDescription(),
                ApproveReason = approveDeleteModelInfo.ApproveReason,
            }.Json();

            //this.Model.ReferenceInfo = approveDeleteModelInfo.ReferenceInfo != null ? approveDeleteModelInfo.ReferenceInfo : string.Empty;

            // 显示删除型号信息 Begin

            EventInfoDeleteModel eventInfoDeleteModel = JsonConvert.DeserializeObject<EventInfoDeleteModel>(approveDeleteModelInfo.EventInfo);
            this.Model.EventInfoDeleteModel = new
            {
                Model = eventInfoDeleteModel.Model,
                Manufacturer = eventInfoDeleteModel.Manufacturer,
                Quantity = eventInfoDeleteModel.Quantity,
            }.Json();

            // 显示删除型号信息 End

            //查出审批日志 Begin

            var approveLogs = new Needs.Ccs.Services.Views.AttachApprovalLogView().GetResults(approveDeleteModelInfo.OrderControlID);
            this.Model.ApproveLogs = approveLogs.Select(t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = t.Summary,
            }).Json();

            //查出审批日志 End
        }

        protected void GetReferenceInfo()
        {
            string orderControlStepID = Request.Form["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveDeleteModelInfo = view.GetApprovedResult(orderControlStepID);

            string referenceInfo = approveDeleteModelInfo.ReferenceInfo != null ? approveDeleteModelInfo.ReferenceInfo : string.Empty;

            Response.Write((new { success = true, referenceInfo = referenceInfo }).Json());
        }

    }
}