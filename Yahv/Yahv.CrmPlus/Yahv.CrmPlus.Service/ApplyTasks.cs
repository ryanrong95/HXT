using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service
{
    public class Applies
    {
        public int Count { set; get; }
        public ApplyTaskType TaskType { set; get; }
        public string TaskName
        {
            get
            {
                return this.TaskType.GetDescription();
            }
        }
    }
    /// <summary>
    /// 审批任务
    /// </summary>
    static public class ApplyTasks
    {
        #region 待审批统计 Dictionary
        static public Dictionary<int, int> Waitings()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var linq = from tasks in reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                           where tasks.Status == (int)Underly.AuditStatus.Waiting
                           group tasks by tasks.ApplyTaskType into entity
                           select new
                           {
                               TaskType = entity.Key,
                               Count = entity.Count(item => item.ApplyTaskType == entity.Key)
                           };
                return linq.ToDictionary(type => type.TaskType, count => count.Count);
            }
        }
        #endregion

        #region 待审批统计 Array
        /// <summary>
        /// 
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        static public Applies[] Array(IErpAdmin admin = null)
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (admin == null || admin?.IsSuper == true)
                {
                    var linq = from tasks in reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                               where tasks.Status == (int)Underly.AuditStatus.Waiting
                               group tasks by tasks.ApplyTaskType into groups
                               select new Applies
                               {
                                   TaskType = (ApplyTaskType)groups.Key,
                                   Count = groups.Count()
                               };
                    return linq.ToArray();
                }
                else
                {
                    var linq = from tasks in reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                               where tasks.Status == (int)Underly.AuditStatus.Waiting
                                    && tasks.ApproverID == admin.ID
                               group tasks by new
                               {
                                   tasks.ApplyTaskType,
                                   tasks.ApproverID
                               } into groups
                               select new Applies
                               {
                                   TaskType = (ApplyTaskType)groups.Key.ApplyTaskType,
                                   Count = groups.Count()
                               };
                    return linq.ToArray();
                }

            }
        }
        #endregion

        #region 所有审批任务
        /// <summary>
        /// 所有审批任务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public IQueryable<Models.Origins.ApplyTask> All(ApplyTaskType? type)
        {
            if (type.HasValue)
            {
                return new Views.Rolls.ApplyTasksRoll().Where(item => item.ApplyTaskType == type);
            }
            return new Views.Rolls.ApplyTasksRoll();
        }
        #endregion

        #region  供应商保护申请
        /// <summary>
        /// 供应商保护申请
        /// </summary>
        /// <returns></returns>
        static public IQueryable<Models.Origins.Rolls.SupplierProtectApply> SupplierProtectApplys()
        {
            return new Views.Rolls.SupplierProtectApplysRoll().Where(item => item.Status == ApplyStatus.Waiting);
        }
        #endregion

    }
}
