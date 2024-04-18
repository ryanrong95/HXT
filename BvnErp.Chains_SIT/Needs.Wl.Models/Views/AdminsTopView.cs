using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class AdminsTopView : View<Admin, ScCustomsReponsitory>
    {
        public AdminsTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public AdminsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 获取人员集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>()
                   join adminWl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>() on admin.ID equals adminWl.AdminID into adminWlTemp
                   from adminEntity in adminWlTemp.DefaultIfEmpty()
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       Tel = adminEntity != null ? adminEntity.Tel : string.Empty,
                       Mobile = adminEntity != null ? adminEntity.Mobile : string.Empty,
                       Email = adminEntity != null ? adminEntity.Email : string.Empty,
                       Password = admin.Password,
                       CreateDate = admin.CreateDate,
                       LoginDate = admin.LoginDate,
                       UpdateDate = admin.UpdateDate,
                       Summary = admin.Summary
                   };
        }
    }
}
