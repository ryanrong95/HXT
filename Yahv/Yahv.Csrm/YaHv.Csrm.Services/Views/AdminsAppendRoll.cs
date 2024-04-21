using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views
{
    public class ConAdminsView : Yahv.Linq.UniqueView<Admin, PvbCrmReponsitory>
    {
        public ConAdminsView()
        {

        }
        protected override IQueryable<Admin> GetIQueryable()
        {
            var adminsView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>(this.Reponsitory);
            return from admin in adminsView
                   select new Admin
                   {
                       ID = admin.ID,
                       Status = admin.Status,
                       RealName = admin.RealName,
                       StaffID = admin.StaffID,
                       SelCode = admin.SelCode,
                       UserName = admin.UserName,
                       LastLoginDate = admin.LastLoginDate,
                       RoleID = admin.RoleID,
                       RoleName = admin.RoleName
                   };
        }



        public IQueryable<Admin> Search(params Yahv.Underly.FixedRole[] roles)
        {
            var arry = roles.Select(role => role.GetFixedID());

            //用 固定角色 作为条件获取真实 角色
            //利用真实角色 获取Admins 备选表（右侧）
            var iQuery = this.IQueryable;
            var linq = from map in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsRoleComposeTopView>()
                       join admins in this.IQueryable
                       on map.ID equals admins.RoleID
                       where arry.Contains(map.ChildID)
                       select admins;
            return linq;
        }


    }
}


