using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Models
{
    public class TaxMapJobLog
    {
        public string ID { get; set; }

        public Enums.TaxMapJobLogType Type { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? SchedulerTime { get; set; }

        public DateTime? NextTime { get; set; }

        public string Exception { get; set; }

        public void InsertNew()
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxMapJobLog>(new Layer.Data.Sqls.ScCustoms.TaxMapJobLog
                {
                    ID = this.ID,
                    Type = (int)this.Type,
                    CreateDate = this.CreateDate,
                    SchedulerTime = this.SchedulerTime,
                    NextTime = this.NextTime,
                    Exception = this.Exception,
                });
            }
        }
    }
}
