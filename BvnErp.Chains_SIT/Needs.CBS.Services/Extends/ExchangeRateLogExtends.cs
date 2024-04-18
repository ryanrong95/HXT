using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Models.Origins
{
    public static partial class ExchangeRateLogExtends
    {
        public static void Log(this ExchangeRate entity,string summary)
        {
            var log = new Origins.ExchangeRateLog();
            log.ExchangeRateID = entity.ID;
            log.Rate = entity.Rate;
            log.Summary = summary;
            log.Enter();
        }
    }
}
