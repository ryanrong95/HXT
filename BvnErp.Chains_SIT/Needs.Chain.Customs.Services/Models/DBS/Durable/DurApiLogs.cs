using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DurApiLogs
    {
        public string ID { get; set; }
        public string TransactionName { get; set; }
        public string msgId { get; set; }
        public string Url { get; set; }
        public string RequestContent { get; set; }
        public string ResponseContent { get; set; }
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public DurApiLogs()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.ApiLogs>(new Layer.Data.Sqls.foricDBS.ApiLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    TransactionName = this.TransactionName,
                    msgId = this.msgId,
                    Url = this.Url,
                    RequestContent = this.RequestContent,
                    ResponseContent = this.ResponseContent,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
