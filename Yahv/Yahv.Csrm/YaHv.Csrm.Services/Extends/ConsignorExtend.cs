using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Extends
{
    static public class ConsignorExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.Consignor entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
                {
                    Status = entity.Status
                }, item => item.ID == entity.ID);
            }
        }
        static public void Delete(this IEnumerable<Models.Origins.Consignor> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
                {
                    Status = Status.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        static public void Enable(this IEnumerable<Models.Origins.Consignor> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
                {
                    Status = Status.Normal
                }, item => arry.Contains(item.ID));
            }
        }
        static public void Unable(this IEnumerable<Models.Origins.Consignor> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Consignors>(new
                {
                    Status = Status.Closed
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
