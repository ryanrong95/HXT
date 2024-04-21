using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    //public class AdminsOrigin : Yahv.Linq.UniqueView<Admin, PvbCrmReponsitory>
    //{
    //    /// <summary>
    //    /// 默认构造器
    //    /// </summary>
    //    internal AdminsOrigin()
    //    {

    //    }
    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="reponsitory">数据库连接</param>
    //    internal AdminsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
    //    {
    //    }
    //    protected override IQueryable<Admin> GetIQueryable()
    //    {
    //        var adminsView = new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>();
    //        return from admin in adminsView
    //               join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsAdmin>() on admin.ID equals maps.AdminID
    //               select new Admin
    //               {
    //                   ID = admin.ID,
    //                   Status = (AdminStatus)admin.Status,
    //                   RealName = admin.RealName,
    //                   StaffID = admin.StaffID,
    //                   SelCode = admin.SelCode,
    //                   UserName = admin.UserName,
    //                   LastLoginDate = admin.LastLoginDate,
    //                   //RoleID = admin.RoleID,
    //                   //RoleName = admin.RoleName,
    //                   IsDefault = maps.IsDefault,
    //                   RealID = maps.RealID,
    //                   Type = (MapsType)maps.Type
    //               };
    //    }
    //}
}
