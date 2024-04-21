using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Alls
{
    public class ShiftStaffsAll : UniqueView<ShiftStaff, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShiftStaffsAll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public ShiftStaffsAll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ShiftStaff> GetIQueryable()
        {
            var originView = new ShiftStaffsOrigin(this.Reponsitory);
            var staffView = new StaffsOrigin(this.Reponsitory);
            var adminView = new AdminsOrigin(this.Reponsitory);
            var schedulingView = new SchedulingsOrigin(this.Reponsitory);

            return from entity in originView
                   join staff in staffView on entity.ID equals staff.ID
                   join currentSch in schedulingView on staff.SchedulingID equals currentSch.ID
                   join nextSch in schedulingView on entity.NextSchedulingID equals nextSch.ID
                   join creator in adminView on entity.CreatorID equals creator.ID
                   join modify in adminView on entity.ModifyID equals modify.ID into admins
                   from modify in admins.DefaultIfEmpty()
                   select new ShiftStaff()
                   {
                       ID = entity.ID,
                       ShiftSchedule = entity.ShiftSchedule,
                       ShiftRules = entity.ShiftRules,
                       NextSchedulingID = entity.NextSchedulingID,
                       CreatorID = entity.CreatorID,
                       ModifyID = entity.ModifyID,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,

                       Staff = staff,
                       CurrentScheduling = currentSch,
                       NextScheduling = nextSch,
                       Creator = creator,
                       Modify = modify,
                   };
        }
    }
}
