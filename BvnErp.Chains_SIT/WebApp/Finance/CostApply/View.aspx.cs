using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CostApply
{
    public partial class View : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string CostApplyID = Request.QueryString["CostApplyID"];

            var costApplyDetail = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(CostApplyID);

            this.Model.CostApplyDetail = new
            {
                CostApplyID = costApplyDetail.CostApplyID,
                PayeeName = costApplyDetail.PayeeName,
                PayeeAccount = costApplyDetail.PayeeAccount,
                PayeeBank = costApplyDetail.PayeeBank,
                CostTypeInt = costApplyDetail.CostType.GetHashCode(),
                CostTypeStr = costApplyDetail.CostType.ToString(),
                FeeTypeInt = costApplyDetail.FeeType.GetHashCode(),
                FeeTypeStr = costApplyDetail.FeeType.ToString(),
                FeeDesc = costApplyDetail.FeeDesc,
                Amount = costApplyDetail.Amount,
                Currency = costApplyDetail.Currency,
                ApplicantID = costApplyDetail.ApplicantID,
                ApplicantName = costApplyDetail.ApplicantName,
                Summary = costApplyDetail.Summary,
                CostStatusInt = (int)costApplyDetail.CostStatus,
                MoneyTypeInt = (int)costApplyDetail.MoneyType,
                MoneyTypeName = costApplyDetail.MoneyType.GetDescription(),
                CashTypeInt = (int)costApplyDetail.CashType,
                CashTypeName = costApplyDetail.CashType.GetDescription(),
            }.Json();

            this.Model.Payers = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                                .Where(manager => manager.Role.Name.Contains("财务"))
                                .Select(item => new { PayerID = item.Admin.ID, PayerName = item.Admin.ByName }).Json();
        }

        /// <summary>
        /// 费用申请文件
        /// </summary>
        protected void CostApplyFiles()
        {
            string CostApplyID = Request.QueryString["CostApplyID"];

            var files = new Needs.Ccs.Services.Views.CostApplyFilesView().GetResults(CostApplyID);

            Func<Needs.Ccs.Services.Views.CostApplyFilesViewModel, object> convert = item => new
            {
                CostApplyFileID = item.CostApplyFileID,
                FileName = item.FileName,
                FileFormat = item.FileFormat,
                Url = item.Url,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
            }.Json());
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        protected void Approve()
        {
            try
            {
                string From = Request.Form["From"];
                string CostApplyID = Request.Form["CostApplyID"];
                string PayerID = Request.Form["PayerID"];
                string ApproveSummary = Request.Form["ApproveSummary"];

                var approval = new Needs.Ccs.Services.Models.CostApplyApproval(CostApplyID);

                Needs.Ccs.Services.Enums.CostStatusEnum currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.Cancel;
                Needs.Ccs.Services.Enums.CostStatusEnum nextCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.Cancel;
                if (From == "FinanceApprover")
                {
                    currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove;
                    nextCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove;
                }
                else if (From == "Approver")
                {
                    currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove;
                    nextCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.UnPay;
                }

                string rtnMsg = string.Empty;
                //var approveAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var approveAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                //var payerAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(PayerID);
                var payerAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == PayerID).FirstOrDefault();

                bool result = approval.Approve(approveAdmin, payerAdmin, ApproveSummary,
                                                currentCostStatus,
                                                nextCostStatus,
                                                out rtnMsg);

                if (result)
                {
                    Response.Write((new { success = true, message = "操作成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "错误：" + rtnMsg }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        protected void Refuse()
        {
            try
            {
                string From = Request.Form["From"];
                string CostApplyID = Request.Form["CostApplyID"];
                string ApproveSummary = Request.Form["ApproveSummary"];

                var approval = new Needs.Ccs.Services.Models.CostApplyApproval(CostApplyID);

                Needs.Ccs.Services.Enums.CostStatusEnum currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.Cancel;
                if (From == "FinanceApprover")
                {
                    currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove;
                }
                else if (From == "Approver")
                {
                    currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove;
                }

                string rtnMsg = string.Empty;
                //var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                bool result = approval.Refuse(admin, ApproveSummary, currentCostStatus, out rtnMsg);

                if (result)
                {
                    Response.Write((new { success = true, message = "操作成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "错误：" + rtnMsg }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        protected void Cancel()
        {
            try
            {
                string CostApplyID = Request.Form["CostApplyID"];

                var costApply = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(CostApplyID);

                var approval = new Needs.Ccs.Services.Models.CostApplyApproval(CostApplyID);

                string rtnMsg = string.Empty;
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                bool result = approval.Cancel(admin, costApply.CostStatus, out rtnMsg);

                if (result)
                {
                    Response.Write((new { success = true, message = "操作成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "错误：" + rtnMsg }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }
    }
}
