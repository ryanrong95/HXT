using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 公有日程安排原始视图
    /// </summary>
    public class SchedulesPublicOrigin : UniqueView<SchedulePublic, PvbErmReponsitory>
    {
        public SchedulesPublicOrigin()
        {

        }

        public SchedulesPublicOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<SchedulePublic> GetIQueryable()
        {
            var scheduleView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedules>();
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.SchedulesPublic>()
                       join schedule in scheduleView on entity.ID equals schedule.ID
                       select new SchedulePublic()
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           PostionID = entity.PostionID,
                           ShiftID = entity.ShiftID,
                           From = (ScheduleFrom)entity.From,
                           SalaryMultiple = entity.SalaryMultiple ?? 1m,
                           Method = (ScheduleMethod)entity.Method,
                           RegionID = entity.RegionID,
                           SchedulingID = entity.SchedulingID,

                           Date = schedule.Date,
                           ModifyDate = schedule.ModifyDate,
                           Type = (ScheduleType)schedule.Type,
                           ModifyID = schedule.ModifyID,
                           CreatorID = schedule.CreatorID,
                           CreateDate = schedule.CreateDate,
                       };
            return linq;
        }
    }
}