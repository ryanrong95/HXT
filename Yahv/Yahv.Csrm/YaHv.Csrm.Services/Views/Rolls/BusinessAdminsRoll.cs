using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class BusinessAdminsRoll : Yahv.Linq.UniqueView<BusinessAdmin, PvbCrmReponsitory>
    {
        public BusinessAdminsRoll(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
            ;
        }
        public BusinessAdminsRoll()
        {

        }
        protected override IQueryable<BusinessAdmin> GetIQueryable()
        {
            var adminsView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>(this.Reponsitory);
            return from admin in adminsView
                   join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsAdmin>() on admin.ID equals maps.AdminID
                   select new BusinessAdmin
                   {
                       ID = admin.ID,
                       Status = admin.Status,
                       RealName = admin.RealName,
                       StaffID = admin.StaffID,
                       SelCode = admin.SelCode,
                       UserName = admin.UserName,
                       LastLoginDate = admin.LastLoginDate,
                       IsDefault = maps.IsDefault,
                       RealID = maps.RealID,
                       Type = (MapsType)maps.Type,
                       RoleID = admin.RoleID,
                       RoleName = admin.RoleName
                   };
        }

    }

    public class TrackerAdmin : Yahv.Linq.UniqueView<BusinessAdmin, PvbCrmReponsitory>
    {
        string RealID;
        MapsType Type;
        public TrackerAdmin(string Realid, MapsType type)
        {
            this.RealID = Realid;
            this.Type = type;
        }
        protected override IQueryable<BusinessAdmin> GetIQueryable()
        {
            var adminsView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>(this.Reponsitory);
            var linq = from maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTracker>()
                       join admin in adminsView on maps.AdminID equals admin.ID
                       where maps.RealID == this.RealID && maps.Type == (int)this.Type
                       select new BusinessAdmin
                       {
                           ID = admin.ID,
                           Status = admin.Status,
                           RealName = admin.RealName,
                           StaffID = admin.StaffID,
                           SelCode = admin.SelCode,
                           UserName = admin.UserName,
                           LastLoginDate = admin.LastLoginDate,
                           IsDefault = maps.IsDefault,
                           RealID = maps.RealID,
                           Type = (MapsType)maps.Type,
                           RoleID = admin.RoleID,
                           RoleName = admin.RoleName
                       };
            return linq;

        }
    }

    /// <summary>
    /// 销售或采购(仅限MapsType.Client; MapsType.Supplier)
    /// </summary>
    /// <summary>
    /// 销售或采购(仅限MapsType.Client; MapsType.Supplier)
    /// </summary>
    public class TradingAdminsRoll : Yahv.Linq.UniqueView<TradingAdmin, PvbCrmReponsitory>
    {
        MapsType Type;
        public TradingAdminsRoll(PvbCrmReponsitory reponsitory, MapsType Type) : base(reponsitory)
        {
            this.Type = Type;
        }
        public TradingAdminsRoll(MapsType Type)
        {
            this.Type = Type;
        }
        protected override IQueryable<TradingAdmin> GetIQueryable()
        {
            var adminsView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>(this.Reponsitory);
            return from admin in adminsView
                   join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on admin.ID equals maps.SubID
                   where maps.Bussiness == (int)Business.Trading && maps.Type == (int)Type
                   select new TradingAdmin
                   {
                       ID = admin.ID,
                       EnterpriseID = maps.EnterpriseID,
                       Status = admin.Status,
                       RealName = admin.RealName,
                       StaffID = admin.StaffID,
                       SelCode = admin.SelCode,
                       UserName = admin.UserName,
                       LastLoginDate = admin.LastLoginDate,
                       RoleID = admin.RoleID,
                       RoleName = admin.RoleName,
                       IsDefault = maps.IsDefault,
                   };
        }
    }




    #region 合作公司
    //public class TradingAdminsRoll : Yahv.Linq.UniqueView<TradingAdmin, PvbCrmReponsitory>
    //{
    //    MapsType Type;
    //    Business business;
    //    public TradingAdminsRoll(PvbCrmReponsitory reponsitory, MapsType Type, Business business) : base(reponsitory)
    //    {
    //        this.Type = Type;
    //        this.business = business;
    //    }
    //    public TradingAdminsRoll(MapsType Type, Business business)
    //    {
    //        this.Type = Type;

    //        this.business = business;
    //    }
    //    protected override IQueryable<TradingAdmin> GetIQueryable()
    //    {
    //        var adminsView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>(this.Reponsitory);
    //        return from admin in adminsView
    //               join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on admin.ID equals maps.SubID
    //               where maps.Bussiness == (int)Business.Trading && maps.Type == (int)Type
    //               select new TradingAdmin
    //               {
    //                   ID = admin.ID,
    //                   EnterpriseID = maps.EnterpriseID,
    //                   Status = admin.Status,
    //                   RealName = admin.RealName,
    //                   StaffID = admin.StaffID,
    //                   SelCode = admin.SelCode,
    //                   UserName = admin.UserName,
    //                   LastLoginDate = admin.LastLoginDate,
    //                   RoleID = admin.RoleID,
    //                   RoleName = admin.RoleName,
    //                   IsDefault = maps.IsDefault,
    //               };

    //    }
    //}
    #endregion
}
