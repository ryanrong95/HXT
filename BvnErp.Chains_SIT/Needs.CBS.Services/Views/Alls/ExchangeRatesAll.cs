using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Alls
{
    /// <summary>
    /// 海关汇率视图
    /// </summary>
    public class ExchangeRatesAll : UniqueView<Models.Origins.ExchangeRate, ScCustomsReponsitory>
    {
        public ExchangeRatesAll()
        {
        }

        protected override IQueryable<Models.Origins.ExchangeRate> GetIQueryable()
        {
            var currenciesView = new Origins.CustomsSettingsOrigin(this.Reponsitory)[Enums.BaseType.Currency];
            var excahngeRatesView = new Origins.ExchangeRatesOrigin(this.Reponsitory);
            return from entity in excahngeRatesView
                   join currency in currenciesView on entity.Code equals currency.ID
                   select new Models.Origins.ExchangeRate
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = currency.Name,
                       Type = entity.Type,
                       Rate = entity.Rate,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="type">汇率类型</param>
        /// <returns></returns>
        public IQueryable<Models.Origins.ExchangeRate> this[Enums.ExchangeRateType type]
        {
            get
            {
                return this.Where(item => item.Type == type);
            }
        }

        /// <summary>
        /// 所以请你
        /// </summary>
        /// <param name="type">汇率类型</param>
        /// <param name="code">币种代码</param>
        /// <returns></returns>
        public Models.Origins.ExchangeRate this[Enums.ExchangeRateType type, string code]
        {
            get
            {
                return this.FirstOrDefault(item => item.Type == type && item.Code == code);
            }
        }
    }
}
