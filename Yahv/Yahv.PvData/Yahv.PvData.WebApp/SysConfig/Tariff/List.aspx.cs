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

namespace Yahv.PvData.WebApp.SysConfig.Tariff
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.Tariffs.Where(query());

            return this.Paging(view, item => new
            {
                item.ID,
                item.HSCode,
                item.Name,
                item.LegalUnit1,
                item.LegalUnit2,
                item.ImportPreferentialTaxRate,
                item.ImportControlTaxRate,
                item.ImportGeneralTaxRate,
                item.VATRate,
                item.ExciseTaxRate,
                item.DeclareElements,
                item.SupervisionRequirements,
                item.CIQC,
                item.CIQCode,
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.Tariff, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.Tariff>(); // 条件拼接

            string hsCode = Request["hsCode"];
            if (!string.IsNullOrEmpty(hsCode))
            {
                predicate = predicate.And(item => item.HSCode.StartsWith(hsCode.Trim()));
            }

            string name = Request["name"];
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name.Trim()));
            }

            return predicate;
            #endregion
        }

        protected JMessage delete()
        {
            string id = Request["id"];

            var tariff = Yahv.Erp.Current.PvData.Tariffs[id];
            tariff?.Delete();

            return new JMessage
            {
                code = 200,
                success = true,
                data = "删除操作成功",
            };
        }
    }
}