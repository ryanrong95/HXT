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
            Expression<Func<Services.Models.Origins.AcceptanceApply, bool>> predicate = item => true;

            //只能看到 自己相关的
            if (!Erp.Current.IsSuper)
            {
                predicate = predicate.And(item =>
                    item.CreatorID == Erp.Current.ID || item.ApplierID == Erp.Current.ID ||
                    item.ApproverID == Erp.Current.ID || item.ExcuterID == Erp.Current.ID);
            }

            string status = Request.QueryString["s_status"];

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