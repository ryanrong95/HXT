using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class OrderFilesView : View<Models.OrderFile, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderFilesView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.OrderFile> GetIQueryable()
        {
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderFiles>()
                   join admins in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on file.AdminID equals admins.ID into Admins
                   from admin in Admins.DefaultIfEmpty()
                   join users in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on file.UserID equals users.ID into Users
                   from user in Users.DefaultIfEmpty()
                   where file.OrderID == this.OrderID && file.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Models.OrderFile
                   {
                       ID = file.ID,
                       OrderID = file.OrderID,
                       Name = file.Name,
                       FileType = (Enums.FileType)file.FileType,
                       FileFormat = file.FileFormat,
                       Url = file.Url,
                       Admin = new Admin()
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName
                       },
                       User = new User()
                       {
                           ID = user.ID,
                           RealName = user.RealName,
                           Name = user.Name
                       },
                       Status = file.Status,
                       FileStatus = (Enums.OrderFileStatus)file.FileStatus,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }

        /// <summary>
        /// 代理报关委托书
        /// </summary>
        /// <returns></returns>
        public Models.OrderFile GetAgentTrustInstrument()
        {
            return this.GetIQueryable().Where(s => s.FileType == Enums.FileType.AgentTrustInstrument).FirstOrDefault();
        }

        /// <summary>
        /// 合同发票
        /// </summary>
        /// <returns></returns>
        public List<Models.OrderFile> GetOriginalInvoice()
        {
            return this.GetIQueryable().Where(s => s.FileType == Enums.FileType.OriginalInvoice).ToList();
        }

        /// <summary>
        /// 提货文件
        /// </summary>
        /// <returns></returns>
        public Models.OrderFile GetDeliveryFiles()
        {
            return this.GetIQueryable().Where(s => s.FileType == Enums.FileType.DeliveryFiles).FirstOrDefault();
        }
    }
}