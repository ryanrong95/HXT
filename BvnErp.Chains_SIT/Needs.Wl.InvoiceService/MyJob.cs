using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService
{
    class MyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Console.WriteLine("schedulertime={0},nexttime={1}", context.ScheduledFireTimeUtc?.LocalDateTime, context.NextFireTimeUtc?.LocalDateTime);

            Services.InvoiceServiceSimple.Execute(context.ScheduledFireTimeUtc?.LocalDateTime, context.NextFireTimeUtc?.LocalDateTime);
        }
    }
}
