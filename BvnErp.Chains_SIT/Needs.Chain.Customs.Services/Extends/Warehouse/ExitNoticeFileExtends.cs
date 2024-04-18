using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库通知扩展方法
    /// </summary>
    public static class ExitNoticeFileExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExitNoticeFiles ToLinq(this Models.ExitNoticeFile entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExitNoticeFiles
            {
                ID = entity.ID,
                ExitNoticeID = entity.ExitNoticeID,
                AdminID = entity.Admin.ID,
                Name = entity.Name,
                URL = entity.URL,
                FileType = (int)entity.FileType,
                FileFormat = entity.FileFormat,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
