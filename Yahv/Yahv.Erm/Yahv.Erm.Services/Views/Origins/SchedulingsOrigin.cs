using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 班别原始视图
    /// </summary>
    public class SchedulingsOrigin : UniqueView<Scheduling, PvbErmReponsitory>
    {
        public SchedulingsOrigin()
        {

        }

        public SchedulingsOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Scheduling> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedulings>()
                   select new Scheduling()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       PostionID = entity.PostionID,
                       PmStartTime = entity.PmStartTime,
                       PmEndTime = entity.PmEndTime,
                       AmEndTime = entity.AmEndTime,
                       DomainValue = entity.DomainValue,
                       AmStartTime = entity.AmStartTime,
                       Summary = entity.Summary,
                       IsMain = entity.IsMain,
                   };
        }
    }
}