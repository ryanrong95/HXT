using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{



    /// <summary>
    /// 业务管理员视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class AdminsBusiness<TReponsitory> : UniqueView<Admin, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        SysBusiness Business;
        public AdminsBusiness(SysBusiness bussiness)
        {
            this.Business = bussiness;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        public AdminsBusiness(TReponsitory reponsitory, SysBusiness bussiness) : base(reponsitory)
        {
            this.Business = bussiness;
        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            string name = this.Business.GetDescription();
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.AdminsBussinessTopView>()
                   where entity.bussiness == name
                   select new Models.Admin
                   {
                       ID = entity.ID,
                       UserName = entity.UserName,
                       RealName = entity.RealName,
                       SelCode = entity.SelCode,
                       Status = (AdminStatus)entity.Status,
                       LastLoginDate = entity.LastLoginDate,
                       StaffID = entity.StaffID,
                       RoleID = entity.RoleID,
                       RoleName = entity.RoleName,
                   };
        }
    }
}
