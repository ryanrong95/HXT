using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AdminsTopView : UniqueView<Admin, ScCustomsReponsitory>, IFkoView<Admin>
    {
        public AdminsTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public AdminsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 获取人员集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>()
                       //join adminWl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>() on admin.ID equals adminWl.AdminID into adminWlTemp
                       //from adminEntity in adminWlTemp.DefaultIfEmpty()
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       Tel = admin.Tel,
                       Mobile = admin.Mobile,
                       Email = admin.Email,
                       DepartmentID = admin.DepartmentID,
                       //Tel = adminEntity != null ? adminEntity.Tel : string.Empty,
                       //Mobile = adminEntity != null ? adminEntity.Mobile : string.Empty,
                       //Email = adminEntity != null ? adminEntity.Email : string.Empty,
                       //DepartmentID = adminEntity.DepartmentID,
                       Password = admin.Password,
                       CreateDate = admin.CreateDate,
                       LoginDate = admin.LoginDate,
                       UpdateDate = admin.UpdateDate,
                       Summary = admin.Summary,
                       ByName = admin.Byname
                   };
        }
    }

    public class AdminsTopView2 : UniqueView<Admin, ScCustomsReponsitory>, IFkoView<Admin>
    {
        public AdminsTopView2()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public AdminsTopView2(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 获取人员集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>()
                   join adminWl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>() on admin.OriginID equals adminWl.AdminID into adminWlTemp
                   from adminEntity in adminWlTemp.DefaultIfEmpty()
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       Tel = adminEntity != null ? adminEntity.Tel : string.Empty,
                       Mobile = adminEntity != null ? adminEntity.Mobile : string.Empty,
                       Email = adminEntity != null ? adminEntity.Email : string.Empty,
                       ByName = adminEntity != null ? adminEntity.Byname : string.Empty,//新增别名
                       Password = admin.Password,
                       CreateDate = admin.CreateDate,
                       LoginDate = admin.LastLoginDate,
                       UpdateDate = admin.UpdateDate != null ? admin.UpdateDate.Value : admin.CreateDate,
                       OriginID = admin.OriginID,
                       ErmAdminID = admin.ID,
                       DyjCode = admin.DyjCode,
                       DyjCompanyCode = admin.DyjCompanyCode,
                       DyjCompany = admin.DyjCompany,
                       DyjDepartmentCode = admin.DyjDepartmentCode,
                       DyjDepartment = admin.DyjDepartment,
                       Status = admin.Status,
                   };
        }
    }
}
