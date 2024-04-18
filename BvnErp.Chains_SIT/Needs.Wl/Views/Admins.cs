using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Models;

namespace Needs.Wl.Views
{
    /// <summary>
    /// 芯达通平台管理员
    /// 授权进入芯达通系统的管理员
    /// </summary>
    public class Admins : UniqueView<Needs.Wl.Admin.Plat.Models.Admin, ScCustomsReponsitory>
    {
        protected override IQueryable<Needs.Wl.Admin.Plat.Models.Admin> GetIQueryable()
        {

            return from adminWl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>()
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>() on adminWl.AdminID equals admin.OriginID
                   join depart in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Departments>() on adminWl.DepartmentID equals depart.ID into depart_temp
                   from depart in depart_temp.DefaultIfEmpty()
                   select new Needs.Wl.Admin.Plat.Models.Admin
                   {
                       ID = admin.OriginID, //adminWl.AdminID,
                       RealName = admin.RealName,
                       UserName = admin.UserName,
                       Tel = adminWl.Tel,
                       Mobile = adminWl.Mobile,
                       Email = adminWl.Email,
                       DepartmentID = adminWl.DepartmentID,
                       DepartmentName = depart != null ? depart.Name : null,
                       Summary = adminWl.Summary,
                       ByName = adminWl.Byname

                   };

            //if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["IsChains"]))
            //{
            //    return from adminWl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>()
            //           join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>() on adminWl.AdminID equals admin.OriginID
            //           select new Needs.Wl.Admin.Plat.Models.Admin
            //           {
            //               ID = admin.OriginID, //adminWl.AdminID,
            //               RealName = admin.RealName,
            //               UserName = admin.UserName,
            //               Tel = adminWl.Tel,
            //               Mobile = adminWl.Mobile,
            //               Email = adminWl.Email,
            //               Summary = adminWl.Summary
            //           };
            //}
            //else
            //{
            //    return from adminWl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>()
            //           join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on adminWl.AdminID equals admin.ID
            //           select new Needs.Wl.Admin.Plat.Models.Admin
            //           {
            //               ID = adminWl.AdminID,
            //               RealName = admin.RealName,
            //               UserName = admin.UserName,
            //               Tel = adminWl.Tel,
            //               Mobile = adminWl.Mobile,
            //               Email = adminWl.Email,
            //               Summary = adminWl.Summary
            //           };
            //}
        }
    }
}
