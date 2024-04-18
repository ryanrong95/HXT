using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Bills.Payments
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<PayerLeftShow, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var query = new PayerLefts_Show_View().GetPageList(page, rows, expression);
            return new
            {
                rows = query.Select(t => new
                {
                    Payer = "深圳市芯达通供应链管理有限公司",
                    t.ID,
                    t.CutDateIndex,
                    Total = t.Total.ToString("f2"),
                }),
                total = query.Total,
            }.Json();
        }

        Expression<Func<PayerLeftShow, bool>> Predicate()
        {
            Expression<Func<PayerLeftShow, bool>> predicate = item => true;
            //查询参数
            var DateIndex = Request.QueryString["DateIndex"];

            if (!string.IsNullOrWhiteSpace(DateIndex))
            {
                var index = int.Parse(DateIndex);
                predicate = predicate.And(item => item.CutDateIndex == index);
            }
            return predicate;
        }
    }
}