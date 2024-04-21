using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.AcceptanceApply
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

            using (var view = new AcceptanceAppliesRoll())
            {
                var query = view.Where(GetExpression())
                    .SearchByPayerName(Request.QueryString["s_payer"])
                    .SearchByPayeeName(Request.QueryString["s_payee"]);
                return query.ToMyPage(page, rows).Json();
            }
        }
        #endregion

        #region 私有函数

        private Expression<Func<Services.Models.Origins.AcceptanceApply, bool>> GetExpression()
        {
            Expression<Func<Services.Models.Origins.AcceptanceApply, bool>> predicate = item => item.Status == ApplyStauts.Paying;


#if !DEBUG
            predicate = predicate.And(item => item.ExcuterID == Erp.Current.ID || item.ExcuterID == null || item.ExcuterID == "");
#endif

            string code = Request.QueryString["s_code"];

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