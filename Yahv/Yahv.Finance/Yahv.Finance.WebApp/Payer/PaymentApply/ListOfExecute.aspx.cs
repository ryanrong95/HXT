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

namespace Yahv.Finance.WebApp.Payer.PaymentApply
{
    public partial class ListOfExecute : ErpParticlePage
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
                PayerAccountName = item?.PayerAccount?.Name,
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
            var catalogID = Erp.Current.Finance.AccountCatalogs.Get("付款类型", "供应链业务")
                .FirstOrDefault(item => item.Name == "代付货款")?.ID;

            Expression<Func<PayerApply, bool>> predicate = item => item.Status == ApplyStauts.Paying && item.PayerLeft.AccountCatalogID == catalogID;


#if !DEBUG
            //显示当前执行人或者执行人为空的列表
            predicate = predicate.And(item => item.ExcuterID == Erp.Current.ID || item.ExcuterID == null || item.ExcuterID == "");
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