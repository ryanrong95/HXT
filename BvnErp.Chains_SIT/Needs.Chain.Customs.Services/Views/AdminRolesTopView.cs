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
    /// 管理员角色视图
    /// </summary>
    public class AdminRolesTopView : QueryView<AdminRole, ScCustomsReponsitory>
    {
        public AdminRolesTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal AdminRolesTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AdminRole> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminRolesTopView>()
                   select new AdminRole()
                   {
                       ID = entity.ID,
                       UserName = entity.UserName,
                       RealName = entity.RealName,
                       RoleID = entity.RoleID,
                       RoleName = entity.RoleName
                   };
        }
    }
}
