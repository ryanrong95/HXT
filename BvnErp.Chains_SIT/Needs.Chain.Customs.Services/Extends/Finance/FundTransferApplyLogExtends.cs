using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class FundTransferApplyLogExtends
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="apply"></param>
       /// <param name="summary"></param>
        public static void Log(this Models.FundTransferApplies apply, string summary)
        {
            Logs log = new Logs();
            log.Name = "资金调拨审批";
            log.MainID = apply.ID;
            log.AdminID = apply.Admin.ID;
            log.Json = "";
            log.Summary = summary;
            log.Enter();
        }
    }
}
