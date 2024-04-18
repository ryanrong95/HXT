using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单扩展方法
    /// </summary>
    public static class DutiablePricePostBackExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.DutiablePricePostLog ToLinq(this Models.DutiablePricePostBack entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DutiablePricePostLog
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                PostStatus = entity.PostStatus,
                Msg = entity.Msg,
                Type = entity.Type,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.CreateDate,
                Summary = entity.Summary,
            };
        }
    }
}
