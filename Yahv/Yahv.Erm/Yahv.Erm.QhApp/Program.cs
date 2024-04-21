using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services;

namespace Yahv.Erm.QhApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //启动报价助手
            var qh = new QuoteHelper();
            qh.Run();

            Console.ReadKey();
        }
    }
}
