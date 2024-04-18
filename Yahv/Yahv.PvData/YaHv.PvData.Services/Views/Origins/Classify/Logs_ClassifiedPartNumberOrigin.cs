using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Origins
{
    /// <summary>
    /// 归类历史记录
    /// </summary>
    internal class Logs_ClassifiedPartNumberOrigin : UniqueView<Models.Log_ClassifiedPartNumber, PvDataReponsitory>
    {
        internal Logs_ClassifiedPartNumberOrigin()
        {
        }

        internal Logs_ClassifiedPartNumberOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Log_ClassifiedPartNumber> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Logs_ClassifiedPartNumber>()
                   select new Models.Log_ClassifiedPartNumber
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       VATRate = entity.VATRate,
                       ImportPreferentialTaxRate = entity.ImportPreferentialTaxRate,
                       OriginATRate = entity.OriginATRate,
                       ExciseTaxRate = entity.ExciseTaxRate,
                       Elements = entity.Elements,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       Currency = entity.Currency,
                       UnitPrice = entity.UnitPrice,
                       Quantity = entity.Quantity,
                       CIQ = entity.CIQ,
                       CIQprice = entity.CIQprice,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
