using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 日志扩展方法
    /// </summary>
    public static class SwapLimitCountryLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.SwapLimitCountryLogs ToLinq(this Models.SwapLimitCountryLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.SwapLimitCountryLogs
            {
                ID = entity.ID,
                BankID = entity.BankID,
                AdminID = entity.Admin?.ID,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
        public static void Log(this SwapLimitCountry limit,Admin admin, string summary)
        {
            SwapLimitCountryLog log = new SwapLimitCountryLog();
            log.BankID = limit.BankID;
            log.Admin = admin;
            log.Summary = summary;
            log.Enter();
        }
    }
}
