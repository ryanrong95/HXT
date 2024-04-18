using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单日志的视图
    /// </summary>
    public class OrderLogsView : UniqueView<Models.OrderLog, ScCustomsReponsitory>
    {
        public OrderLogsView()
        {
        }

        internal OrderLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);

            return from orderLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                   join admin in adminsView on orderLog.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join user in usersView on orderLog.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   orderby orderLog.CreateDate
                   select new Models.OrderLog
                   {
                       ID = orderLog.ID,
                       OrderID = orderLog.OrderID,
                       OrderItemID = orderLog.OrderItemID,
                       Admin = admin,
                       User = user,
                       OrderStatus = (Enums.OrderStatus)orderLog.OrderStatus,
                       CreateDate = orderLog.CreateDate,
                       Summary = orderLog.Summary
                   };
        }
    }
}
