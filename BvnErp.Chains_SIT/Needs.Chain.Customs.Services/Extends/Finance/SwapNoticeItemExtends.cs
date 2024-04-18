using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 换汇明细项
    /// </summary>
    public static class SwapNoticeItemExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.SwapNoticeItems ToLinq(this Models.SwapNoticeItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.SwapNoticeItems
            {
                ID = entity.ID,
                SwapNoticeID = entity.SwapNoticeID,
                DecHeadID = entity.SwapDecHead.ID,
                CreateDate = entity.CreateDate,
                Amount = entity.Amount,
                Status = (int)entity.Status,
            };
        }
    }
}
