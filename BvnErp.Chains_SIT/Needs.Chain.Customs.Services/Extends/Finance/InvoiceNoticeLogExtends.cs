using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票通知扩展方法
    /// </summary>
    public static class InvoiceNoticeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.InvoiceNoticeLogs ToLinq(this Models.InvoiceNoticeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.InvoiceNoticeLogs
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                InvoiceNoticeID = entity.InvoiceNoticeID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }


        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Operator"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.InvoiceContext entity, Admin Operator, string summary)
        {
            InvoiceNoticeLog log = new InvoiceNoticeLog();
            log.InvoiceNoticeID = entity.InvoiceNoticeID;
            log.Admin = Operator;
            log.Status =entity.InvoiceNoticeStatus;
            log.Summary = summary;
            log.Enter();
        }
    }
}
