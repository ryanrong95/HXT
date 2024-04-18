using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.IcgooData.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Models.Icgoo_Eccns().Import();
            new Models.Icgoo_ProductLimits().Import();
            new Models.Icgoo_ProductTaxes().Import();

            Console.ReadKey();
        }
    }
}
