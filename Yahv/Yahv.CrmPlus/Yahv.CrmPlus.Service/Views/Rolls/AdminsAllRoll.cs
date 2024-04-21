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
    public class AdminsAllRoll : Yahv.Linq.UniqueView<Admin, PvdCrmReponsitory>
    {
        public AdminsAllRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsAllRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.AdminsTopView>()
                   select new Admin
                   {
                       ID = admin.ID,
                       Status = (AdminStatus)admin.Status,
                       RealName = admin.RealName,
                       StaffID = admin.StaffID,
                       SelCode = admin.SelCode,
                       UserName = admin.UserName,
                       LastLoginDate = admin.LastLoginDate,
                       RoleID = admin.RoleID,
                       RoleName = admin.RoleName
                   };
        }
    }
}
