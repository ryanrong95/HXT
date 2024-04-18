using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 汇率
    /// </summary>
    public class ExchangeRatesAll : QueryView<Models.ExchangeRate, PvDataReponsitory>
    {
        public ExchangeRatesAll()
        {
        }

        internal ExchangeRatesAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExchangeRate> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ExchangeRates>()
                        where entity.From >= 1 && entity.From <= 3 && entity.To >= 1 && entity.To <= 3
                        select new Models.ExchangeRate
                        {
                            District = (District)entity.District,
                            From = (Currency)entity.From,
                            To = (Currency)entity.To,
                            Type = entity.Type,
                            Value = entity.Value,
                            StartDate = entity.StartDate,
                            ModifyDate = entity.ModifyDate
                        };

            return linqs;
        }

        public Models.ExchangeRate this[string type, District district, Currency from, Currency to]
        {
            get
            {
                return this.FirstOrDefault(item => item.Type == type && item.District == district && item.From == from && item.To == to);
            }
        }
    }
}
