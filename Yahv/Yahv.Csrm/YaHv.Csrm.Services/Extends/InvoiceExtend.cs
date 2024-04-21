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
   static public class InvoiceExtend
    {
        /// <summary>
        /// 审批结果：正常，否决
        /// </summary>
        static public void Approve(this Models.Origins.Invoice entity)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                {
                    Status = entity.Status
                }, item => item.ID == entity.ID);
            }
        }
        static public void Delete(this IEnumerable<Models.Origins.Invoice> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                {
                    Status = Status.Deleted
                }, item => arry.Contains(item.ID));
            }
        }
        static public void Enable(this IEnumerable<Models.Origins.Invoice> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                {
                    Status = ApprovalStatus.Normal
                }, item => arry.Contains(item.ID));
            }
        }
        static public void Unable(this IEnumerable<Models.Origins.Invoice> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var arry = ienumers.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                {
                    Status = ApprovalStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
