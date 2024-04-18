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

namespace Yahv.PvData.WebApp.SysConfig.OriginATRate
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.Origin = ExtendsEnum.ToDictionary<Origin>().Select(item => new
            {
                value = item.Key,
                text = Enum.Parse(typeof(Origin), item.Key) + "-" + item.Value,
            });
        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.OriginsATRate.Where(query());

            return this.Paging(view, item => new
            {
                item.ID,
                item.Tariff.HSCode,
                item.Tariff.Name,
                item.Origin,
                Type = "进口关税",
                item.Rate,
                StartDate = item.StartDate.ToShortDateString(),
                EndDate = item.EndDate?.ToShortDateString()
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.OriginATRate, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.OriginATRate>(); // 条件拼接

            string hsCode = Request["hsCode"];
            if (!string.IsNullOrEmpty(hsCode))
            {
                predicate = predicate.And(item => item.TariffID.StartsWith(hsCode.Trim()));
            }

            string origin = Request["origin"];
            if (!string.IsNullOrEmpty(origin))
            {
                origin = ((Origin)Enum.Parse(typeof(Origin), origin)).GetOrigin().Code;
                predicate = predicate.And(item => item.Origin == origin);
            }

            return predicate;
            #endregion
        }

        protected JMessage delete()
        {
            string id = Request["id"];

            var oririnATRate = Yahv.Erp.Current.PvData.OriginsATRate[id];
            oririnATRate?.Abandon();

            return new JMessage
            {
                code = 200,
                success = true,
                data = "删除操作成功",
            };
        }
    }
}