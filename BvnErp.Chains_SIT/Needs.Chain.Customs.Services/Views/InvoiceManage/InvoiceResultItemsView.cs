using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceResultItemsView : UniqueView<Needs.Ccs.Services.Models.InvoiceDetailData, ScCustomsReponsitory>
    {
        public InvoiceResultItemsView()
        {
        }

        public InvoiceResultItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Ccs.Services.Models.InvoiceDetailData> GetIQueryable()
        {           
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceResultDetails>()
                         where c.Status ==  (int)Enums.Status.Normal
                         select new Needs.Ccs.Services.Models.InvoiceDetailData
                         {
                             ID = c.ID,
                             InvoiceResultID = c.InvoiceResultID,
                             lineNum = c.lineNum,
                             goodserviceName = c.goodserviceName,
                             model = c.model,
                             unit = c.unit,
                             number = c.number,
                             price = c.price,
                             sum = c.sum,
                             taxRate = c.taxRate,                            
                             tax = c.tax,
                             isBillLine = c.isBillLine,
                             zeroTaxRateSign = c.zeroTaxRateSign,
                             zeroTaxRateSignName = c.zeroTaxRateSignName,
                             Status = (Enums.Status)c.Status,
                             CreateDate = c.CreateDate,
                             UpdateDate = c.UpdateDate
                         };

            return result;

        }
    }
}
