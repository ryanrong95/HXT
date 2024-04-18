using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品扩展方法
    /// </summary>
    public static class InvoiceWaybillExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.InvoiceWaybills ToLinq(this Models.InvoiceWaybill entity)
        {
            return new Layer.Data.Sqls.ScCustoms.InvoiceWaybills
            {
                ID = entity.ID,
                InvoiceNoticeID = entity.InvoiceNotice.ID,
                CompanyName = entity.CompanyName,
                WaybillCode = entity.WaybillCode,
                CreateDate = entity.CreateDate,
            };
        }
    }
}
