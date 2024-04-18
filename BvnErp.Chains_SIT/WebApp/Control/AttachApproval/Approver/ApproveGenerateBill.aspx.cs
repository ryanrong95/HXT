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
    public partial class ApproveGenerateBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveGenerateBillInfo = view.GetApprovedResult(orderControlStepID);

            this.Model.ApproveGenerateBillInfo = new
            {
                OrderControlID = approveGenerateBillInfo.OrderControlID,
                OrderControlStepID = approveGenerateBillInfo.OrderControlStepID,
                ControlTypeDes = approveGenerateBillInfo.ControlType.GetDescription(),
                ApplicantName = approveGenerateBillInfo.ApplicantName,
                MainOrderID = approveGenerateBillInfo.MainOrderID,
                ClientName = approveGenerateBillInfo.ClientName,
                Currency = approveGenerateBillInfo.Currency,
                DeclarePrice = approveGenerateBillInfo.DeclarePrice,
                ApproveAdminName = approveGenerateBillInfo.ApproveAdminName,
                OrderControlStatusInt = (int)approveGenerateBillInfo.OrderControlStatus,
                OrderControlStatusDes = approveGenerateBillInfo.OrderControlStatus.GetDescription(),
                ApproveReason = approveGenerateBillInfo.ApproveReason,
            }.Json();

            this.Model.ReferenceInfo = approveGenerateBillInfo.ReferenceInfo != null ? approveGenerateBillInfo.ReferenceInfo : string.Empty;

            // 显示修改前后汇率信息 Begin

            List<object> oldRateValue = new List<object>();
            List<object> newRateValue = new List<object>();
            EventInfoGenerateBill eventInfoGenerateBill = JsonConvert.DeserializeObject<EventInfoGenerateBill>(approveGenerateBillInfo.EventInfo);
            foreach (var tinyOrderInfo in eventInfoGenerateBill.TinyOrderInfos)
            {
                oldRateValue.Add(new
                {
                    OrderID = tinyOrderInfo.TinyOrderID,
                    CustomsExchangeRate = tinyOrderInfo.OldCustomsExchangeRate,
                    RealExchangeRate = tinyOrderInfo.OldRealExchangeRate,
                    OrderBillType = tinyOrderInfo.OldOrderBillTypeInt,
                    OrderBillTypeDes = GetOrderBillTypeDes(tinyOrderInfo.OldOrderBillTypeInt), //((Needs.Ccs.Services.Enums.OrderBillType)tinyOrderInfo.OldOrderBillTypeInt).GetDescription(),
                    RealAgencyFee = tinyOrderInfo.OldAgencyFeeUnitPrice,
                });

                newRateValue.Add(new
                {
                    OrderID = tinyOrderInfo.TinyOrderID,
                    CustomsExchangeRate = tinyOrderInfo.NewCustomsExchangeRate,
                    RealExchangeRate = tinyOrderInfo.NewRealExchangeRate,
                    OrderBillType = tinyOrderInfo.NewOrderBillTypeInt,
                    OrderBillTypeDes = GetOrderBillTypeDes(tinyOrderInfo.NewOrderBillTypeInt), //((Needs.Ccs.Services.Enums.OrderBillType)tinyOrderInfo.NewOrderBillTypeInt).GetDescription(),
                    RealAgencyFee = tinyOrderInfo.NewAgencyFeeUnitPrice,
                });
            }

            this.Model.OldExchangeRateValue = oldRateValue.Json();
            this.Model.NewExchangeRateValue = newRateValue.Json();
            // [{"OrderID":"NL02020200121001-01","CustomsExchangeRate":7.0118,"RealExchangeRate":7.0154,"OrderBillType":1,"RealAgencyFee":0.0}]

            // 显示修改前后汇率信息 End

            //查出审批日志 Begin

            var approveLogs = new Needs.Ccs.Services.Views.AttachApprovalLogView().GetResults(approveGenerateBillInfo.OrderControlID);
            this.Model.ApproveLogs = approveLogs.Select(t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = t.Summary,
            }).Json();

            //查出审批日志 End
        }

        private string GetOrderBillTypeDes(int OrderBillTypeInt)
        {
            string des = string.Empty;
            switch (OrderBillTypeInt)
            {
                case (int)Needs.Ccs.Services.Enums.OrderBillType.Normal:
                    des = "正常";
                    break;
                case (int)Needs.Ccs.Services.Enums.OrderBillType.MinAgencyFee:
                    des = "实际费用";
                    break;
                case (int)Needs.Ccs.Services.Enums.OrderBillType.Pointed:
                    des = "指定费用";
                    break;
                default:
                    break;
            }
            return des;
        }

    }
}