using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class ApplySetsExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.ApplySteps ToLinq(this ApplyStep entity)
        {
            return new Layer.Data.Sqls.BvCrm.ApplySteps
            {
                ApplyID = entity.ApplyID,
                Step = entity.Step,
                AdminID = entity.AdminID,
                Status = (int)entity.Status,
                Comment = entity.Comment,
                Aprdate = entity.AprDate,
            };
        }
    }
}
