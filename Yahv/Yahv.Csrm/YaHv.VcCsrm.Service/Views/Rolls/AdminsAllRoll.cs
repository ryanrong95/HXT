using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class AdminsAllRoll : Yahv.Linq.UniqueView<Models.Admin, PvcCrmReponsitory>
    {
        public AdminsAllRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsAllRoll(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            var adminsView = new Yahv.Services.Views.AdminsAll<PvcCrmReponsitory>(this.Reponsitory);
            return from admin in adminsView
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       //Status = admin.Status,
                       RealName = admin.RealName,
                       //StaffID = admin.StaffID,
                       //SelCode = admin.SelCode,
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

    public class TrackerAdmin : Yahv.Linq.UniqueView<Models.Admin, PvcCrmReponsitory>
    {
        string RealID;
        MapsType Type;
        public TrackerAdmin(string Realid, MapsType type)
        {
            this.RealID = Realid;
            this.Type = type;
        }
        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            var adminsView = new Yahv.Services.Views.AdminsAll<PvcCrmReponsitory>(this.Reponsitory);
            var linq = from maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.MapsTracker>()
                       join admin in adminsView on maps.AdminID equals admin.ID
                       where maps.RealID == this.RealID && maps.Type == (int)this.Type
                       select new Models.Admin
                       {
                           ID = admin.ID,
                           //Status = admin.Status,
                           RealName = admin.RealName,
                           //StaffID = admin.StaffID,
                           //SelCode = admin.SelCode,
                           UserName = admin.UserName,
                           LastLoginDate = admin.LastLoginDate,
                           RoleID = admin.RoleID,
                           RoleName = admin.RoleName
                       };
            return linq;

        }
    }
}
