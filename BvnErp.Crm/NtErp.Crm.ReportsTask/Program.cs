using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtErp.Crm.ReportsTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("程序启动 ... \r\n");

            using (var view = new ReportsView())
            {
                view.StatisticByMonth();
            }

            Console.WriteLine("程序将在5s后退出 ... \r\n");
            Thread.Sleep(5000);

            Environment.Exit(0);
        }
    }
}
