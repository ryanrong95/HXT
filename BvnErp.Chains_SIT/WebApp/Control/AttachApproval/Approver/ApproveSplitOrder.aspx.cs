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
    public partial class ApproveSplitOrder : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveSplitOrderInfo = view.GetApprovedResult(orderControlStepID);

            this.Model.ApproveSplitOrderInfo = new
            {
                OrderControlID = approveSplitOrderInfo.OrderControlID,
                OrderControlStepID = approveSplitOrderInfo.OrderControlStepID,
                ControlTypeDes = approveSplitOrderInfo.ControlType.GetDescription(),
                ApplicantName = approveSplitOrderInfo.ApplicantName,
                TinyOrderID = approveSplitOrderInfo.TinyOrderID,
                ClientName = approveSplitOrderInfo.ClientName,
                Currency = approveSplitOrderInfo.Currency,
                DeclarePrice = approveSplitOrderInfo.DeclarePrice,
                ApproveAdminName = approveSplitOrderInfo.ApproveAdminName,
                OrderControlStatusInt = (int)approveSplitOrderInfo.OrderControlStatus,
                OrderControlStatusDes = approveSplitOrderInfo.OrderControlStatus.GetDescription(),
                ApproveReason = approveSplitOrderInfo.ApproveReason,
            }.Json();

            //this.Model.ReferenceInfo = approveSplitOrderInfo.ReferenceInfo != null ? approveSplitOrderInfo.ReferenceInfo : string.Empty;

            // 显示拆分订单信息 Begin

            EventInfoSplitOrder eventInfoSplitOrder = JsonConvert.DeserializeObject<EventInfoSplitOrder>(approveSplitOrderInfo.EventInfo);
            this.Model.EventInfoSplitOrder = new
            {
                TinyOrderID = eventInfoSplitOrder.TinyOrderID,
                Packs = eventInfoSplitOrder.Packs,
            }.Json();

            // 显示拆分订单信息 End

            //查出审批日志 Begin

            var approveLogs = new Needs.Ccs.Services.Views.AttachApprovalLogView().GetResults(approveSplitOrderInfo.OrderControlID);
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
            var approveSplitOrderInfo = view.GetApprovedResult(orderControlStepID);

            string rInfo = approveSplitOrderInfo.ReferenceInfo != null ? approveSplitOrderInfo.ReferenceInfo : string.Empty;

            string referenceInfo = string.Empty;
            string referenceInfo2 = string.Empty;
            string[] arrInfo = rInfo.Split(new string[] { "这是一个超级分隔符" }, StringSplitOptions.None);
            if (arrInfo != null && arrInfo.Length == 2)
            {
                referenceInfo = arrInfo[0];
                referenceInfo2 = arrInfo[1];
            }


            Response.Write((new { success = true, referenceInfo = referenceInfo, referenceInfo2 = referenceInfo2, }).Json());
        }

    }
}