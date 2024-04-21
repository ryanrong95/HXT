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
    public class SupplierProtectedRecordsRoll : Yahv.Linq.UniqueView<ProtectedRecord, PvdCrmReponsitory>
    {
        public SupplierProtectedRecordsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SupplierProtectedRecordsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ProtectedRecord> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from tasks in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                   //供应商
                   join suppliers in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>() on tasks.MainID equals suppliers.ID
                   //供应商企业信息
                   join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   on suppliers.ID equals enterprise.ID
                   
                   //申请人
                   join applier in adminsView on tasks.ApplierID equals applier.ID into _applyAdmin
                   from applyAdmin in _applyAdmin.DefaultIfEmpty()
                   //审批人
                   join approve in adminsView on tasks.ApproverID equals approve.ID into _approveAdmin
                   from approveAdmin in _approveAdmin.DefaultIfEmpty()

                   where tasks.ApplyTaskType == (int)Underly.ApplyTaskType.SupplierProtected 
                   select new ProtectedRecord
                   {
                       ID = tasks.ID,
                       MainID = tasks.MainID,
                       EnterpriseName = enterprise.Name,
                       Status = (Underly.ApplyStatus)tasks.Status,
                       ApplyDate = tasks.CreateDate,
                       ApproveDate = tasks.ApproveDate,
                       TaskType = (Underly.ApplyTaskType)tasks.ApplyTaskType,
                       ApplyAdmin = applyAdmin,
                       ApproveAdmin = approveAdmin
                   };
        }
    }
}
