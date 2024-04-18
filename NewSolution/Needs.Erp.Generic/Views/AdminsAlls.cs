using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Erp.Generic.Views
{
    public class AdminsAlls : UniqueView<Models.Admin, BvnErpReponsitory>
    {
        protected internal AdminsAlls()
        {
        }

        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       Status = (Status)admin.Status,
                       CreateDate = admin.CreateDate,
                       LoginDate = admin.LoginDate,
                       Password = admin.Password,
                       Summary = admin.Summary,
                       UpdateDate = admin.UpdateDate
                   };
        }
    }
}
