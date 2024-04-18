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
    public class RoleViews : UniqueView<Models.Role, ScCustomsReponsitory>
    {
        public RoleViews()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public RoleViews(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Role> GetIQueryable()
        {
            var result = from roles in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Roles>()                         
                         where roles.Status == (int)Enums.Status.Normal
                         select new Models.Role
                         {
                             ID = roles.ID,
                             Name = roles.Name,                              
                             SysCode=roles.SysCode,
                             Status = (Enums.Status)roles.Status,
                             Summary = roles.Summary,
                             UpdateDate = roles.UpdateDate,
                             CreateDate = roles.CreateDate,
                         };
            return result;
        }
    }
}
