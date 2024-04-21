using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Extends
{
    public static class CompanyExtend
    {
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.Company> ienumers)
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvdCrm.Companies>(new
                {
                    Status = DataStatus.Normal
                }, item => arry.Contains(item.ID));
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Close(this IEnumerable<Models.Origins.Company> ienumers)
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvdCrm.Companies>(new
                {
                    Status = DataStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }

    }
}
