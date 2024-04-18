using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.AttendanceData.Import.Models;
using Yahv.Underly.Enums;

namespace Yahv.AttendanceData.Import.Views
{
    /// <summary>
    /// 公共日程安排的视图
    /// </summary>
    public class SchedulesPublicView : UniqueView<Models.SchedulePublic, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal SchedulesPublicView() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal SchedulesPublicView(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<SchedulePublic> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.SchedulesPublic>()
                   join schedule in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedules>() on entity.ID equals schedule.ID
                   select new Models.SchedulePublic()
                   {
                       ID = entity.ID,
                       Method = (ScheduleMethod)entity.Method,
                       Name = entity.Name,
                       From = (ScheduleFrom)entity.From,
                       RegionID = entity.RegionID,
                       PostionID = entity.PostionID,
                       SalaryMultiple = entity.SalaryMultiple,
                       ShiftID = entity.ShiftID,
                       SchedulingID = entity.SchedulingID,

                       Date = schedule.Date
                   };
        }
    }
}
