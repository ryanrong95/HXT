using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class TraceExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm._bak_Traces ToLinq(this Models.Trace entity)
        {
            return new Layer.Data.Sqls.BvCrm._bak_Traces
            {
                ID = entity.ID,
                Type=(int)entity.Type,
                Date=entity.Date,
                Context=entity.Context,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }
    }
}
