using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;

namespace YaHv.CrmPlus.Services.Views.Rolls
{
    /// <summary>
    /// 人员角色关系
    /// </summary>
    public class AdminsRolesAllRoll : Yahv.Linq.UniqueView<Admin, PvdCrmReponsitory>
    {
        public AdminsRolesAllRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsRolesAllRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.AdminsRolesTopView>()
                   select new Admin
                   {
                       ID = admin.ID,
                       RealName = admin.RealName,
                       UserName = admin.UserName,
                       RoleID = admin.RoleID,
                       RoleName = admin.RoleName
                   };
        }
    }
}
