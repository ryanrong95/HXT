using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceResultView : UniqueView<Needs.Ccs.Services.Models.InvoiceResult, ScCustomsReponsitory>
    {
        public InvoiceResultView()
        {
        }

        public InvoiceResultView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Ccs.Services.Models.InvoiceResult> GetIQueryable()
        {

            var ressult = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceResults>()
                              where c.Status == (int)Enums.Status.Normal 
                          select new Needs.Ccs.Services.Models.InvoiceResult
                          {
                              ID = c.ID,
                              invoiceTypeName = c.invoiceTypeName,
                              invoiceTypeCode = c.invoiceTypeCode,
                              checkDate = c.checkDate,
                              checkNum = c.checkNum,
                              invoiceDataCode = c.invoiceDataCode,
                              invoiceNumber = c.invoiceNumber,
                              billingTime = c.billingTime,
                              purchaserName = c.purchaserName,
                              taxpayerNumber = c.taxpayerNumber,
                              taxDiskCode = c.taxDiskCode,
                              taxpayerAddressOrId = c.taxpayerAddressOrId,
                              taxpayerBankAccount = c.taxpayerBankAccount,
                              salesName = c.salesName,
                              salesTaxpayerNum = c.salesTaxpayerNum,
                              salesTaxpayerAddress = c.salesTaxpayerAddress,
                              salesTaxpayerBankAccount = c.salesTaxpayerBankAccount,
                              totalAmount = c.totalAmount,
                              totalTaxNum = c.totalTaxNum,
                              totalTaxSum = c.totalTaxSum,
                              invoiceRemarks = c.invoiceRemarks,
                              goodsClerk = c.goodsClerk,
                              checkCode = c.checkCode,
                              voidMark = c.voidMark,
                              isBillMark = c.isBillMark,
                              tollSign = c.tollSign,
                              tollSignName = c.tollSignName
                          };

            return ressult;
        }
    }
}
