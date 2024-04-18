using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Utils.Extends;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.InfoSearch.Embargo
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.EmbargoInfos.Where(query());

            return this.Paging(view, item => new
            {
                item.ID,
                item.PartNumber,
                item.Manufacturer,

                Ccc = item.Ccc.Text(),
                Embargo = item.Embargo.Text(),
                HkControl = item.HkControl.Text(),
                Coo = item.Coo.Text(),
                CIQ = item.CIQ.Text(),
                item.CIQprice,
                item.Summary,

                CreateDate = item.CreateDate?.ToString("yyyy-MM-dd HH:mm"),
                UpdateDate = item.OrderDate?.ToString("yyyy-MM-dd HH:mm"),

                Eccns = item.Eccns
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.Other, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.Other>(); // 条件拼接

            string partNumber = Request["partNumber"];
            if (!string.IsNullOrEmpty(partNumber))
            {
                predicate = predicate.And(item => item.PartNumber.StartsWith(partNumber.Trim()));
            }

            return predicate;
            #endregion
        }
    }
}