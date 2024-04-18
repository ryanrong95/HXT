using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class ProblemExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.Problems ToLinq(this Problem entity)
        {
            return new Layer.Data.Sqls.BvCrm.Problems {
                ID = entity.ID,
                ActionID = entity.ActionID,
                StandardID = entity.StandardID,
                ReportID = entity.ReportID,
                ContactID = entity.ContactID,
                Context = entity.Context,
                Answer = entity.Answer,
                AdminID = entity.AdminID,
                CreateDate=entity.CreateDate,
                UpdateDate=entity.UpdateDate,
                Status=(int)entity.Status
            };
        }
    }
}
