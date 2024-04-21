using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 供应商保护申请
    /// </summary>
    public class SupplierProtectApplysOrigin : Linq.UniqueView<SupplierProtectApply, PvdCrmReponsitory> 
    {
        internal SupplierProtectApplysOrigin()
        {

        }
        internal SupplierProtectApplysOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SupplierProtectApply> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var suppliersView = new Rolls.SuppliersRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                   join admin in adminsView on entity.ApplierID equals admin.ID
                   join suppliers in suppliersView on entity.MainID equals suppliers.ID
                   where entity.ApplyTaskType == (int)Underly.ApplyTaskType.SupplierProtected
                   select new SupplierProtectApply
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
                       SupplierName = suppliers.Name,
                   };
        }
    }
}
