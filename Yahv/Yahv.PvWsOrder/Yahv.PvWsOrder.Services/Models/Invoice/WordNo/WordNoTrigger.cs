using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{

    public class WordNoTrigger
    {

        public void Credential()
        {

            IWordNo invoiceWordNo = new InvoiceWordNo();
            invoiceWordNo.GetWordNo();
        }


    }
}
