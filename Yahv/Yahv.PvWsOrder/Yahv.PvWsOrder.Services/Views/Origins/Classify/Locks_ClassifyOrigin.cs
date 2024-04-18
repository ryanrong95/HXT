using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 归类锁定试图
    /// </summary>
    internal class Locks_ClassifyOrigin : UniqueView<Models.Lock_Classify, PvWsOrderReponsitory>
    {
        internal Locks_ClassifyOrigin()
        {
        }

        internal Locks_ClassifyOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Lock_Classify> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Locks_ClassifyTopView>()
                   select new Models.Lock_Classify
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       LockDate = entity.LockDate,
                       LockerID = entity.LockerID,
                       Status = (Underly.GeneralStatus)entity.Status,
                       Summary = entity.Summary
                   };
        }
    }
}
