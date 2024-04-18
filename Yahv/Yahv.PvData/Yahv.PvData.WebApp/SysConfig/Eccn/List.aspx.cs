using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.SysConfig.Eccn
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.Eccns.Where(query())
                .OrderByDescending(item => item.ModifyDate);

            return this.Paging(view, item => new
            {
                item.ID,
                item.PartNumber,
                item.Manufacturer,
                item.Code,
                item.LastOrigin,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ModifyDate = item.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Yahv.Services.Models.Eccn, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<Yahv.Services.Models.Eccn>(); // 条件拼接

            string partnumber = Request["partnumber"];
            if (!string.IsNullOrEmpty(partnumber))
            {
                predicate = predicate.And(item => item.PartNumber.StartsWith(partnumber.Trim()));
            }

            return predicate;
            #endregion
        }
    }
}