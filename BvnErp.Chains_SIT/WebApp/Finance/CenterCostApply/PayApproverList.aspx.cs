using Needs.Ccs.Services.Enums;
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
    public partial class PayApproverList : Uc.PageBase
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

            this.Model.CostStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CostStatusEnum>().Select(item => new { item.Key, item.Value }).Json();
        }

        protected void PayApproverListData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string PayeeName = Request.QueryString["PayeeName"];
            //string CostType = Request.QueryString["FeeType"];
            string CostStatus = Request.QueryString["CostStatus"];
            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];


            List<LambdaExpression> lambdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(PayeeName))
            {
                PayeeName = PayeeName.Trim();
                lambdas.Add((Expression<Func<Needs.Ccs.Services.Models.PayApproverList, bool>>)(t => t.PayeeName.Contains(PayeeName)));
            }
            //if (!string.IsNullOrEmpty(CostType))
            //{
            //    int costtype = int.Parse(CostType);
            //    lambdas.Add((Expression<Func<Needs.Ccs.Services.Models.PayApproverList, bool>>)(t => t.CostType == (CostTypeEnum)costtype));
            //}
            if (!string.IsNullOrEmpty(CostStatus))
            {
                int coststatus = int.Parse(CostStatus);
                lambdas.Add((Expression<Func<Needs.Ccs.Services.Models.PayApproverList, bool>>)(t => t.CostStatus == (CostStatusEnum)coststatus));
            }
            if (!string.IsNullOrEmpty(CreateDateBegin))
            {
                DateTime start = Convert.ToDateTime(CreateDateBegin);
                lambdas.Add((Expression<Func<Needs.Ccs.Services.Models.PayApproverList, bool>>)(t => t.CreateDate >= start));
            }
            if (!string.IsNullOrEmpty(CreateDateEnd))
            {
                DateTime end = Convert.ToDateTime(CreateDateEnd).AddDays(1);
                lambdas.Add((Expression<Func<Needs.Ccs.Services.Models.PayApproverList, bool>>)(t => t.CreateDate < end));
            }

            int totalCount = 0;
            var view = new Needs.Ccs.Services.Views.PayApproverListView();
            var payApprovertList = view.GetResult(out totalCount, page, rows, lambdas.ToArray());
            Response.Write(new
            {
                rows = payApprovertList.Select(
                           item => new
                           {
                               CostApplyID = item.CostApplyID,
                               PayeeName = item.PayeeName,
                               CostType = item.CostType.ToString(),
                               FeeTypeName = item.CostType == Needs.Ccs.Services.Enums.CostTypeEnum.费用 ? (item.FeeType == Needs.Ccs.Services.Enums.FeeTypeEnum.其它 ? item.FeeDesc : item.FeeType.ToString()) : "",//Needs.Ccs.Services.Enums.FeeTypeEnum.其它 ? item.FeeDesc : item.FeeType.ToString(),
                               Amount = item.Amount,
                               Currency = item.Currency,
                               CostStatusInt = item.CostStatus.GetHashCode(),
                               CostStatus = item.CostStatus.GetDescription(),
                               PayTime = item.PayTime?.ToString("yyyy-MM-dd"),
                               CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                               PaperNotesStatusInt = item.PaperNotesStatus.GetHashCode(),
                               PaperNotesStatus = item.PaperNotesStatus.GetDescription()
                           }
                        ).ToArray(),
                total = totalCount,
            }.Json());
        }
    }
}