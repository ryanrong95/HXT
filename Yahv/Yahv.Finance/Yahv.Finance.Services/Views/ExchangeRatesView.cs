using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views
{
    public class ExchangeRatesView : Yahv.Linq.QueryView<Models.ExchangeRate, PvDataReponsitory>
    {
        public ExchangeRatesView()
        {
            //PvDataReponsitory
        }

        protected override IQueryable<Models.ExchangeRate> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ExchangeRates>()
                   select new Models.ExchangeRate
                   {
                       Type = entity.Type,
                       District = (ExchangeDistrict)entity.District,
                       From = (Underly.Currency)entity.From,
                       To = (Underly.Currency)entity.To,
                       Value = entity.Value,
                       StartDate = entity.StartDate,
                       ModifyDate = entity.ModifyDate
                   };
        }

        static public void Enter<TReponsitory>(Rater rater)
                  where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
        {
            using (TReponsitory reponsitory = new TReponsitory())
            {
                string typeName = rater.Type.ToString();

                Expression<Func<Layers.Data.Sqls.Overalls.ExchangeRates, bool>> lambda = item => item.District == (int)rater.District
                    && item.From == (int)rater.From
                    && item.To == (int)rater.To
                    && item.Type == typeName;

                if (reponsitory.ReadTable<Layers.Data.Sqls.Overalls.ExchangeRates>().Any(lambda))
                {
                    reponsitory.Update(new Layers.Data.Sqls.Overalls.ExchangeRates
                    {
                        Value = rater.Value

                    }, lambda);
                }
                else
                {
                    reponsitory.Insert(new Layers.Data.Sqls.Overalls.ExchangeRates
                    {
                        District = (int)rater.District,
                        From = (int)rater.From,
                        To = (int)rater.To,
                        Type = typeName,
                        Value = rater.Value,
                    });

                }
            }
        }

        static public void Delete<TReponsitory>(ExchangeType type, ExchangeDistrict district, Currency from, Currency to)
             where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
        {
            using (TReponsitory reponsitory = new TReponsitory())
            {
                string typeName = type.ToString();
                Expression<Func<Layers.Data.Sqls.PvData.ExchangeRates, bool>> lambda = item => item.District == (int)district
                    && item.From == (int)from
                    && item.To == (int)to
                    && item.Type == typeName;

                reponsitory.Delete(lambda);
            }
        }

        public Models.ExchangeRate this[ExchangeType type, ExchangeDistrict district, Currency from, Currency to]
        {
            get
            {
                return this.FirstOrDefault(item => item.Type == type.ToString() && item.District == district && item.From == from && item.To == to);
            }
        }
    }
}
