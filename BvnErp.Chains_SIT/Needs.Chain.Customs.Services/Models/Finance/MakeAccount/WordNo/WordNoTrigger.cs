using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class WordNoTrigger
    {
        public void Credential()
        {
            IWordNo accWordNo = new AccWordNo();
            IWordNo buyWordNo = new BuyWordNo();
            IWordNo declareOutServiceWordNo = new DeclareOutServiceWordNo();
            IWordNo declareWordNo = new DeclareWordNo();
            IWordNo financePWordNo = new FinancePWordNo();

            IWordNo fundTransWordNo = new FundTransWordNo();
            IWordNo goodsWordNo = new GoodsWordNo();
            IWordNo invoiceWordNo = new InvoiceWordNo();
            IWordNo invoiceWordNoFull = new InvoiceWordNoFull();
            IWordNo poundageWordNo = new PoundageWordNo();

            IWordNo qRWordNo = new QRWordNo();
            IWordNo receivingWordNo = new ReceivingWordNo();
            IWordNo receivingWordNoFull = new ReceivingWordNoFull();
            IWordNo swapWordNo = new SwapWordNo();

            accWordNo.GetWordNo();
            buyWordNo.GetWordNo();
            declareOutServiceWordNo.GetWordNo();
            declareWordNo.GetWordNo();
            financePWordNo.GetWordNo();

            fundTransWordNo.GetWordNo();
            goodsWordNo.GetWordNo();
            invoiceWordNo.GetWordNo();
            invoiceWordNoFull.GetWordNo();
            poundageWordNo.GetWordNo();

            qRWordNo.GetWordNo();
            receivingWordNo.GetWordNo();
            receivingWordNoFull.GetWordNo();
            swapWordNo.GetWordNo();
        }
    }
}
