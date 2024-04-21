using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class AdminsAllRoll : Yahv.Linq.UniqueView<Admin, PvbCrmReponsitory>
    {
        public AdminsAllRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsAllRoll(PvbCrmReponsitory reponsitory) : base(reponsitory)
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
        //public override Admin this[string ID]
        //{
        //    get
        //    {
        //        return this.SingleOrDefault(item => item.ID == ID);
        //    }
        //}
    }
}
