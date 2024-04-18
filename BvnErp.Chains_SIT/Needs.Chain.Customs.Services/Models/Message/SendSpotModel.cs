using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public class SendSpotModel:IUnique
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public SpotName Type { get; set; }
        public BusinessType SystemID { get; set; }
        public string SendMessage { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public SendSpotModel()
        {
            this.Status = Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SendSpot>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SendSpot>(new Layer.Data.Sqls.ScCustoms.SendSpot
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Type = (int)this.Type,
                        SystemID = (int)this.SystemID,
                        SendMessage = this.SendMessage,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SendSpot>(new
                    {
                        Name = this.Name,
                        Type = (int)this.Type,
                        SystemID = (int)this.SystemID,
                        SendMessage = this.SendMessage,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
