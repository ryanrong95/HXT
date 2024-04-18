using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    public class AdminsAlls : UniqueView<Models.Admin, BvnErpReponsitory>
    {
        public AdminsAlls()
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
                       Password = admin.Password,
                       Summary = admin.Summary,
                       CreateDate = admin.CreateDate,
                       UpdateDate = admin.UpdateDate,
                       LoginDate = admin.LoginDate,
                       Status = (Needs.Erp.Generic.Status)admin.Status,
                   };
        }
    }
}
