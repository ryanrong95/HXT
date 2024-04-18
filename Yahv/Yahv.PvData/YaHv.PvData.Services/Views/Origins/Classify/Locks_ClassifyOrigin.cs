using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.PvData.Services.Views.Origins
{
    /// <summary>
    /// 归类锁定视图
    /// </summary>
    internal class Locks_ClassifyOrigin : UniqueView<Models.Lock_Classify, PvDataReponsitory>
    {
        internal Locks_ClassifyOrigin()
        {
        }

        internal Locks_ClassifyOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Lock_Classify> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Locks_Classify>()
                   select new Models.Lock_Classify
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       LockDate = entity.LockDate,
                       LockerID = entity.LockerID,
                       LockerName = entity.LockerName,
                       Status = (GeneralStatus)entity.Status,
                       Summary = entity.Summary
                   };
        }
    }
}
