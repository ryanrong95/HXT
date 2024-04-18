using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class PayExchangelogExtends
    {
        /// <summary>
        /// 写入订单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.PayExchangeApply apply, string summary)
        {
            PayExchangeLog log = new PayExchangeLog();
            log.PayExchangeApplyID = apply.ID;
            log.Admin = apply.Admin;
            log.User = apply.User;
            log.PayExchangeApplyStatus = apply.PayExchangeApplyStatus;
            log.CreateDate = apply.CreateDate;
            log.Summary = summary;
            log.Enter();
        }

        /// <summary>
        /// 管理端付汇申请日志
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Operator">操作人</param>
        /// <param name="Summary">操作日志</param>
        public static void Log(this Models.PayExchangeApply entity, Admin Operator, string Summary)
        {
            PayExchangeLog log = new PayExchangeLog();
            log.PayExchangeApplyID = entity.ID;
            log.Admin = Operator;
            log.PayExchangeApplyStatus = entity.PayExchangeApplyStatus;
            log.Summary = Summary;
            log.Enter();
        }
    }
}
