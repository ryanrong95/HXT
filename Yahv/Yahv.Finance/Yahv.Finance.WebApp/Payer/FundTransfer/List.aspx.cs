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
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.FundTransfer
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
            var query = Erp.Current.Finance.SelfAppliesView.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderBy(item => item.Status).ThenByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                SenderName = item?.Sender?.Name,
                PayerAccountName = item?.PayerAccount?.ShortName ?? item?.PayerAccount?.Name,
                PayeeAccountName = item?.PayeeAccount?.ShortName ?? item?.PayeeAccount?.Name,
                TargetCurrency = item.TargetCurrency.GetDescription(),
                item.TargetPrice,
                item.TargetERate,
                ApplierName = item?.Applier?.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
                StatusName = GetStatusName(item, item.Status),
            });
        }
        #endregion

        #region 私有函数

        private Expression<Func<SelfApply, bool>> GetExpression()
        {
            Expression<Func<SelfApply, bool>> predicate = item => true;

            //只能看到 自己相关的
            if (!Erp.Current.IsSuper)
            {
                predicate = predicate.And(item =>
                    item.CreatorID == Erp.Current.ID || item.ApplierID == Erp.Current.ID ||
                    item.ApproverID == Erp.Current.ID || item.ExcuterID == Erp.Current.ID);
            }

            string payer = Request.QueryString["s_payer"];
            string status = Request.QueryString["s_status"];
            string payee = Request.QueryString["s_payee"];

            //调出账户
            if (!string.IsNullOrWhiteSpace(payer))
            {
                predicate = predicate.And(item => item.PayerAccount.Name.Contains(payer) || item.PayerAccount.Code.Contains(payer) || item.PayerAccount.ShortName.Contains(payer));
            }
            //调入账户
            if (!string.IsNullOrWhiteSpace(payee))
            {
                predicate = predicate.And(item => item.PayeeAccount.Name.Contains(payee) || item.PayeeAccount.Code.Contains(payee) || item.PayeeAccount.ShortName.Contains(payee));
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
        private string GetStatusName(SelfApply apply, ApplyStauts status)
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