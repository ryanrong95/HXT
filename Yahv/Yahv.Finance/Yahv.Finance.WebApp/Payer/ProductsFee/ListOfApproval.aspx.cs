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

namespace Yahv.Finance.WebApp.Payer.ProductsFee
{
    public partial class ListOfApproval : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 功能函数
        protected object data()
        {
            var query = Erp.Current.Finance.PayerAppliesView.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
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
                Status = item.Status == ApplyStauts.Completed ? item.Status.GetDescription() : $"{item.Status.GetDescription()}({item?.Approver?.RealName})"
            });
        }
        #endregion

        #region 私有函数

        private Expression<Func<PayerApply, bool>> GetExpression()
        {
            Expression<Func<PayerApply, bool>> predicate = item => item.Status == ApplyStauts.Waiting;


#if !DEBUG
            //显示当前审批人或者审批人为空的列表
            predicate = predicate.And(item => item.ApproverID == Erp.Current.ID || item.ApplierID=="");
#endif

            string payer = Request.QueryString["s_name"];
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

            return predicate;
        }
        #endregion
    }
}