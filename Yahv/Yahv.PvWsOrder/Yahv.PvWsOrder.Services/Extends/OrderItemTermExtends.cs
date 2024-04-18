using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Extends
{
    public static class OrderItemTermExtends
    {
        public static string GetSpecialType(this Models.OrderItemsTerm term)
        {
            StringBuilder specialType = new StringBuilder();

            if (term.Ccc)
                specialType.Append("3C|");
            if (term.Embargo)
                specialType.Append("禁运|");
            if (term.HkControl)
                specialType.Append("香港管制|");
            if (term.Coo)
                specialType.Append("原产地证明|");
            if (term.CIQ)
                specialType.Append("商检|");
            if (term.IsHighPrice)
                specialType.Append("高价值|");
            if (term.IsDisinfected)
                specialType.Append("检疫|");

            if (specialType.Length == 0)
                return "--";
            else
                return specialType.ToString().TrimEnd('|');
        }
    }
}
