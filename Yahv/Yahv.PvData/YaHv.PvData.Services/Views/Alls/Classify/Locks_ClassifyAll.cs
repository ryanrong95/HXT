using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Views;
using Yahv.Underly;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 归类锁定视图
    /// </summary>
    public class Locks_ClassifyAll : UniqueView<Models.Lock_Classify, PvDataReponsitory>
    {
        public Locks_ClassifyAll()
        {
        }

        internal Locks_ClassifyAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Lock_Classify> GetIQueryable()
        {
            var locks_ClassifyView = new Origins.Locks_ClassifyOrigin(this.Reponsitory).Where(cl => cl.Status == GeneralStatus.Normal);
            //TODO: 芯达通与代仓储管理员ID完成数据同步后再做关联
            //var adminsView = new AdminsAll<PvDataReponsitory>(this.Reponsitory).Where(a => a.Status != AdminStatus.Closed);
            return from entity in locks_ClassifyView
                   //join admin in adminsView on entity.LockerID equals admin.ID
                   select new Models.Lock_Classify
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       LockDate = entity.LockDate,
                       LockerID = entity.LockerID,
                       LockerName = entity.LockerName,
                       //Locker = admin,
                       Status = entity.Status,
                       Summary = entity.Summary
                   };
        }
    }
}
