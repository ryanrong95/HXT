using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    #region 供应商注册审批记录
    public class SupplierRegisteRecordsRoll : Yahv.Linq.UniqueView<SupplierRegisteRecord, PvdCrmReponsitory>
    { 
        public SupplierRegisteRecordsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SupplierRegisteRecordsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SupplierRegisteRecord> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from tasks in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                   join supplier in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>() on tasks.MainID equals supplier.ID
                   join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   on supplier.ID equals enterprise.ID

                   //申请人
                   join applier in adminsView on tasks.ApplierID equals applier.ID into _applyAdmin
                   from applyAdmin in _applyAdmin.DefaultIfEmpty()
                   //审批人
                   join approve in adminsView on tasks.ApproverID equals approve.ID into _approveAdmin
                   from approveAdmin in _approveAdmin.DefaultIfEmpty()

                   where tasks.ApplyTaskType == (int)ApplyTaskType.SupplierRegist
                   && tasks.Status != (int)ApplyStatus.Waiting
                   select new SupplierRegisteRecord
                   {
                       ID = tasks.ID,
                       MainID = tasks.MainID,
                       SupplierID = tasks.MainID,
                       Status = (ApplyStatus)tasks.Status,
                       SupplierName = enterprise.Name,
                       ApplyDate = tasks.CreateDate,
                       ApproveDate = tasks.ApproveDate,
                       TaskType = (ApplyTaskType)tasks.ApplyTaskType,
                       ApplyAdmin = applyAdmin,
                       ApproveAdmin = approveAdmin
                   };
        }
    }
    #endregion
}
