using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Origins
{
    /// <summary>
    /// 海关汇率视图
    /// </summary>
    internal class ExchangeRatesOrigin : UniqueView<Models.Origins.ExchangeRate, ScCustomsReponsitory>
    {
        internal ExchangeRatesOrigin()
        {
        }

        internal ExchangeRatesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.ExchangeRate> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.ExchangeRates>()
                   select new Models.Origins.ExchangeRate
                   {
                       ID = cert.ID,
                       Code = cert.Code,
                       Rate = cert.Rate,
                       Type = (Enums.ExchangeRateType)cert.Type,
                       CreateDate = cert.CreateDate,
                       UpdateDate = cert.UpdateDate,
                       Summary = cert.Summary
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="type">汇率类型</param>
        /// <returns></returns>
        internal IQueryable<Models.Origins.ExchangeRate> this[Enums.ExchangeRateType type]
        {
            get
            {
                return this.Where(item => item.Type == type);
            }
        }
    }
}
