using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services
{
    public class ConvertHelper
    {
        public static DateTime? ToDateTime(string str)
        {
            DateTime result;
            if (DateTime.TryParse(str, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static int? ToInt32(string str)
        {
            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static decimal? ToDecimal(string str)
        {
            decimal result;
            if (decimal.TryParse(str, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
