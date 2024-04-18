using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 全部汇率的视图
    /// </summary>
    public abstract class ExchangeRatesView<T> : View<T, ScCustomsReponsitory> where T : Models.ExchangeRate, new()
    {
        /// <summary>
        /// 汇率类型
        /// </summary>
        private Enums.ExchangeRateType Type;

        public ExchangeRatesView(Enums.ExchangeRateType type)
        {
            this.Type = type;
        }

        internal ExchangeRatesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<T> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRates>()
                   join currency in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCurrencies>() on entity.Code equals currency.ID
                   where entity.Type == (int)this.Type
                   select new T
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = currency.Name,
                       Type = (Enums.ExchangeRateType)entity.Type,
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