using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Models
{
    public class InvoiceResult
    {
        public string ID { get; set; }

        public string RtnCode { get; set; }

        public string resultCode { get; set; }

        public string invoicefalseCode { get; set; }

        public string invoiceTypeName { get; set; }

        public string invoiceTypeCode { get; set; }

        public DateTime? checkDate { get; set; }

        public int? checkNum { get; set; }

        public string invoiceDataCode { get; set; }

        public string invoiceNumber { get; set; }

        public DateTime? billingTime { get; set; }

        public string purchaserName { get; set; }

        public string taxpayerNumber { get; set; }

        public string taxDiskCode { get; set; }

        public string taxpayerAddressOrId { get; set; }

        public string taxpayerBankAccount { get; set; }

        public string salesName { get; set; }

        public string salesTaxpayerNum { get; set; }

        public string salesTaxpayerAddress { get; set; }

        public string salesTaxpayerBankAccount { get; set; }

        public decimal? totalAmount { get; set; }

        public decimal? totalTaxNum { get; set; }

        public decimal? totalTaxSum { get; set; }

        public string invoiceRemarks { get; set; }

        public string goodsClerk { get; set; }

        public string checkCode { get; set; }

        public string voidMark { get; set; }

        public string isBillMark { get; set; }

        public string tollSign { get; set; }

        public string tollSignName { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string resultMsg { get; set; }

        public string invoiceName { get; set; }

        public string Summary { get; set; }

        public void InsertNew()
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.InvoiceResults>(new Layer.Data.Sqls.ScCustoms.InvoiceResults
                {
                    ID = this.ID,
                    RtnCode = this.RtnCode,
                    resultCode = this.resultCode,
                    invoicefalseCode = this.invoicefalseCode,
                    invoiceTypeName = this.invoiceTypeName,
                    invoiceTypeCode = this.invoiceTypeCode,
                    checkDate = this.checkDate,
                    checkNum = this.checkNum,
                    invoiceDataCode = this.invoiceDataCode,
                    invoiceNumber = this.invoiceNumber,
                    billingTime = this.billingTime,
                    purchaserName = this.purchaserName,
                    taxpayerNumber = this.taxpayerNumber,
                    taxDiskCode = this.taxDiskCode,
                    taxpayerAddressOrId = this.taxpayerAddressOrId,
                    taxpayerBankAccount = this.taxpayerBankAccount,
                    salesName = this.salesName,
                    salesTaxpayerNum = this.salesTaxpayerNum,
                    salesTaxpayerAddress = this.salesTaxpayerAddress,
                    salesTaxpayerBankAccount = this.salesTaxpayerBankAccount,
                    totalAmount = this.totalAmount,
                    totalTaxNum = this.totalTaxNum,
                    totalTaxSum = this.totalTaxSum,
                    invoiceRemarks = this.invoiceRemarks,
                    goodsClerk = this.goodsClerk,
                    checkCode = this.checkCode,
                    voidMark = this.voidMark,
                    isBillMark = this.isBillMark,
                    tollSign = this.tollSign,
                    tollSignName = this.tollSignName,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    resultMsg = this.resultMsg,
                    invoiceName = this.invoiceName,
                    Summary = this.Summary,
                });
            }
        }
    }
}
