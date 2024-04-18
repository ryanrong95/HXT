using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 付汇申请日志记录
    /// </summary>
    public class PayExchangeLogsView : UniqueView<Models.PayExchangeLog, ScCustomsReponsitory>
    {
        public PayExchangeLogsView()
        {

        }

        internal PayExchangeLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);

            var result = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs>()
                         join admin in adminsView on entity.AdminID equals admin.ID into admins
                         from _admin in admins.DefaultIfEmpty()
                         join user in usersView on entity.UserID equals user.ID into users
                         from _user in users.DefaultIfEmpty()
                         select new Models.PayExchangeLog
                         {
                             ID = entity.ID,
                             PayExchangeApplyID = entity.PayExchangeApplyID,
                             PayExchangeApplyStatus = (PayExchangeApplyStatus)entity.PayExchangeApplyStatus,
                             Admin = _admin,
                             User = _user,
                             Summary = entity.Summary,
                             CreateDate = entity.CreateDate
                         };
            return result;
        }
    }
}