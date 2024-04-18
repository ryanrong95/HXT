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
    /// 代理订单附件的视图
    /// </summary>
    public class OrderFilesView : UniqueView<OrderFile, ScCustomsReponsitory>
    {
        public OrderFilesView()
        {
        }

        internal OrderFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderFile> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);

            return from orderFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderFiles>()
                   join admin in adminsView on orderFile.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join user in usersView on orderFile.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   select new Models.OrderFile
                   {
                       ID = orderFile.ID,
                       OrderID = orderFile.OrderID,
                       OrderItemID = orderFile.OrderItemID,
                       OrderPremiumID = orderFile.OrderPremiumID,
                       Admin = admin,
                       User = user,
                       Name = orderFile.Name,
                       FileType = (Enums.FileType)orderFile.FileType,
                       FileFormat = orderFile.FileFormat,
                       Url = orderFile.Url,
                       FileStatus = (Enums.OrderFileStatus)orderFile.FileStatus,
                       Status = (Enums.Status)orderFile.Status,
                       CreateDate = orderFile.CreateDate,
                       Summary = orderFile.Summary
                   };
        }
    }
}
