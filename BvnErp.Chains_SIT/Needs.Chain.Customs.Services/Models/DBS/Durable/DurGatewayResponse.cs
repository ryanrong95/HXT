using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public class DurGatewayResponse
    {
        public string ID { get; set; }
        public string msgId { get; set; }
        public DateTime timeStamp { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string DBSstatus { get; set; }
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public DurGatewayResponse()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.GatewayResponse>(new Layer.Data.Sqls.foricDBS.GatewayResponse
                {
                    ID = ChainsGuid.NewGuidUp(),                   
                    msgId = this.msgId,
                    code = this.code,
                    description = this.description,
                    DBSstatus = this.DBSstatus,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
