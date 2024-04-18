using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Drawing;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    public class SCBSaleContractPDF : BaseSaleContractPDF
    {
        public SCBSaleContractPDF(SwapNotice swap)
        {
            SwapNotice = swap;
        }
        public override string ClearingType()
        {
            return "货到付款";
        }
    }
}
