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
    public partial class ApproveChangeQuantity : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveChangeQuantityInfo = view.GetApprovedResult(orderControlStepID);

            this.Model.ApproveChangeQuantityInfo = new
            {
                OrderControlID = approveChangeQuantityInfo.OrderControlID,
                OrderControlStepID = approveChangeQuantityInfo.OrderControlStepID,
                ControlTypeDes = approveChangeQuantityInfo.ControlType.GetDescription(),
                ApplicantName = approveChangeQuantityInfo.ApplicantName,
                TinyOrderID = approveChangeQuantityInfo.TinyOrderID,
                ClientName = approveChangeQuantityInfo.ClientName,
                Currency = approveChangeQuantityInfo.Currency,
                DeclarePrice = approveChangeQuantityInfo.DeclarePrice,
                ApproveAdminName = approveChangeQuantityInfo.ApproveAdminName,
                OrderControlStatusInt = (int)approveChangeQuantityInfo.OrderControlStatus,
                OrderControlStatusDes = approveChangeQuantityInfo.OrderControlStatus.GetDescription(),
                ApproveReason = approveChangeQuantityInfo.ApproveReason,
            }.Json();

            // 显示修改数量信息 Begin

            EventInfoChangeQuantity eventInfoChangeQuantity = JsonConvert.DeserializeObject<EventInfoChangeQuantity>(approveChangeQuantityInfo.EventInfo);
            this.Model.EventInfoChangeQuantity = new
            {
                Model = eventInfoChangeQuantity.Model,
                Manufacturer = eventInfoChangeQuantity.Manufacturer,
                OldQuantity = eventInfoChangeQuantity.OldQuantity,
                NewQuantity = eventInfoChangeQuantity.NewQuantity,
            }.Json();

            // 显示修改数量信息 End

            //查出审批日志 Begin

            var approveLogs = new Needs.Ccs.Services.Views.AttachApprovalLogView().GetResults(approveChangeQuantityInfo.OrderControlID);
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
            var approveChangeQuantityInfo = view.GetApprovedResult(orderControlStepID);

            string referenceInfo = approveChangeQuantityInfo.ReferenceInfo != null ? approveChangeQuantityInfo.ReferenceInfo : string.Empty;

            Response.Write((new { success = true, referenceInfo = referenceInfo }).Json());
        }

    }
}