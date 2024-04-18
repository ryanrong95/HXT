using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static partial class ExchangeRateLogExtends
    {
        public static void Log(this Models.ExchangeRate entity, Admin admin, string summary)
        {
            ExchangeRateLog log = new ExchangeRateLog();
            log.ExchangeRateID = entity.ID;
            log.Rate = entity.Rate;
            log.Admin = admin;
            log.Summary = summary;
            log.Enter();
        }
    }
}
