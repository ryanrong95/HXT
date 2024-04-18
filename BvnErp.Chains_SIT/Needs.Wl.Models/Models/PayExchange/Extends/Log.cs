using System;

namespace Needs.Wl.Models
{
    public static partial class PayExchangeApplyExtends
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this PayExchangeApply apply, string summary)
        {
            PayExchangeLog log = new PayExchangeLog();
            log.PayExchangeApplyID = apply.ID;
            log.AdminID = apply.Admin == null ? "" : apply.Admin.ID;
            log.UserID = apply.User == null ? "" : apply.User.ID;
            log.PayExchangeApplyStatus = apply.PayExchangeApplyStatus;
            log.CreateDate = DateTime.Now;
            log.Summary = summary;
            log.Enter();
        }
    }
}