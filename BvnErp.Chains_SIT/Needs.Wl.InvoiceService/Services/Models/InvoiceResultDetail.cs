using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Models
{
    public class InvoiceResultDetails
    {
        public List<InvoiceResultDetail> Details { get; set; } = new List<InvoiceResultDetail>();

        public void BatchInsertNew()    
        {
            var layerDetails = this.Details.Select(item => new Layer.Data.Sqls.ScCustoms.InvoiceResultDetails
            {
                ID = item.ID,
                InvoiceResultID = item.InvoiceResultID,
                lineNum = item.lineNum,
                goodserviceName = item.goodserviceName,
                model = item.model,
                unit = item.unit,
                number = item.number,
                price = item.price,
                sum = item.sum,
                taxRate = item.taxRate,
                tax = item.tax,
                isBillLine = item.isBillLine,
                zeroTaxRateSign = item.zeroTaxRateSign,
                zeroTaxRateSignName = item.zeroTaxRateSignName,
                Status = (int)item.Status,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                Summary = item.Summary,
            }).ToArray();

            using (var reponsitory = new ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.InvoiceResultDetails>(layerDetails);
            }
        }
    }

    public class InvoiceResultDetail
    {
        public string ID { get; set; }

        public string InvoiceResultID { get; set; }

        public int? lineNum { get; set; }

        public string goodserviceName { get; set; }

        public string model { get; set; }

        public string unit { get; set; }

        public decimal? number { get; set; }

        public decimal? price { get; set; }

        public decimal? sum { get; set; }

        public string taxRate { get; set; }

        public decimal? tax { get; set; }

        public string isBillLine { get; set; }

        public string zeroTaxRateSign { get; set; }

        public string zeroTaxRateSignName { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public void InsertNew()
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.InvoiceResultDetails>(new Layer.Data.Sqls.ScCustoms.InvoiceResultDetails
                {
                    ID = this.ID,
                    InvoiceResultID = this.InvoiceResultID,
                    lineNum = this.lineNum,
                    goodserviceName = this.goodserviceName,
                    model = this.model,
                    unit = this.unit,
                    number = this.number,
                    price = this.price,
                    sum = this.sum,
                    taxRate = this.taxRate,
                    tax = this.tax,
                    isBillLine = this.isBillLine,
                    zeroTaxRateSign = this.zeroTaxRateSign,
                    zeroTaxRateSignName = this.zeroTaxRateSignName,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,
                });
            }
        }
    }
}
