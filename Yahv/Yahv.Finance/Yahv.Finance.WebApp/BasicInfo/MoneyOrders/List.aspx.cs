using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.MoneyOrders
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Statuses = ExtendsEnum.ToDictionary<MoneyOrderStatus>()
                    .Select(item => new { value = item.Key, text = item.Value });
            }
        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);


            using (var view = new MoneyOrdersRoll())
            {
                var query = view.Search(GetPredicate());
                return query.ToMyPage(page, rows).Json();
            }
        }


        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<MoneyOrder, bool>> GetPredicate()
        {
            Expression<Func<MoneyOrder, bool>> predicate = item => true;

            if (!Erp.Current.IsSuper)
            {
                predicate = predicate.And(item => item.CreatorID == Erp.Current.ID);
            }

            string code = Request.QueryString["s_code"];
            string status = Request.QueryString["s_status"];
            string begin = Request.QueryString["s_begin"];
            string end = Request.QueryString["s_end"];

            if (!string.IsNullOrEmpty(code))
            {
                predicate = predicate.And(item => item.Code.Contains(code) || item.ID.Contains(code));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var status_temp = (MoneyOrderStatus)int.Parse(status);
                predicate = predicate.And(item => item.Status == status_temp);
            }

            if (!string.IsNullOrEmpty(begin))
            {
                predicate = predicate.And(item => item.EndDate >= DateTime.Parse(begin));
            }

            if (!string.IsNullOrEmpty(end))
            {
                DateTime end_temp = DateTime.Parse(end).AddDays(1);
                predicate = predicate.And(item => item.EndDate < end_temp);
            }

            return predicate;
        }
    }
}