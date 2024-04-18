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
    public class MainOrderFilesView : UniqueView<MainOrderFile, ScCustomsReponsitory>
    {
        public MainOrderFilesView()
        {
        }

        internal MainOrderFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MainOrderFile> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);

            return from orderFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrderFiles>()
                   join admin in adminsView on orderFile.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join user in usersView on orderFile.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   where orderFile.Status == (int)Enums.Status.Normal
                   select new Models.MainOrderFile
                   {
                       ID = orderFile.ID,
                       MainOrderID = orderFile.MainOrderID,
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


    public class CenterLinkXDTFilesTopView : UniqueView<MainOrderFile, ScCustomsReponsitory>
    {

        public CenterLinkXDTFilesTopView()
        {
        }

        internal CenterLinkXDTFilesTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MainOrderFile> GetIQueryable()
        {
            var orderFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CenterLinkXDTFilesTopView>();
            return from orderFile in orderFiles
                  // where orderFile.Status!=(int)Enums.Status.Delete
                   select new MainOrderFile
                   {
                       ID = orderFile.ID,
                       MainOrderID = orderFile.MainOrderID,
                       Name = orderFile.Name,
                       FileType = (Enums.FileType)orderFile.FileType,
                       AdminID = orderFile.AdminID,
                       //UserID = orderFile.UserID,
                       Url = orderFile.Url,
                       FileStatus = (Enums.OrderFileStatus)orderFile.FileStatus,
                       Status = (Enums.Status)orderFile.Status,
                       CreateDate = orderFile.CreateDate,
                       DataType=orderFile.DateType
                   };
        }
    }



}
