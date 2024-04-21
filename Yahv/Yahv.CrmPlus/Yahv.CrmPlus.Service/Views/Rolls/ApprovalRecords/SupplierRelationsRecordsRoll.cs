using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Rolls;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class SupplierRelationsRecordsRoll : Yahv.Linq.UniqueView<BusinessRelationsRecord, PvdCrmReponsitory>
    {
        public SupplierRelationsRecordsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SupplierRelationsRecordsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<BusinessRelationsRecord> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from tasks in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()

                   join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>() on tasks.MainID equals maps.ID

                   join mainenterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   on maps.MainID equals mainenterprise.ID

                   join subenterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   on maps.SubID equals subenterprise.ID
                   //申请人
                   join applier in adminsView on tasks.ApplierID equals applier.ID into _applyAdmin
                   from applyAdmin in _applyAdmin.DefaultIfEmpty()
                       //审批人
                   join approve in adminsView on tasks.ApproverID equals approve.ID into _approveAdmin
                   from approveAdmin in _approveAdmin.DefaultIfEmpty()

                   where tasks.ApplyTaskType == (int)Underly.ApplyTaskType.SupplierBusinessRelation &&tasks.Status!= (int)Underly.ApplyStatus.Waiting
                   select new BusinessRelationsRecord
                   {
                       ID = tasks.ID,
                       MainID = tasks.MainID,
                       MainEnterpriseName = mainenterprise.Name,
                       Status = (Underly.ApplyStatus)tasks.Status,
                       SubEnterpriseName = subenterprise.Name,
                       ApplyDate = tasks.CreateDate,
                       ApproveDate = tasks.ApproveDate,
                       TaskType = (Underly.ApplyTaskType)tasks.ApplyTaskType,
                       ApplyAdmin = applyAdmin,
                       ApproveAdmin = approveAdmin,
                       RelationType=(Underly.BusinessRelationType)maps.Type
                   };
        }
    }
}
