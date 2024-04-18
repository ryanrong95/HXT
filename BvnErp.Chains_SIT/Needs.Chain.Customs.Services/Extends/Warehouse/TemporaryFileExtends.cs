using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 暂存文件扩展方法
    /// </summary>
    public static class TemporaryFileExtends
    {
        public static Layer.Data.Sqls.ScCustoms.TemporaryFiles ToLinq(this Models.TemporaryFile entity)
        {
            return new Layer.Data.Sqls.ScCustoms.TemporaryFiles
            {
                ID = entity.ID,
                AdminID = entity.AdminID,
                TemporaryID = entity.TemporaryID,
                Name = entity.Name,
                FileType = (int)Enums.FileType.TemporaryPicture,
                FileFormat = entity.FileFormat,
                URL = entity.URL,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
