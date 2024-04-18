using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 特殊类型信息通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class XbjsTopView<TReponsitory> : UniqueView<Models.XbjInfo, TReponsitory> where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public XbjsTopView()
        {

        }
        public XbjsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.XbjInfo> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.XbjsTopView>()
                   select new Models.XbjInfo
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       LegalUnit1 = entity.LegalUnit1,
                       LegalUnit2 = entity.LegalUnit2,
                       VATRate = entity.VATRate,
                       ImportPreferentialTaxRate = entity.ImportPreferentialTaxRate,
                       ExciseTaxRate = entity.ExciseTaxRate,
                       Elements = entity.Elements,
                       SupervisionRequirements = entity.SupervisionRequirements,
                       CIQC = entity.CIQC,
                       CIQCode = entity.CIQCode,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       CreateDate = entity.CreateDate,
                       OrderDate = entity.OrderDate,

                       Ccc = entity.Ccc,
                       Embargo = entity.Embargo,
                       HkControl = entity.HkControl,
                       Coo = entity.Coo,
                       CIQ = entity.CIQ,
                       CIQprice = entity.CIQprice,
                       Summary = entity.Summary
                   };
        }
    }
}
