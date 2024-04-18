using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.SysConfig.ElementsManufacturer
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.ElementsManufacturers.Where(query());

            return this.Paging(view, item => new
            {
                item.ID,
                item.Manufacturer,
                item.MfrMapping
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.ElementsManufacturer, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.ElementsManufacturer>(); // 条件拼接

            string mfr = Request["mfr"];
            if (!string.IsNullOrEmpty(mfr))
            {
                predicate = predicate.And(item => item.Manufacturer.StartsWith(mfr.Trim()));
            }

            return predicate;
            #endregion
        }
    }
}