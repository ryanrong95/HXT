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
    public partial class ApplicantList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.CostStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CostStatusEnum>().Select(item => new { item.Key, item.Value }).Json();
        }

        protected void AppliantListData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string PayeeName = Request.QueryString["PayeeName"];           
            string CostStatus = Request.QueryString["CostStatus"];
            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            lambdas.Add((Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>>)(t => t.AdminID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID));

            if (!string.IsNullOrEmpty(PayeeName))
            {
                PayeeName = PayeeName.Trim();
                Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.PayeeName.Contains(PayeeName);
                lambdas.Add(lambda);
            }
            
            if (!string.IsNullOrEmpty(CostStatus))
            {
                int CostStatusInt = int.Parse(CostStatus);
                Expression<Func<Needs.Ccs.Services.Views.ApplicantListViewModel, bool>> lambda = item => item.CostStatus == (Needs.Ccs.Services.Enums.CostStatusEnum)CostStatusInt;
                lambdas.Add(lambda);
            }
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
                            //FeeTypeName = item.FeeType.GetDescription(),
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
        /// 根据 CostApplyID 获取 CostStatus
        /// </summary>
        protected void GetCostStatusByCostApplyID()
        {
            string CostApplyID = Request.Form["CostApplyID"];

            var costApplyDetail = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(CostApplyID);

            Response.Write(new { success = true, coststatus = costApplyDetail.CostStatus.GetHashCode(), }.Json());
        }

    }
}