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
    public static class PreProductsExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.PreProducts ToLinq(this Models.IcgooPreProduct entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PreProducts
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                ProductUnionCode = entity.sale_orderline_id,
                Model = entity.partno,
                Manufacturer = entity.mfr,
                Price = entity.price,
                Currency = entity.currency_code,
                Supplier = entity.supplier,             
                CreateDate = entity.CreateTime,
                UpdateDate = entity.UpdateTime,
                Status = entity.Status,  
                CompanyType = (int)entity.CompanyType,
                BatchNo = entity.BatchNo,
                Pack = entity.Pack,
                Description = entity.Description,
                UseFor = entity.UseFor,
                AreaOfProduction = entity.AraeOfProduction,
            };
        }
    }
}
