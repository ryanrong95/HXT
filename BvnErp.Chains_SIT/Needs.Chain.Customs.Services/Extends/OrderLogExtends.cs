using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class OrderLogExtends
    {
        /// <summary>
        /// 管理端写入订单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this Interfaces.IOrder order, Admin admin, string summary)
        {
            OrderLog log = new OrderLog();
            log.OrderID = order.ID;
            log.Admin = admin;
            log.OrderStatus = order.OrderStatus;
            log.Summary = summary;
            log.Enter();
        }

        /// <summary>
        /// 会员端写入订单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="user"></param>
        /// <param name="summary"></param>
        public static void Log(this Interfaces.IOrder order, User user, string summary)
        {
            OrderLog log = new OrderLog();
            log.OrderID = order.ID;
            log.User = user;
            log.OrderStatus = order.OrderStatus;
            log.Summary = summary;
            log.Enter();
        }
    }
}
