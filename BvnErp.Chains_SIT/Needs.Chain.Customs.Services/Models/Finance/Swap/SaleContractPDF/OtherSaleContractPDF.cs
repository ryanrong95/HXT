using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OtherSaleContractPDF : BaseSaleContractPDF
    {
        public OtherSaleContractPDF(SwapNotice swap)
        {
            SwapNotice = swap;
        }
        public override string ClearingType()
        {
            return "TT";
        }
    }
}
