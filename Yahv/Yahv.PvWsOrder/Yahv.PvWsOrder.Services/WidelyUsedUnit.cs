using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services
{
    public class WidelyUsedUnit
    {
        public static LegalUnit[] Units =
        {
            LegalUnit.千个,
            LegalUnit.千克,
            LegalUnit.克,
            LegalUnit.台,
            LegalUnit.个,
            LegalUnit.只,
            LegalUnit.块,
            LegalUnit.套,
        };

        public static int[] Values
        {
            get { return Units.Select(item => (int)item).ToArray(); }
        }
    }
}
