using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;

namespace Needs.Wl.InvoiceService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<MyService>();

                //以local system模式运行
                x.RunAsLocalSystem();


                //启动类型设置
                x.StartAutomatically(); //自动
                //x.StartAutomaticallyDelayed(); //自动（延迟启动）
                //x.StartManually(); //手动
                //x.Disabled(); //禁用

                //常规信息
                x.SetDescription("芯达通查验发票信息服务"); //MyService 服务的描述信息
                x.SetDisplayName("Needs.Wl.InvoiceService"); //MyService 服务的显示名称
                x.SetServiceName("Needs.Wl.InvoiceService"); //MyService 服务名称
            });
        }
    }

    public class MyService : ServiceControl
    {
        static IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public MyService()
        {
            string checkInvoiceCron = System.Configuration.ConfigurationManager.AppSettings["checkInvoiceCron"];

            var job = JobBuilder.Create<MyJob>().WithIdentity("checkinvoice", "invoiceservice").Build();
            var trigger = TriggerBuilder.Create().WithCronSchedule(checkInvoiceCron).Build();
            scheduler.ScheduleJob(job, trigger);
        }

        public bool Start(HostControl hostControl)
        {
            scheduler.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            scheduler.Shutdown();
            return true;
        }
    }
}
