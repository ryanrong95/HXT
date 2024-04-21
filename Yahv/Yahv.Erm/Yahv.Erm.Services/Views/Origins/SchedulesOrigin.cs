using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 日程安排视图
    /// </summary>
    public class SchedulesOrigin : UniqueView<Schedule, PvbErmReponsitory>
    {
        public SchedulesOrigin()
        {

        }

        public SchedulesOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Schedule> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedules>()
                   select new Schedule()
                   {
                       ID = entity.ID,
                       Date = entity.Date,
                       ModifyDate = entity.ModifyDate,
                       Type = (ScheduleType)entity.Type,
                       ModifyID = entity.ModifyID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                   };
        }
    }
}