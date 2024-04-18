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
    public class Locks_ClassifyAll : UniqueView<Models.Lock_Classify, PvWsOrderReponsitory>
    {
        public Locks_ClassifyAll()
        {
        }

        internal Locks_ClassifyAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Lock_Classify> GetIQueryable()
        {
            var locks_ClassifyView = new Locks_ClassifyOrigin(this.Reponsitory).Where(cl => cl.Status == Underly.GeneralStatus.Normal);
            var adminsView = new AdminsAll(this.Reponsitory);
            return from entity in locks_ClassifyView
                   join admin in adminsView on entity.LockerID equals admin.ID
                   select new Models.Lock_Classify
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       LockDate = entity.LockDate,
                       LockerID = entity.LockerID,
                       Locker = admin,
                       Status = entity.Status,
                       Summary = entity.Summary
                   };
        }
    }
}
