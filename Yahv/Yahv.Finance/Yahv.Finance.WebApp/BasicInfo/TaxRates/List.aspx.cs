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

namespace Yahv.Finance.WebApp.BasicInfo.TaxRates
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 功能函数
        protected object data()
        {
            var query = Erp.Current.Finance.TaxRates.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                item.Name,
                item.Code,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.JsonName,
                item.CreatorName,
                item.Rate,
            });
        }
        #endregion

        #region 私有函数

        private Expression<Func<TaxRate, bool>> GetExpression()
        {
            Expression<Func<TaxRate, bool>> predicate = item => true;

            string name = Request.QueryString["s_name"];

            //名称
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            return predicate;
        }

        #endregion
    }
}