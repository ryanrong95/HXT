using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class WorksOtherExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.WorksOther ToLinq(this Models.WorksOther entity)
        {
            return new Layer.Data.Sqls.BvCrm.WorksOther
            {
                ID = entity.ID,
                Context = entity.Context,
                StartDate = entity.StartDate,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Status = (int)entity.Status
            };
        }
    }
}
