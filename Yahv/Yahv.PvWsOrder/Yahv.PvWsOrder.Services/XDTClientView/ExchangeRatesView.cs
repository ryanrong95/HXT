using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 全部汇率的视图
    /// </summary>
    public abstract class ExchangeRatesView<T> : UniqueView<T, ScCustomReponsitory> where T : ExchangeRate, new()
    {
        /// <summary>
        /// 汇率类型
        /// </summary>
        private ExchangeRateType Type;

        public ExchangeRatesView(ExchangeRateType type)
        {
            this.Type = type;
        }

        protected override IQueryable<T> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ExchangeRates>()
                   join currency in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.BaseCurrencies>() on entity.Code equals currency.ID
                   where entity.Type == (int)this.Type
                   select new T
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = currency.Name,
                       Type = (ExchangeRateType)entity.Type,
                       Rate = entity.Rate,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate.Value
                   };
        }

        public virtual T FindByCode(string code)
        {
            return this.GetIQueryable().Where(t => t.Code == code).FirstOrDefault();
        }
    }
}
