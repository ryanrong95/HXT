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
    public class PvbErmStaffsTopView : UniqueView<XDTStaff, ScCustomsReponsitory>
    {
        public PvbErmStaffsTopView()
        {
        }

        public PvbErmStaffsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<XDTStaff> GetIQueryable()
        {
            return from xdtstaff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbErmStaffsTopView>()
                   select new XDTStaff
                   {
                       ID = xdtstaff.AdminID,
                       AdminID = xdtstaff.AdminID,
                       OriginID = xdtstaff.DyjCode,
                       StaffID = xdtstaff.ID,
                       DepartmentCode = xdtstaff.DyjDepartmentCode,
                   };
        }
    }
}
