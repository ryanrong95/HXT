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
    public static class PostBackExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.PreProductPostLog ToLinq(this Models.PostBack entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PreProductPostLog
            {
                ID = entity.ID,
                PreProductCategoryID = entity.id,
                PostStatus = entity.status,
                Msg = entity.msg,
                Status = (int)entity.RecordStatus,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.CreateDate,
                Summary = entity.Summary,
            };
        }
    }
}
