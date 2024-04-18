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

namespace Yahv.PvData.WebApp.SysConfig.PartNumberControl
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.CustomsControls.Where(query());

            return this.Paging(view, item => new
            {
                item.ID,
                item.PartNumber,
                item.Name,
                CreateDate = item.CreateDate.ToShortDateString(),
                UpdateDate = item.ModifyDate.ToShortDateString()
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.CustomsControl, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.CustomsControl>(); // 条件拼接
            predicate = predicate.And(item => item.Type == YaHv.PvData.Services.CustomsControlType.Partnumber);

            string partNumber = Request["partNumber"];
            if (!string.IsNullOrEmpty(partNumber))
            {
                predicate = predicate.And(item => item.PartNumber.Contains(partNumber.Trim()));
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

            var hsCodeControl = Yahv.Erp.Current.PvData.CustomsControls[id];
            hsCodeControl?.Abandon();

            return new JMessage
            {
                code = 200,
                success = true,
                data = "删除操作成功",
            };
        }
    }
}