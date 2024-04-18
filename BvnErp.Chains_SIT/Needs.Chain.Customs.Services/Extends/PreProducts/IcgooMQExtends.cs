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
    public static class IcgooMQExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.IcgooPostLog ToLinq(this Models.IcgooMQ entity)
        {
            return new Layer.Data.Sqls.ScCustoms.IcgooPostLog
            {
                ID = entity.ID,
                PostData = entity.PostData,
                IsAnalyzed = entity.IsAnalyzed,        
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Status = (int)entity.Status,   
                Summary = entity.Summary,
                IsForklift = entity.IsForklift,
                PlateQty = entity.AdditionWeight,
                CompanyType = (int)entity.CompanyType
            };
        }
    }
}
