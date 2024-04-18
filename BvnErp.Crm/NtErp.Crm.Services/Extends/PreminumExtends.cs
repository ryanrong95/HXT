using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class PreminumExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.Preminums ToLinq(this Preminum entity)
        {
            return new Layer.Data.Sqls.BvCrm.Preminums {
                ID = entity.ID,
                CatalogueID = entity.CatalogueID,
                DeclareID = entity.DeclareID,
                Name = entity.Name,
                Price = entity.Price,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}
