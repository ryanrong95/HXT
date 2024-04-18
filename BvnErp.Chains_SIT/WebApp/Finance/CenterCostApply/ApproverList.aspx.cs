using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CenterCostApply
{
    /// <summary>
    /// 费用申请-待审批
    /// </summary>
    public partial class ApproverList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> dicFeeType = new Dictionary<string, string>();
            foreach (int item in Enum.GetValues(typeof(Needs.Ccs.Services.Enums.FeeTypeEnum)))
            {
                string strName = Enum.GetName(typeof(Needs.Ccs.Services.Enums.FeeTypeEnum), item);
                string strVaule = item.ToString();
                dicFeeType.Add(strVaule, strName);
            }
            dicFeeType.Remove("0");
            this.Model.FeeType = dicFeeType.Select(item => new { item.Key, item.Value }).Json();

            this.Model.Payers = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                                .Where(manager => manager.Role.Name.Contains("财务"))
                                .Select(item => new { PayerID = item.Admin.ID, PayerName = item.Admin.ByName }).Json();

            //Dictionary<string, string> dicCostStatus = new Dictionary<string, string>();
            //Needs.Ccs.Services.Enums.CostStatusEnum[] showCostStatusEnums = new Needs.Ccs.Services.Enums.CostStatusEnum[]
            //{
            //    Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove,
            //    Needs.Ccs.Services.Enums.CostStatusEnum.UnPay,
            //    Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess,
            //};
            //foreach (var item in showCostStatusEnums)
            //{
            //    dicCostStatus.Add(Convert.ToString(item.GetHashCode()), item.GetDescription());
            //}
            //this.Model.CostStatus = dicCostStatus.Select(item => new { item.Key, item.Value }).Json();
        }

        protected void ApproverListData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string PayeeName = Request.QueryString["PayeeName"];
            string FeeType = Request.QueryString["FeeType"];
            //string CostStatus = Request.QueryString["CostStatus"];
            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            int[] ShowCostStatusInt = (new Needs.Ccs.Services.Enums.CostStatusEnum[]
            {
                Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove,
            }).Select(t => (int)t).ToArray();
            lambdas.Add((Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>>)(t => ShowCostStatusInt.Contains(t.CostStatusInt)));

            if (!string.IsNullOrEmpty(PayeeName))
            {
                PayeeName = PayeeName.Trim();
                Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.PayeeName.Contains(PayeeName);
                lambdas.Add(lambda);
            }
            //if (!string.IsNullOrEmpty(FeeType))
            //{
            //    int FeeTypeInt = int.Parse(FeeType);
            //    Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.FeeType == (Needs.Ccs.Services.Enums.FeeTypeEnum)FeeTypeInt;
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(CostStatus))
            //{
            //    int CostStatusInt = int.Parse(CostStatus);
            //    Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CostStatus == (Needs.Ccs.Services.Enums.CostStatusEnum)CostStatusInt;
            //    lambdas.Add(lambda);
            //}
            if (!string.IsNullOrEmpty(CreateDateBegin))
            {
                var from = DateTime.Parse(CreateDateBegin);
                Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(CreateDateEnd))
            {
                var to = DateTime.Parse(CreateDateEnd);
                Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }

            int totalCount = 0;
            var view = new Needs.Ccs.Services.Views.ApplicantListView();
            var applicantList = view.GetResult(out totalCount, page, rows, lambdas.ToArray());

            Response.Write(new
            {
                rows = applicantList.Select(
                        item => new
                        {
                            CostApplyID = item.CostApplyID,
                            PayeeName = item.PayeeName,
                            //CostType = item.CostType.ToString(),
                            //FeeType = item.FeeType.ToString(),
                            //FeeTypeName = item.FeeType == Needs.Ccs.Services.Enums.FeeTypeEnum.其它 ? item.FeeDesc : item.FeeType.ToString(),
                            //FeeTypeName = item.CostType == Needs.Ccs.Services.Enums.CostTypeEnum.费用 ? (item.FeeType == Needs.Ccs.Services.Enums.FeeTypeEnum.其它 ? item.FeeDesc : item.FeeType.ToString()) : "",
                            Amount = item.Amount,
                            Currency = item.Currency,
                            CostStatusInt = item.CostStatus.GetHashCode(),
                            CostStatus = item.CostStatus.GetDescription(),
                            PayTime = item.PayTime?.ToString("yyyy-MM-dd"),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                        }
                     ).ToArray(),
                total = totalCount,
            }.Json());
        }

        /// <summary>
        /// 批量审批
        /// </summary>
        protected void BatchApprove()
        {
            try
            {
                string[] CostApplyIDs = Request.Form["CostApplyIDs"].Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
                string RadioVal = Request.Form["RadioVal"];
                string PayerID = Request.Form["Payer"];
                string ApproveSummary = Request.Form["ApproveSummary"];

                Needs.Ccs.Services.Enums.CostStatusEnum currentCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove;
                Needs.Ccs.Services.Enums.CostStatusEnum nextCostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.UnPay;

                var approveAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var payerAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(PayerID);

                foreach (var costApplyID in CostApplyIDs)
                {
                    string rtnMsg = string.Empty;
                    var approval = new Needs.Ccs.Services.Models.CostApplyApproval(costApplyID);

                    if (RadioVal == "1")
                    {
                        bool result = approval.Approve(approveAdmin, payerAdmin, ApproveSummary,
                                                        currentCostStatus,
                                                        nextCostStatus,
                                                        out rtnMsg);
                    }
                    else if (RadioVal == "2")
                    {
                        bool result = approval.Refuse(approveAdmin, ApproveSummary, currentCostStatus, out rtnMsg);
                    }
                }

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }


    }
}
