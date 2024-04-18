using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 归类锁定试图
    /// </summary>
    public class Locks_ClassifyAll : UniqueView<Models.Lock_Classify, ScCustomsReponsitory>
    {
        public Locks_ClassifyAll()
        {
        }

        internal Locks_ClassifyAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Lock_Classify> GetIQueryable()
        {
            var locks_ClassifyView = new Origins.Locks_ClassifyOrigin(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            return from entity in locks_ClassifyView
                   join admin in adminsView on entity.LockerID equals admin.ID
                   select new Models.Lock_Classify
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       LockDate = entity.LockDate,
                       LockerID = entity.LockerID,
                       Locker = admin,
                       Summary = entity.Summary
                   };
        }
    }
}
