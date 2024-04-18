using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CenterCostApply.FinanceStaff
{
    public partial class ApproverListApped : Uc.PageBase
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

            Dictionary<string, string> dicCostStatus = new Dictionary<string, string>();
            Needs.Ccs.Services.Enums.CostStatusEnum[] showCostStatusEnums = new Needs.Ccs.Services.Enums.CostStatusEnum[]
            {
                Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove,
                Needs.Ccs.Services.Enums.CostStatusEnum.UnPay,
                Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess,
            };
            foreach (var item in showCostStatusEnums)
            {
                dicCostStatus.Add(Convert.ToString(item.GetHashCode()), item.GetDescription());
            }
            this.Model.CostStatus = dicCostStatus.Select(item => new { item.Key, item.Value }).Json();
        }

        //protected void ApproverListData()
        //{
        //    int page, rows;
        //    int.TryParse(Request.QueryString["page"], out page);
        //    int.TryParse(Request.QueryString["rows"], out rows);

        //    string PayeeName = Request.QueryString["PayeeName"];
        //    string FeeType = Request.QueryString["FeeType"];
        //    string CostStatus = Request.QueryString["CostStatus"];
        //    string CreateDateBegin = Request.QueryString["CreateDateBegin"];
        //    string CreateDateEnd = Request.QueryString["CreateDateEnd"];

        //    List<LambdaExpression> lambdas = new List<LambdaExpression>();
        //    int[] ShowCostStatusInt = (new Needs.Ccs.Services.Enums.CostStatusEnum[]
        //    {
        //        Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove,
        //        Needs.Ccs.Services.Enums.CostStatusEnum.UnPay,
        //        Needs.Ccs.Services.Enums.CostStatusEnum.PaySuccess,
        //    }).Select(t => (int)t).ToArray();
        //    lambdas.Add((Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>>)(t => ShowCostStatusInt.Contains(t.CostStatusInt)));

        //    if (!string.IsNullOrEmpty(PayeeName))
        //    {
        //        PayeeName = PayeeName.Trim();
        //        Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.PayeeName.Contains(PayeeName);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(FeeType))
        //    {
        //        int FeeTypeInt = int.Parse(FeeType);
        //        Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.FeeType == (Needs.Ccs.Services.Enums.FeeTypeEnum)FeeTypeInt;
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(CostStatus))
        //    {
        //        int CostStatusInt = int.Parse(CostStatus);
        //        Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CostStatus == (Needs.Ccs.Services.Enums.CostStatusEnum)CostStatusInt;
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(CreateDateBegin))
        //    {
        //        var from = DateTime.Parse(CreateDateBegin);
        //        Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CreateDate >= from;
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(CreateDateEnd))
        //    {
        //        var to = DateTime.Parse(CreateDateEnd);
        //        Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CreateDate < to.AddDays(1);
        //        lambdas.Add(lambda);
        //    }

        //    int totalCount = 0;
        //    var view = new Needs.Ccs.Services.Views.ApplicantListView();
        //    var applicantList = view.GetResult(out totalCount, page, rows, lambdas.ToArray());

        //    Response.Write(new
        //    {
        //        rows = applicantList.Select(
        //                item => new
        //                {
        //                    CostApplyID = item.CostApplyID,
        //                    PayeeName = item.PayeeName,
        //                    //CostType = item.CostType.ToString(),
        //                    FeeType = item.FeeType.ToString(),
        //                    FeeTypeName = item.FeeType == Needs.Ccs.Services.Enums.FeeTypeEnum.其它 ? item.FeeDesc : item.FeeType.ToString(),
        //                    Amount = item.Amount,
        //                    Currency = item.Currency,
        //                    CostStatusInt = item.CostStatus.GetHashCode(),
        //                    CostStatus = item.CostStatus.GetDescription(),
        //                    PayTime = item.PayTime?.ToString("yyyy-MM-dd"),
        //                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
        //                }
        //             ).ToArray(),
        //        total = totalCount,
        //    }.Json());
        //}

        protected void ApproverListData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string PayeeName = Request.QueryString["PayeeName"];            
            string CostStatus = Request.QueryString["CostStatus"];
            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];

            List<LambdaExpression> lambdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(PayeeName))
            {
                PayeeName = PayeeName.Trim();
                Expression<Func<Needs.Ccs.Services.Views.ApprovedListViewModel, bool>> lambda = item => item.PayeeName.Contains(PayeeName);
                lambdas.Add(lambda);
            }
            
            if (!string.IsNullOrEmpty(CostStatus))
            {
                int CostStatusInt = int.Parse(CostStatus);
                Expression<Func<Needs.Ccs.Services.Views.ApprovedListViewModel, bool>> lambda = item => item.CostStatus == (Needs.Ccs.Services.Enums.CostStatusEnum)CostStatusInt;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(CreateDateBegin))
            {
                var from = DateTime.Parse(CreateDateBegin);
                Expression<Func<Needs.Ccs.Services.Views.ApprovedListViewModel, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(CreateDateEnd))
            {
                var to = DateTime.Parse(CreateDateEnd);
                Expression<Func<Needs.Ccs.Services.Views.ApprovedListViewModel, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }

            string currentAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            int totalCount = 0;
            var view = new Needs.Ccs.Services.Views.ApprovedListView();
            var applicantList = view.GetResults(out totalCount, page, rows, currentAdminID, lambdas.ToArray());

            Response.Write(new
            {
                rows = applicantList.Select(
                        item => new
                        {
                            CostApplyID = item.CostApplyID,
                            PayeeName = item.PayeeName,
                            //CostType = item.CostType.ToString(),
                            //FeeType = item.FeeType.ToString(),
                            // FeeTypeName = item.FeeType == Needs.Ccs.Services.Enums.FeeTypeEnum.其它 ? item.FeeDesc : item.FeeType.ToString(),
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


    }
}