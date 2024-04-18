using Layers.Data.Sqls.PvbErm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class MyAdmins<TReponsitory> : UniqueView<Admin, TReponsitory> // PvbErmReponsitory
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyAdmins()
        {
        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            var result = from admin in this.Reponsitory.ReadTable<AdminsTopView>()
                         select new Admin()
                         {
                             ID = admin.ID,
                             Status = (Underly.AdminStatus)admin.Status,
                             RealName = admin.RealName,
                             StaffID = admin.StaffID,
                             SelCode = admin.SelCode,
                             UserName = admin.UserName,
                             LastLoginDate = admin.LastLoginDate,
                         };
            return result;
        }



    }
}
