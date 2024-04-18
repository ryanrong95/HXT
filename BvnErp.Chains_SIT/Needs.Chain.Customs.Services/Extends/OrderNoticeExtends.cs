using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services
{
    public static class OrderNoticeExtends
    {
        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void SendNotice(this Interfaces.IOrder order, Enums.SendNoticeType noticeType)
        {
            NoticeLog log = new NoticeLog();
            log.NoticeType = noticeType;
            log.MainID = order.ID;
            log.AdminIDs.Add(order.Client.Merchandiser.ID);
            log.SendNotice();
        }
    }
}
