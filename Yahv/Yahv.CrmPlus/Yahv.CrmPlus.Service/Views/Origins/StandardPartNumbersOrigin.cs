using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class StandardPartNumbersOrigin : Yahv.Linq.UniqueView<StandardPartNumber, PvdCrmReponsitory>
    {
        internal StandardPartNumbersOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal StandardPartNumbersOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<StandardPartNumber> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>()
                   join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>()
                   on entity.BrandID equals brand.ID
                   select new StandardPartNumber
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       BrandID = entity.BrandID,
                       Brand = brand.Name,
                       ProductName = entity.ProductName,
                       //DateCode = entity.DateCode,
                       PackageCase = entity.PackageCase,
                       Packaging = entity.Packaging,
                       Moq = entity.Moq,
                       Mpq = entity.Mpq,
                       TaxCode = entity.TaxCode,
                       Eccn = entity.Eccn,
                       TariffRate = entity.TariffRate,
                       Ccc = entity.Ccc,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.DataStatus)entity.Status,
                       Summary = entity.Summary,
                       Catalog = entity.Catalog
                   };
        }
    }


}
