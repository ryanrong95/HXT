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
    public static class IcgooRequestLogExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.IcgooRequestLog ToLinq(this Models.IcgooRequestLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.IcgooRequestLog
            {
               id = entity.ID,
               Supplier = entity.Supplier,
               Days = entity.Days,
               RunPara = entity.RunPara,
               IsSuccess = entity.IsSuccess,
               Info = entity.Info,
               IsSend = entity.IsSend,
               Createtime = entity.Createtime,
               Updatetime = entity.Updatetime,
               CompanyType = (int)entity.CompanyType,
            };
        }
    }
}
