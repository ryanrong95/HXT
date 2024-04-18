using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.ClassifyService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

            //var a = new ProductClassify().RecievedClassify;
            //var b = new ProductClassify().TransportClassify;
            //var a = new Yahv.Services.Views.MyClass_456789();
            //new test().test1();
        }
    }
}
