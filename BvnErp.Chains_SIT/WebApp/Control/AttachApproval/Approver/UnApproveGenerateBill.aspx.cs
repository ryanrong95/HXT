using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
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
    public partial class UnApproveGenerateBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;
            string ReplaceSingleQuotes = "这里是一个单引号";
            this.Model.ReplaceSingleQuotes = ReplaceSingleQuotes;

            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveGenerateBillInfo = view.GetUnApproveResult(orderControlStepID);

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
            }.Json();

            //根据主订单号查出重新生成对账单页面，看到的内容 Begin

            var viewModel = getModel(approveGenerateBillInfo.MainOrderID);
            foreach (var t in viewModel.Bills)
            {
                foreach (var p in t.Products)
                {
                    p.Model = p.Model.Replace("\"", ReplaceQuotes).Replace("\'", ReplaceSingleQuotes);
                }
            }
            this.Model.Bill = viewModel.Json();

            //根据主订单号查出重新生成对账单页面，看到的内容 End

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

            //TODO:向后把IsHistory 逻辑完善
            this.Model.IsHistory = "0";

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

        protected MainOrderBillViewModel getModel(string id)
        {
            var viewModel = new MainOrderBillViewModel();
            var model = getModelStander(id);
            if (model == null)
            {
                return null;
            }
            else
            {
                #region 两个Model 转换              
                viewModel.MainOrderID = id;

                viewModel.Bills = model.Bills;

                var purchaser = PurchaserContext.Current;
                viewModel.AgentName = purchaser.CompanyName;
                viewModel.AgentAddress = purchaser.Address;
                viewModel.AgentTel = purchaser.Tel;
                viewModel.AgentFax = purchaser.UseOrgPersonTel;
                viewModel.Purchaser = purchaser.CompanyName;
                viewModel.Bank = purchaser.BankName;
                viewModel.Account = purchaser.AccountName;
                viewModel.AccountId = purchaser.AccountId;
                viewModel.SealUrl = PurchaserContext.Current.SealUrl.ToUrl();

                viewModel.ClientName = model.OrderBill.Client.Company.Name;
                viewModel.ClientTel = model.OrderBill.Client.Company.Contact.Tel;
                viewModel.Currency = model.OrderBill.Currency;
                viewModel.IsLoan = model.OrderBill.IsLoan;
                viewModel.DueDate = model.OrderBill.GetDueDate().ToString("yyyy年MM月dd日");
                viewModel.CreateDate = model.OrderBill.CreateDate.ToString();
                viewModel.ClientType = model.OrderBill.Client.ClientType;

                var OrderBillFile = model.OrderBillFile;

                viewModel.FileID = OrderBillFile?.ID;
                viewModel.FileStatus = OrderBillFile == null ? OrderFileStatus.NotUpload.GetDescription() :
                                        OrderBillFile.FileStatus.GetDescription();
                viewModel.FileName = OrderBillFile == null ? "" : OrderBillFile.Name;
                viewModel.Url = OrderBillFile == null ? "" : OrderBillFile.Url;
                //viewModel.FileStatusValue = OrderBillFile == null ? Needs.Wl.Models.Enums.OrderFileStatus.NotUpload : OrderBillFile.FileStatus;

                viewModel.Url = FileDirectory.Current.FileServerUrl + "/" + OrderBillFile?.Url.ToUrl();
                viewModel.summaryTotalPrice = model.BillTotalPrice;
                viewModel.summaryTotalCNYPrice = model.BillTotalCNYPrice;
                viewModel.summaryTotalTariff = model.BillTotalTariff;
                viewModel.summaryTotalAddedValueTax = model.BillTotalAddedValueTax;
                viewModel.summaryTotalAgencyFee = model.BillTotalAgencyFee;
                viewModel.summaryTotalIncidentalFee = model.BillTotalIncidentalFee;

                viewModel.summaryPay = model.BillTotalTaxAndFee;
                viewModel.summaryPayAmount = model.BillTotalDeclarePrice;


                viewModel.CreateDate = model.MainOrder.CreateDate.ToString("yyyy-MM-dd HH:mm");
                #endregion

                return viewModel;
            }
        }

        private MainOrderBillStander getModelStander(string id)
        {
            var Orders = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                                                  && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned)
                         .ToList();

            var purchaser = PurchaserContext.Current;
            if (Orders.Count == 0)
            {
                return null;
            }
            else
            {
                MainOrderBillStander mainOrderBillStander = new MainOrderBillStander(purchaser, Orders);

                return mainOrderBillStander;
            }
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        protected void ApproveOk()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.ApproveSuccess(admin, referenceInfo);
                attachApproval.ExecuteTargetOperation();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        protected void ApproveRefuse()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];
                string reason = Request.Form["ApproveCancelReason"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.ApproveRefuse(admin, referenceInfo, reason);

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        protected void ApproveCancel()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.CancelApply(admin, referenceInfo);

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

    }
}