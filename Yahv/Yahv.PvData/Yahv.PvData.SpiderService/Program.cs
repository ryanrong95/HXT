using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.SpiderService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            /*
            while (true)
            {
                Plat.Current.Feroboc.Crawling();
                System.Threading.Thread.Sleep(10000);
            }
            */

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SpiderService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
