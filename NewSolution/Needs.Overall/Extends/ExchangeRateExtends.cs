using System;

namespace Needs.Overall.Extends
{
    [Obsolete("建议放弃，我会在一定时间通知彻底放弃的")]
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
