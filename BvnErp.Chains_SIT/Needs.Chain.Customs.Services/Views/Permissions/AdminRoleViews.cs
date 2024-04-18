using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 角色的视图
    /// </summary>
    public class AdminRoleViews : UniqueView<Models.AdminRoles, ScCustomsReponsitory>
    {
        public AdminRoleViews()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public AdminRoleViews(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<AdminRoles> GetIQueryable()
        {
            var roleView = new Views.RoleViews(this.Reponsitory);
            var adminView=new Views.AdminsTopView(this.Reponsitory);
            var result = from adminroles in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminRoles>()
                         join role in roleView on adminroles.RoleID equals role.ID
                         join adminWl in adminView on adminroles.AdminID equals adminWl.ID
                         select new Models.AdminRoles
                         {
                             ID = adminroles.ID,
                             CreateDate = adminroles.CreateDate,
                             Role=role,
                             Admin=adminWl,  
                         };
            return result;
        }
    }
}
