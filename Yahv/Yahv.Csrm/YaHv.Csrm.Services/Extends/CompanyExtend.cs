using Layers.Data.Sqls;
using Layers.Linq;
using System.Collections.Generic;
using System.Linq;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Extends
{
    public static class CompanyExtend
    {
        /// <summary>
        /// 删除
        /// </summary>
        static public void Delete(this IEnumerable<Models.Origins.Company> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Companies>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.Company> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Companies>(new
                {
                    Status = ApprovalStatus.Normal
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Unable(this IEnumerable<Models.Origins.Company> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Companies>(new
                {
                    Status = ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
