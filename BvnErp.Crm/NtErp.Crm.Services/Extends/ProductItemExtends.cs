using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Extends
{
    public static class ProductItemExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //static internal Layer.Data.Sqls.BvCrm.ProductItems ToLinq(this ProductItem entity)
        //{
        //    return new Layer.Data.Sqls.BvCrm.ProductItems
        //    {
        //        ID = entity.ID,
        //        StandardID = entity.standardProduct.ID,
        //        CompeteID = string.IsNullOrWhiteSpace(entity.CompeteProduct.ID) ? null : entity.CompeteProduct.ID,
        //        RefQuantity = entity.RefQuantity,
        //        RefUnitQuantity = entity.RefUnitQuantity,
        //        RefUnitPrice = entity.RefUnitPrice,
        //        ExpectRate = entity.ExpectRate,
        //        UnitPrice = entity.UnitPrice,
        //        Quantity = entity.Quantity,
        //        Status = (int)entity.Status,
        //        Count = entity.Count,
        //        CreateDate = entity.CreateDate,
        //        UpdateDate = entity.UpdateDate,
        //        ExpectDate = entity.ExpectDate,
        //        OriginNumber = entity.OriginNumber,
        //        PMAdmin = entity.PMAdminID,
        //        FAEAdmin = entity.FAEAdminID,
        //    };
        //}
    }
}
