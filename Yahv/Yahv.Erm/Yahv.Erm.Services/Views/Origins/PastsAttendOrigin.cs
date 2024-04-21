using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 考勤统计原始视图
    /// </summary>
    internal class PastsAttendOrigin : UniqueView<PastsAttend, PvbErmReponsitory>
    {
        public PastsAttendOrigin()
        {

        }

        public PastsAttendOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PastsAttend> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Pasts_Attend>()
                   select new PastsAttend()
                   {
                       ID = entity.ID,
                       Date = entity.Date,
                       StaffID = entity.StaffID,
                       CreateDate = entity.CreateDate,
                       SchedulingID = entity.SchedulingID,
                       ModifyDate = entity.ModifyDate,
                       AmOrPm = entity.AmOrPm,
                       EndTime = entity.EndTime,
                       InFact = (AttendInFactType)entity.InFact,
                       IsEarly = entity.IsEarly,
                       IsLater = entity.IsLater,
                       OffWorkRemedy = entity.OffWorkRemedy,
                       OnWorkRemedy = entity.OnWorkRemedy,
                       StartTime = entity.StartTime,
                   };
        }
    }
}