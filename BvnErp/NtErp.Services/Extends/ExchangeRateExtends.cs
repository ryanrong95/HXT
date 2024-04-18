using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Extends
{
    static public class ExchangeRateExtends
    {
        public static Layer.Data.Sqls.BvOveralls.ExchangeRates ToLinq(this Models.ExchangeRate entity)
        {
            return new Layer.Data.Sqls.BvOveralls.ExchangeRates()
            {
                Type = entity.Type,
                District = (int)entity.District,
                From = (int)entity.From,
                To = (int)entity.To,
                Value = entity.Value
            };
        }
    }
}
