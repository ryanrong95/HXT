using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static partial class ExchangeRateLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExchangeRateLogs ToLinq(this Models.ExchangeRateLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExchangeRateLogs
            {
                ID = entity.ID,
                ExchangeRateID = entity.ExchangeRateID,
                Rate = entity.Rate,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
