using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.AttendService
{
    /// <summary>
    /// 员工假期初始化任务
    /// </summary>
    public class InitStaffVocationTask
    {
        public InitStaffVocationTask()
        {
        }

        public void Init()
        {
            var staffs = new Erm.Services.Views.StaffAlls()
                .Where(item => item.Labour.EnterpriseID == Services.Common.ErmConfig.LabourEnterpriseID || item.Labour.EnterpriseID == Services.Common.ErmConfig.LabourEnterpriseID2)
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).ToArray();

            foreach (var staff in staffs)
            {
                staff.InitVacation();
            }
        }
    }
}
