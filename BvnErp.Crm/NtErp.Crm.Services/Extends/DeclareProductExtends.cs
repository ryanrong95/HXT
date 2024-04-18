using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class DeclareProductExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.DeclareProducts ToLinq(this DeclareProduct entity)
        {
            return new Layer.Data.Sqls.BvCrm.DeclareProducts
            {
                ID = entity.ID,
                CatalogueID = entity.CatelogueID,
                StandardID = entity.StandardID,
                SupplierID = entity.SupplierID,
                Amount = entity.Amount,
                Currency = (int)entity.Currency,
                UnitPrice = entity.UnitPrice,
                TotalPrice = entity.TotalPrice,
                Expect = entity.Expect,
                ExpectDate = entity.ExpectDate,
                Delivery = entity.Delivery,
                Count = entity.Count,
                Status = (int)entity.Status,
                CompeteManu = entity.CompeteManu,
                CompeteModel = entity.CompeteModel,
                CompetePrice = entity.CompetePrice,
                OriginNumber = entity.OriginNumber,
            };
        }
    }
}
