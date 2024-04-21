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
    public class SupplierSpecialRecordsRoll : Yahv.Linq.UniqueView<SupplierSpecialRecord, PvdCrmReponsitory>
    {
        public SupplierSpecialRecordsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SupplierSpecialRecordsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SupplierSpecialRecord> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from tasks in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                   join special in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Specials>() on tasks.MainID equals special.ID

                   join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   on special.EnterpriseID equals enterprise.ID

                   //申请人
                   join applier in adminsView on tasks.ApplierID equals applier.ID into _applyAdmin
                   from applyAdmin in _applyAdmin.DefaultIfEmpty()
                  //审批人
                   join approve in adminsView on tasks.ApproverID equals approve.ID into _approveAdmin
                   from approveAdmin in _approveAdmin.DefaultIfEmpty()

                   where tasks.ApplyTaskType == (int)Underly.ApplyTaskType.SupplierSpecials
                   && tasks.Status != (int)Underly.ApplyStatus.Waiting
                   select new SupplierSpecialRecord
                   {
                       ID = tasks.ID,
                       MainID = tasks.MainID,
                       Status = (Underly.ApplyStatus)tasks.Status,
                       EnterpriseName = enterprise.Name,
                       ApplyDate = tasks.CreateDate,
                       ApproveDate = tasks.ApproveDate,
                       TaskType = (Underly.ApplyTaskType)tasks.ApplyTaskType,
                       ApplyAdmin = applyAdmin,
                       ApproveAdmin = approveAdmin,
                       Brand = special.Brand,
                       PartNumber = special.PartNumber,
                       SpecialType = (Underly.nBrandType)special.Type,
                       Summary = special.Summary
                   };
        }
    }
}
