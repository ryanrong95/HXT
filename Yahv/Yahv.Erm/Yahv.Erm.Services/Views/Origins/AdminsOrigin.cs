using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 管理员源视图
    /// </summary>
    internal class AdminsOrigin : UniqueView<Admin, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal AdminsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal AdminsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Admins>()
                   select new Admin()
                   {
                       ID = entity.ID,
                       Status = (AdminStatus)entity.Status,
                       RoleID = entity.RoleID,
                       RealName = entity.RealName,
                       StaffID = entity.StaffID,
                       CreateDate = entity.CreateDate,
                       SelCode = entity.SelCode,
                       UserName = entity.UserName,
                       UpdateDate = entity.UpdateDate,
                       LastLoginDate = entity.LastLoginDate,
                       Password = entity.Password,
                   };
        }
    }
}