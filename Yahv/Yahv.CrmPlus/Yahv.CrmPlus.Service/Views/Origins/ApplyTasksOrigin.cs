using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ApplyTasksOrigin : Linq.UniqueView<ApplyTask, PvdCrmReponsitory>
    {
        internal ApplyTasksOrigin()
        {

        }
        internal ApplyTasksOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ApplyTask> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()

                   select new ApplyTask
                   {
                       ID = entity.ID,
                       MainType = (Underly.MainType)entity.MainType,
                       MainID = entity.MainID,
                       CreateDate = entity.CreateDate,
                       ApplierID = entity.ApplierID,
                       ApproverID = entity.ApproverID,
                       ApproveDate = entity.ApproveDate,
                       ApplyTaskType = (Underly.ApplyTaskType)entity.ApplyTaskType,
                       Status = (Underly.ApplyStatus)entity.Status
                   };
        }
    }
}
