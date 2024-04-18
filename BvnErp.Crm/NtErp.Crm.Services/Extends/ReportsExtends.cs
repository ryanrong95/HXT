using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using Needs.Utils.Serializers;

namespace NtErp.Crm.Services.Extends
{
    public static class ReportsExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.Reports ToLinq(this Models.Report entity)
        {
            return new Layer.Data.Sqls.BvCrm.Reports
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Context = (new
                {
                    entity.Type,
                    entity.Date,
                    entity.NextDate,
                    entity.Content,
                    entity.Plan,
                    entity.NextType,
                    entity.OriginalStaffs,
                }).Json(),
                ActionID = entity.Action?.ID,
                ProjectID=entity.Project?.ID,
                ClientID=entity.Client?.ID,
                Status=(int)entity.Status
            };
        }

    }
}
