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

namespace Yahv.PvData.WebApp.SysConfig.ExchangeRateManage
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> exchangeCurrency = new Dictionary<string, string>() { { "-100", "全部" } };
            this.Model.ExchangeCurrency = exchangeCurrency.Concat(ExtendsEnum.ToDictionary<Currency>()).Select(item => new
            {
                value = item.Key,
                text = item.Value,
            });
        }

        protected object data()
        {
            var view = Yahv.Erp.Current.PvData.ExchangeRates.Where(query());

            return this.Paging(view, item => new
            {
                Type = typeConverter(item.Type),
                TypeValue = item.Type,
                District = item.District.GetDescription(),
                DistrictValue = item.District,
                From = item.From.GetDescription(),
                To = item.To.GetDescription(),
                FromShortName = item.From.GetCurrency().ShortName,
                ToShortName = item.To.GetCurrency().ShortName,
                item.Value,
            });
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.ExchangeRate, bool>> query()
        {
            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.ExchangeRate>(); // 条件拼接
            predicate = predicate.And(item => item.Type != "Preset");

            Currency fromCurrency;
            if (Enum.TryParse(Request["from"], out fromCurrency) && fromCurrency != 0 && (int)fromCurrency != -100)
            {
                predicate = predicate.And(item => item.From == fromCurrency);
            }

            Currency toCurrency;
            if (Enum.TryParse(Request["to"], out toCurrency) && toCurrency != 0 && (int)toCurrency != -100)
            {
                predicate = predicate.And(item => item.To == toCurrency);
            }

            return predicate;
            #endregion
        }

        protected JMessage delete()
        {
            string type = Request["type"];
            District district;
            Currency from, to;

            Enum.TryParse(Request["district"], out district);
            Enum.TryParse(Request["from"], out from);
            Enum.TryParse(Request["to"], out to);

            var exchangeRate = Yahv.Erp.Current.PvData.ExchangeRates[type, district, from, to];
            exchangeRate?.Delete();

            return new JMessage
            {
                code = 200,
                success = true,
                data = "删除操作成功",
            };
        }

        private string typeConverter(string type)
        {
            string result = string.Empty;
            switch (type)
            {
                case "Customs":
                    result = "海关汇率";
                    break;
                case "Fixed":
                    result = "固定汇率";
                    break;
                case "Floating":
                    result = "浮动汇率";
                    break;
            }

            return result;
        }
    }
}