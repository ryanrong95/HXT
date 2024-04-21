using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly.Enums;
using System;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 公有日程安排原始视图
    /// </summary>
    public class SchedulesPrivateOrigin : UniqueView<SchedulePrivate, PvbErmReponsitory>
    {
        public SchedulesPrivateOrigin()
        {

        }

        public SchedulesPrivateOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<SchedulePrivate> GetIQueryable()
        {
            var scheduleView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedules>();
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.SchedulesPrivate>()
                       join schedule in scheduleView on entity.ID equals schedule.ID
                       select new SchedulePrivate()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           SchedulePrivateType = (SchedulePrivateType)entity.Type,
                           AmOrPm = entity.AmOrPm,
                           StaffID = entity.StaffID,
                           OnWorkRemedy = entity.OnWorkRemedy,
                           OffWorkRemedy = entity.OffWorkRemedy,
                           CreateDate = entity.CreateDate,

                           Date = schedule.Date,
                           ModifyDate = schedule.ModifyDate,
                           Type = (ScheduleType)schedule.Type,
                           ModifyID = schedule.ModifyID,
                           CreatorID = schedule.CreatorID,
                       };
            return linq;
        }
    }
}