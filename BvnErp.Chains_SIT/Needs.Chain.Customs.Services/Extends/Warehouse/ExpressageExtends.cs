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
    public static class ExpressageExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Expressages ToLinq(this Models.Expressage entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Expressages
            {
                ID = entity.ID,
                Contact = entity.Contact,
                Mobile = entity.Mobile,
                Address = entity.Address,
                ExpressCompanyID = entity.ExpressCompany.ID,
                ExpressTypeID = entity.ExpressType.ID,
                PayType = (int)entity.PayType,
                WaybillCode = entity.WaybillCode,
                //HtmlTemplate = entity.HtmlTemplate,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}
