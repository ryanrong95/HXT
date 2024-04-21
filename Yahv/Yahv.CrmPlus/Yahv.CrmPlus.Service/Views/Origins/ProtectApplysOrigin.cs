using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 供应商保护申请
    /// </summary>
    public class ProtectApplysOrigin : Linq.UniqueView<ProtectApply, PvdCrmReponsitory>
    {
        internal ProtectApplysOrigin()
        {

        }
        internal ProtectApplysOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ProtectApply> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var clientsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>().Where(x => x.Status == (int)AuditStatus.Normal);
            var enterpriseView = Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>().Where(x=>x.IsDraft==false &&x.Status==(int)AuditStatus.Normal);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                   join admin in adminsView on entity.ApplierID equals admin.ID
                   join clients in clientsView on entity.MainID equals clients.ID
                   join enterprise in enterpriseView  on entity.MainID equals enterprise.ID
                   where entity.ApplyTaskType == (int)Underly.ApplyTaskType.ClientProtected
                   select new ProtectApply
                   {
                       ID = entity.ID,
                       MainType = (Underly.MainType)entity.MainType,
                       MainID = entity.MainID,
                       CreateDate = entity.CreateDate,
                       ApplierID = entity.ApplierID,
                       ApproverID = entity.ApproverID,
                       ApproveDate = entity.ApproveDate,
                       ApplyTaskType = (Underly.ApplyTaskType)entity.ApplyTaskType,
                       Status = (Underly.ApplyStatus)entity.Status,
                       ApplyerName = admin.RealName,
                       Name = enterprise.Name,
                   };
        }
    }
}
