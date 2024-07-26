using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class XDTStaffsTopView : UniqueView<XDTStaff, ScCustomsReponsitory>
    {
        public XDTStaffsTopView()
        {
        }

        public XDTStaffsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<XDTStaff> GetIQueryable()
        {
            return from xdtstaff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.XDTStaffsTopView>()
                   select new XDTStaff
                   {
                       ID = xdtstaff.AdminID,
                       AdminID = xdtstaff.AdminID,
                       OriginID = xdtstaff.OriginID,
                       StaffID = xdtstaff.StaffID,
                       DepartmentCode = xdtstaff.DepartmentCode,
                   };
        }
    }



    /// <summary>
    /// 华芯通员工
    /// </summary>
    public class XDTAdminStaffsTopView : UniqueView<XDTAdminStaff, ScCustomsReponsitory>
    {
        public XDTAdminStaffsTopView()
        {
        }

        public XDTAdminStaffsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<XDTAdminStaff> GetIQueryable()
        {
            return from xdtstaff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.XDTAdminStaffsTopView>()
                   select new XDTAdminStaff
                   {
                       ID = xdtstaff.ID,
                       OriginID = xdtstaff.OriginID,
                       StaffID = xdtstaff.StaffID,
                       RealName = xdtstaff.RealName,
                       Status  = xdtstaff.Status,
                       DyjCode = xdtstaff.DyjCode,
                       DyjCompanyCode = xdtstaff.DyjCompanyCode,
                       DyjCompany = xdtstaff.DyjCompany,
                       DyjDepartmentCode = xdtstaff.DyjDepartmentCode,
                       DyjDepartment = xdtstaff.DyjDepartment,
                       EntryCompany = xdtstaff.EntryCompany
                   };
        }
    }

    /// <summary>
    /// 所有员工
    /// </summary>
    public class AdminStaffsTopView : UniqueView<XDTAdminStaff, ScCustomsReponsitory>
    {
        public AdminStaffsTopView()
        {
        }

        public AdminStaffsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<XDTAdminStaff> GetIQueryable()
        {
            return from xdtstaff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminStaffsTopView>()
                   select new XDTAdminStaff
                   {
                       ID = xdtstaff.ID,
                       OriginID = xdtstaff.OriginID,
                       StaffID = xdtstaff.StaffID,
                       RealName = xdtstaff.RealName,
                       Status = xdtstaff.Status,
                       DyjCode = xdtstaff.DyjCode,
                       DyjCompanyCode = xdtstaff.DyjCompanyCode,
                       DyjCompany = xdtstaff.DyjCompany,
                       DyjDepartmentCode = xdtstaff.DyjDepartmentCode,
                       DyjDepartment = xdtstaff.DyjDepartment,
                       EntryCompany = xdtstaff.EntryCompany,
                       EnterpriseID = xdtstaff.EnterpriseID
                   };
        }
    }
}
