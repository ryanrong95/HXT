using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class AdminsRoll : UniqueView<Admin, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsRoll()
        {
        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            var adminsView = new AdminsOrigin(this.Reponsitory);
            var rolesView = new RolesOrigin(this.Reponsitory);

            var result = from admin in adminsView
                         join _role in rolesView on admin.RoleID equals _role.ID into joinRole
                         from role in joinRole.DefaultIfEmpty()
                         select new Admin()
                         {
                             ID = admin.ID,
                             Status = admin.Status,
                             RoleID = admin.RoleID,
                             RealName = admin.RealName,
                             StaffID = admin.StaffID,
                             CreateDate = admin.CreateDate,
                             SelCode = admin.SelCode,
                             UserName = admin.UserName,
                             UpdateDate = admin.UpdateDate,
                             RoleName = role.Name,
                             LastLoginDate = admin.LastLoginDate,
                             Password = admin.Password
                         };

            return result;
        }

        /// <summary>
        /// 已重写 索引器
        /// </summary>
        /// <param name="id">唯一码</param>
        /// <returns>Partner</returns>
        public override Admin this[string id]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == id);
            }
        }
    }
}