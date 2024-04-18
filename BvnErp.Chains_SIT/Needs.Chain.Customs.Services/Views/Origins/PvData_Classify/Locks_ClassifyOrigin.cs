using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    /// <summary>
    /// 归类锁定视图
    /// </summary>
    internal class Locks_ClassifyOrigin : UniqueView<Models.Lock_Classify, ScCustomsReponsitory>
    {
        internal Locks_ClassifyOrigin()
        {
        }

        internal Locks_ClassifyOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Lock_Classify> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Locks_ClassifySynonymTopView>()
                   select new Models.Lock_Classify
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       LockDate = entity.LockDate,
                       LockerID = entity.LockerID,
                       Summary = entity.Summary
                   };
        }
    }
}
