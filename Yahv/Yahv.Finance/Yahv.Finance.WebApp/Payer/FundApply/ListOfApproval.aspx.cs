using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.FundApply
{
    public partial class ListOfApproval : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 功能函数
        protected object data()
        {
            var query = Erp.Current.Finance.CostApplies.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderBy(item => item.Status).ThenByDescending(item => item.CreateDate), item => new
            {
                ID = item.ID,
                SenderName = item.SenderName,
                PayerAccountName = item?.PayerAccountName ?? item.PayerName,
                PayeeAccountName = item?.PayeeAccountName,
                CurrencyDes = item.Currency.GetDescription(),
                Price = item.Price.ToRound1(2),
                ApplierName = item.ApplierName,
                StatusDes = GetStatusName(item),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
            });
        }
        #endregion

        #region 私有函数
        private Expression<Func<Services.Models.Origins.CostApply, bool>> GetExpression()
        {
            Expression<Func<Services.Models.Origins.CostApply, bool>> predicate = item => item.Status == ApplyStauts.Waiting;


#if !DEBUG
            //显示当前审批人或者审批人为空的列表
            predicate = predicate.And(item => item.ApproverID == Erp.Current.ID || item.ApplierID == "");
#endif
            string payer = Request.QueryString["s_payer"];
            string status = Request.QueryString["s_status"];
            string code = Request.QueryString["s_code"];

            //付款账户
            if (!string.IsNullOrWhiteSpace(payer))
            {
                predicate = predicate.And(item => item.PayerAccountName.Contains(payer) || item.PayerAccountCode.Contains(payer));
            }
            //申请编码
            if (!string.IsNullOrWhiteSpace(code))
            {
                predicate = predicate.And(item => item.ID.Contains(code));
            }
            //状态
            if (!string.IsNullOrWhiteSpace(status))
            {
                predicate = predicate.And(item => item.Status == (ApplyStauts)int.Parse(status));
            }

            return predicate;
        }

        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        string GetStatusName(Services.Models.Origins.CostApply apply)
        {
            string result = String.Empty;
            switch (apply.Status)
            {
                case ApplyStauts.Completed:
                    result = apply.Status.GetDescription();
                    break;
                case ApplyStauts.Rejecting:
                    result = apply.ApplierName != null ? $"{apply.Status.GetDescription()}({apply.ApplierName})" : apply.Status.GetDescription();
                    break;
                case ApplyStauts.Paying:
                    result = apply.ExcuterName != null ? $"{apply.Status.GetDescription()}({apply.ExcuterName})" : apply.Status.GetDescription();
                    break;
                default:
                    result = apply.ApproverName != null ? $"{apply.Status.GetDescription()}({apply.ApproverName})" : apply.Status.GetDescription();
                    break;
            }
            return result;
        }
        #endregion
    }
}