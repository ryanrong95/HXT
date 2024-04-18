using Yahv.Plats.Services.Models;
using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
namespace Yahv.Plats.Services.Views.Origins
{
    /// <summary>
    /// 管理员 视图
    /// </summary>
    public class AdminsOrigin : UniqueView<Models.Origins.Admin, PvbErmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal AdminsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        internal AdminsOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 管理员 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        sealed protected override IQueryable<Models.Origins.Admin> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>()
                   select new Models.Origins.Admin
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       UserName = entity.UserName,
                       RealName = entity.RealName,
                       Password = entity.Password,
                       SelCode = entity.SelCode,
                       RoleID = entity.RoleID,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (Underly.AdminStatus)entity.Status,
                       LastLoginDate = entity.LastLoginDate,
                       PwdModifyDate = entity.PwdModifyDate,
                   };
        }
    }
}