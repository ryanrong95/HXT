using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 分拣装箱扩展方法
    /// </summary>
    public static class ExpressTypeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExpressTypes ToLinq(this Models.ExpressType entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExpressTypes
            {
                ID = entity.ID,
                ExpressCompanyID = entity.ExpressCompanyID,
                TypeName = entity.TypeName,
                TypeValue = entity.TypeValue,
                Status=(int)entity.Status
            };
        }
    }
}
