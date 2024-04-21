using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Extends
{
    public static class ContactExtend
    {
        /// <summary>
        /// 停用
        /// </summary>
        static public void Closed(this IEnumerable<Models.Origins.Contact> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var entity in ienumers)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contacts>(new
                    {
                        Status = Status.Closed
                    }, item => item.ID == entity.ID);
                }
                
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        static public void Deleted(this IEnumerable<Models.Origins.Contact> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var entity in ienumers)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contacts>(new
                    {
                        Status = Status.Deleted
                    }, item => item.ID == entity.ID);
                }
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        static public void Enable(this IEnumerable<Models.Origins.Contact> ienumers)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                foreach (var entity in ienumers)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Contacts>(new
                    {
                        Status = Status.Normal
                    }, item => item.ID == entity.ID);
                }
            }
        }
    }

}
