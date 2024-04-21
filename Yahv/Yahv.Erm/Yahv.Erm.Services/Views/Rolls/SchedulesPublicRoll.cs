using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 公共日程安排视图
    /// </summary>
    public class SchedulesPublicRoll : UniqueView<SchedulePublic, PvbErmReponsitory>
    {
        public SchedulesPublicRoll()
        {

        }

        public SchedulesPublicRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<SchedulePublic> GetIQueryable()
        {
            var schedulesPubl = new SchedulesPublicOrigin(this.Reponsitory);
            var schedulings = new SchedulingsOrigin(this.Reponsitory);
            var regions = new RegionsAcOrigin(this.Reponsitory);

            return from sp in schedulesPubl
                   join st in schedulings on sp.ShiftID equals st.ID
                   join ss in schedulings on sp.SchedulingID equals ss.ID
                   join rg in regions on sp.RegionID equals rg.ID
                   select new SchedulePublic()
                   {
                       ID = sp.ID,
                       Name = sp.Name,
                       PostionID = sp.PostionID,
                       ShiftID = sp.ShiftID,
                       From = sp.From,
                       SalaryMultiple = sp.SalaryMultiple,
                       Method = sp.Method,
                       RegionID = sp.RegionID,
                       SchedulingID = sp.SchedulingID,
                       SchedulingName = ss.Name,
                       ShiftName = st.Name,
                       RegionName = rg.Name,
                       Date = sp.Date,
                       CreateDate = sp.CreateDate,
                       ModifyDate = sp.ModifyDate,
                       CreatorID = sp.CreatorID,
                   };
        }
    }
}