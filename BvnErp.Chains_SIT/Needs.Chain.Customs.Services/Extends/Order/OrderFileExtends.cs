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
    public static class OrderFileExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderFiles ToLinq(this Models.OrderFile entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderFiles
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                OrderPremiumID = entity.OrderPremiumID,
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
