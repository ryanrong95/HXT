using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Underly;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{

    public class AdminTopView : UniqueView<AdminTop, BvCrmReponsitory>, IFkoView<AdminTop>
    {

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public AdminTopView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal AdminTopView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        /// 获取人员集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<AdminTop> GetIQueryable()
        {
            var companyall = new CompanyAlls(this.Reponsitory);

            return from adminTop in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>()
                   join adminProject in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminsProject>() on adminTop.ID equals adminProject.AdminID into adminprojects
                   from project in adminprojects.DefaultIfEmpty()
                   join _company in companyall on project.CompanyID equals _company.ID into companys
                   from company in companys.DefaultIfEmpty()
                   select new AdminTop
                   {
                       ID = adminTop.ID,
                       UserName = adminTop.UserName,
                       RealName = adminTop.RealName,
                       Company = company,
                       JobType = project != null ? (JobType)project.JobType : 0,
                       Token = project != null ? project.Token : null,
                       WXID = project != null ? project.WXID : null,
                       IsAgree = project != null ? project.IsAgree : false,
                       CreateDate = project != null ? project.CreateDate : adminTop.CreateDate,
                       UpdateDate = project != null ? project.UpdateDate : adminTop.UpdateDate,
                       ScoreType = project != null ? (ScoreType?)project.ScoreType : null,
                       SalaryBase = project != null ? project.SalaryBase : null,
                       DyjID = project != null ? project.DyjID : null,
                       Summary = project != null ? project.Summary : string.Empty,
                   };
        }


        /// <summary>
        /// 获取当前区域经理
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        public AdminTop[] GetLead(string districtid, DistrictType type)
        {
            var linq = from admin in this.GetIQueryable()
                       join maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>()
                       on admin.ID equals maps.LeadID
                       where maps.DistrictID == districtid && maps.Type == (int)type
                       select admin;
            return linq.Distinct().ToArray();
        }


        /// <summary>
        /// 获取当前区域员工
        /// </summary>
        /// <param name="districtid"></param>
        /// <returns></returns>
        public AdminTop[] GetAdmin(string districtid, DistrictType type)
        {
            var linq = from admin in this.GetIQueryable()
                       join maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDistrict>()
                       on admin.ID equals maps.AdminID
                       where maps.DistrictID == districtid && maps.Type == (int)type
                       select admin;
            return linq.Distinct().ToArray();
        }
    }
}
