using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单附件扩展方法
    /// </summary>
    public static class MainOrderFileExtends
    {
        public static Layer.Data.Sqls.ScCustoms.MainOrderFiles ToLinq(this Models.MainOrderFile entity)
        {
            return new Layer.Data.Sqls.ScCustoms.MainOrderFiles
            {
                ID = entity.ID,            
                MainOrderID = entity.MainOrderID,
                AdminID = entity.Admin?.ID,
                UserID = entity.User?.ID,
                Name = entity.Name,
                FileType = (int)entity.FileType,
                FileFormat = entity.FileFormat,
                Url = entity.Url,
                FileStatus = (int)entity.FileStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
