using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class WorksWeeklyExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.WorksWeekly ToLinq(this Models.WorksWeekly entity)
        {
            return new Layer.Data.Sqls.BvCrm.WorksWeekly
            {
                ID = entity.ID,
                Context = entity.Context,
                WeekOfYear = entity.WeekOfYear,
                AdminID = entity.Admin.ID,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Status = (int)entity.Status,
            };
        }
    }
}
