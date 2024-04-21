using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Extends
{
    public static class BeneficiaryExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.Beneficiary entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
                {
                    Status = entity.Status
                }, item => item.ID == entity.ID);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        static public void Delete(this IEnumerable<Models.Origins.Beneficiary> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        static public void Blacked(this IEnumerable<Models.Origins.Beneficiary> ienumers)
        {
            var arry = ienumers.Select(item => item.ID);
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
                {
                    Status = ApprovalStatus.Black
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Unable(this IEnumerable<Models.Origins.Beneficiary> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
                {
                    Status = ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.Beneficiary> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
                {
                    Status = ApprovalStatus.Normal
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
