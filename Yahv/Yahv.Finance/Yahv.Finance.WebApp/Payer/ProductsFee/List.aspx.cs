using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ProductsFee
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Statuses = ExtendsEnum.ToDictionary<ApplyStauts>().Select(item => new { text = item.Value, value = item.Key });
            }
        }

        #region 功能函数
        protected object data()
        {
            var query = Erp.Current.Finance.PayerAppliesView.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderBy(item => item.Status).ThenByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                SenderName = item?.Sender?.Name,
                PayerAccountName = item?.PayerAccount?.Name ?? item.PayerName,
                PayeeAccountName = item?.PayeeAccount?.Name,
                Currency = item.Currency.GetDescription(),
                item.Price,
                ApplierName = item?.Applier?.RealName,
                //PaymentDate = item?.PaymentDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
                StatusName = GetStatusName(item, item.Status),
            });
        }
        #endregion

        #region 私有函数

        private Expression<Func<PayerApply, bool>> GetExpression()
        {
            Expression<Func<PayerApply, bool>> predicate = item => true;

            //只能看到 自己相关的
            if (!Erp.Current.IsSuper)
            {
                predicate = predicate.And(item =>
                    item.CreatorID == Erp.Current.ID || item.ApplierID == Erp.Current.ID ||
                    item.ApproverID == Erp.Current.ID || item.ExcuterID == Erp.Current.ID);
            }

            string payer = Request.QueryString["s_name"];
            string status = Request.QueryString["s_status"];
            string code = Request.QueryString["s_code"];

            //付款账户
            if (!string.IsNullOrWhiteSpace(payer))
            {
                predicate = predicate.And(item => item.PayerAccount.Name.Contains(payer) || item.PayerAccount.Code.Contains(payer));
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
        private string GetStatusName(PayerApply apply, ApplyStauts status)
        {
            string result = String.Empty;
            switch (status)
            {
                case ApplyStauts.Completed:
                    result = status.GetDescription();
                    break;
                case ApplyStauts.Rejecting:
                    result = apply.Applier?.RealName != null ? $"{status.GetDescription()}({apply.Applier?.RealName})" : status.GetDescription();
                    break;
                case ApplyStauts.Paying:
                    result = apply.Excuter?.RealName != null ? $"{status.GetDescription()}({apply.Excuter?.RealName})" : status.GetDescription();
                    break;
                default:
                    result = apply.Approver?.RealName != null ? $"{status.GetDescription()}({apply.Approver?.RealName})" : status.GetDescription();
                    break;
            }
            return result;
        }
        #endregion
    }
}