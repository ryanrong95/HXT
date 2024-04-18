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
    /// 代理订单轨迹的视图
    /// </summary>
    public class OrderTracesView : UniqueView<Models.OrderTrace, ScCustomsReponsitory>
    {
        protected override IQueryable<OrderTrace> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);

            return from orderTrace in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                   join admin in adminsView on orderTrace.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join user in usersView on orderTrace.UserID equals  user.ID into users
                   from user in users.DefaultIfEmpty()
                   select new Models.OrderTrace
                   {
                       ID = orderTrace.ID,
                       OrderID = orderTrace.OrderID,
                       Admin = admin,
                       User = user,
                       Step = (Enums.OrderTraceStep)orderTrace.Step,
                       CreateDate = orderTrace.CreateDate,
                       Summary = orderTrace.Summary
                   };
        }
    }
}
