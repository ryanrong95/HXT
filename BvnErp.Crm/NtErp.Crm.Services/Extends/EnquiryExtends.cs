using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class EnquiryExtends
    {
        static internal Layer.Data.Sqls.BvCrm.ProductItemEnquiries ToLinq(this Models.Enquiry entity)
        {
            return new Layer.Data.Sqls.BvCrm.ProductItemEnquiries
            {
                ID = entity.ID,
                ReplyPrice = entity.ReplyPrice,
                ReplyDate = entity.ReplyDate,
                RFQ = entity.RFQ,
                OriginModel = entity.OriginModel ?? string.Empty,
                MOQ = entity.MOQ.GetValueOrDefault(),
                MPQ = entity.MPQ,
                Currency = (int)entity.Currency,
                ExchangeRate = entity.ExchangeRate,
                TaxRate = entity.TaxRate,
                Tariff = entity.Tariff,
                OtherRate = entity.OtherRate,
                Cost = entity.Cost,
                Validity = entity.Validity,
                ValidityCount = entity.ValidityCount,
                SalePrice = entity.SalePrice,
                CreateDate = entity.CreateDate ?? DateTime.Now,
                UpdateDate = entity.UpdateDate ?? DateTime.Now,
                Summary = entity.Summary,
                ReportDate = entity.ReportDate,
            };
        }
    }
}
