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
    public partial class ListOfApproval : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 功能函数
        protected object data()
        {
            var query = Erp.Current.Finance.SelfAppliesView.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                SenderName = item?.Sender?.Name,
                PayerAccountName = item?.PayerAccount?.ShortName ?? item?.PayerAccount?.Name,
                PayeeAccountName = item?.PayeeAccount?.ShortName ?? item?.PayeeAccount?.Name,
                TargetERate = item.TargetERate,
                TargetCurrency = item.TargetCurrency.GetDescription(),
                item.TargetPrice,
                ApplierName = item?.Applier?.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusName = item.Status == ApplyStauts.Completed ? item.Status.GetDescription() : $"{item.Status.GetDescription()}({item?.Approver?.RealName})"
            });
        }
        #endregion

        #region 私有函数

        private Expression<Func<SelfApply, bool>> GetExpression()
        {
            Expression<Func<SelfApply, bool>> predicate = item => item.Status == ApplyStauts.Waiting;


#if !DEBUG
            //显示当前审批人或者审批人为空的列表
            predicate = predicate.And(item => item.ApproverID == Erp.Current.ID || item.ApplierID=="");
#endif

            string payer = Request.QueryString["s_payer"];
            string payee = Request.QueryString["s_payee"];
            string code = Request.QueryString["s_code"];

            //调出账户
            if (!string.IsNullOrWhiteSpace(payer))
            {
                predicate = predicate.And(item => item.PayerAccount.Name.Contains(payer) || item.PayerAccount.Code.Contains(payer) || item.PayerAccount.ShortName.Contains(payer));
            }
            //调入编码
            if (!string.IsNullOrWhiteSpace(payee))
            {
                predicate = predicate.And(item => item.PayeeAccount.Name.Contains(payee) || item.PayeeAccount.Code.Contains(payee) || item.PayeeAccount.ShortName.Contains(payee));
            }
            //申请编码
            if (!string.IsNullOrEmpty(code))
            {
                predicate = predicate.And(item => item.ID.Contains(code));
            }

            return predicate;
        }
        #endregion
    }
}