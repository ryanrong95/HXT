using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 用户支出视图
    /// </summary>
    public class UserOutputsAdmins : UniqueView<Models.AdminUserOutput, CvOssReponsitory>
    {
        string orderID;



        public UserOutputsAdmins(string orderID)
        {
            this.orderID = orderID;
        }

        protected override IQueryable<Models.AdminUserOutput> GetIQueryable()
        {
            var clientsView = new ClientsTopView(this.Reponsitory);
            var adminsTopView = new NtErp.Services.Views.AdminsTopView<CvOssReponsitory>(this.Reponsitory);


            return from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                   join _admin in adminsTopView on entity.AdminID equals _admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where entity.OrderID == this.orderID
                   select new Models.AdminUserOutput
                   {

                       ID = entity.ID,
                       ClientID = entity.ClientID,
                       Type = (UserAccountType)entity.Type,
                       From = (OutputTo)entity.From,
                       OrderID = entity.OrderID,
                       Currency = (Needs.Underly.Currency)entity.Currency,
                       Amount = entity.Amount,
                       CreateDate = entity.CreateDate,
                       UserInputID = entity.UserInputID,
                       DateIndex = entity.DateIndex,
                       Admin = admin,
                   };
        }
    }
}
