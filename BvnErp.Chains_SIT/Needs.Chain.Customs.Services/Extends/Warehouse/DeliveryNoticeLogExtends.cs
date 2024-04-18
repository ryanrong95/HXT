using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class DeliveryNoticeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DeliveryNoticeLogs ToLinq(this Models.DeliveryNoticeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DeliveryNoticeLogs
            {
                ID = entity.ID,
                DeliveryNoticeID = entity.DeliveryNoticeID,
                AdminID = entity.Admin.ID,
                OperType = (int)entity.OperType,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }

        /// <summary>
        /// 写入订单日志,提货完成
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.DeliveryNotice deliveryNotice, string summary)
        {
            DeliveryNoticeLog log = new DeliveryNoticeLog();
            log.DeliveryNoticeID = deliveryNotice.ID;
            log.Admin = deliveryNotice.Admin;
            log.OperType = Enums.DeliveryOperType.HadDelivered;
            log.Summary = summary;
            log.Enter();
        }
    }
}
