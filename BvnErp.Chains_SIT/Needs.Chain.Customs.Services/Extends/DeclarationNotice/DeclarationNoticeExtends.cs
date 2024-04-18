using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关通知扩展方法
    /// </summary>
    public static class DeclarationNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DeclarationNotices ToLinq(this Models.DeclarationNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DeclarationNotices
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                AdminID = entity.Admin?.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
