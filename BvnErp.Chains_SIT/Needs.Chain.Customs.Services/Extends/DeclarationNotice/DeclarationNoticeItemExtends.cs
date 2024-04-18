using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关通知项扩展方法
    /// </summary>
    public static class DeclarationNoticeItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems ToLinq(this Models.DeclarationNoticeItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems
            {
                ID = entity.ID,
                DeclarationNoticeID = entity.DeclarationNoticeID,
                SortingID = entity.Sorting.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}