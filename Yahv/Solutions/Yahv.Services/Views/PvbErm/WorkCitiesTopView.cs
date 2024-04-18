using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models.PvbErm;
using Yahv.Underly;

namespace Yahv.Services.Views.PvbErm
{
    /// <summary>
    /// 员工所在地区
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class WorkCitiesTopView<TReponsitory> : QueryView<WorkCity, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WorkCitiesTopView()
        {
        }

        public WorkCitiesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<WorkCity> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.WorkCitiesTopView>()
                   select new WorkCity()
                   {
                       ID = entity.ID,
                       Status = (GeneralStatus)entity.Status,
                       Name = entity.Name,
                   };
        }
    }
}