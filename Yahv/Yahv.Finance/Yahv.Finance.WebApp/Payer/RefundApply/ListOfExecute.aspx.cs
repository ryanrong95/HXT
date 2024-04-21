using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.RefundApply
{
    public partial class ListOfExecute : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 功能函数
        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            using (var view = new RefundAppliesRoll())
            {
                var query = view.Where(GetPredicate())
                    .SearchByPayerName(Request.QueryString["s_name"])
                    .ToMyPage(page, rows);

                return query.Json();
            }
        }
        #endregion

        #region 私有函数
        private Expression<Func<Services.Models.Origins.RefundApply, bool>> GetPredicate()
        {
            Expression<Func<Services.Models.Origins.RefundApply, bool>> predicate = item => item.Status == ApplyStauts.Paying;

#if !DEBUG
            //显示当前审批人或者审批人为空的列表
            predicate = predicate.And(item => item.ApproverID == Erp.Current.ID || item.ApplierID=="");
#endif
            string status = Request.QueryString["s_status"];
            string code = Request.QueryString["s_code"];

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
        #endregion
    }
}