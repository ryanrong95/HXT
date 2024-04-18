using Layers.Data.Sqls.PvbErm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class AdminsAll<TReponsitory> : UniqueView<Admin, TReponsitory> // PvbErmReponsitory
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsAll()
        {
            //System.Data.DataSet
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsAll(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<AdminsTopView>()
                   select new Admin()
                   {
                       ID = entity.ID,
                       Status = (AdminStatus)entity.Status,
                       RealName = entity.RealName,
                       StaffID = entity.StaffID,
                       SelCode = entity.SelCode,
                       UserName = entity.UserName,
                       LastLoginDate = entity.LastLoginDate,
                       RoleID = entity.RoleID,
                       RoleName = entity.RoleName,
                   };
        }

    }
}
