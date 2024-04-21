using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 考勤打卡日志
    /// </summary>
    public class LogsAttendRoll : UniqueView<Logs_Attend, PvbErmReponsitory>
    {
        public LogsAttendRoll()
        {

        }

        public LogsAttendRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Logs_Attend> GetIQueryable()
        {
            return new Logs_AttendOrigin();
        }
    }
}