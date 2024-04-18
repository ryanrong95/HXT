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
    public static class IcgooMapExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.IcgooOrderMap ToLinq(this Models.IcgooMap entity)
        {
            return new Layer.Data.Sqls.ScCustoms.IcgooOrderMap
            {
                ID = entity.ID,
                IcgooOrder = entity.IcgooOrder,
                OrderID = entity.OrderID,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Status = (int)entity.Status,  
                CompanyType = (int)entity.CompanyType,
                Summary = entity.Summary,               
            };
        }
    }
}
