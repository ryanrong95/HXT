using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class Logs_Waybills : IUnique
    {
        public string ID { get; set; }

        public string MainID { get; set; }

        //目前只有执行状态
        public int Type { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatorID { get; set; }

        public bool IsCurrent { get; set; }

        public void Enter()
        {
            using (var res = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                res.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false
                }, item => item.MainID == this.MainID && item.Type == (int)this.Type);
                res.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = this.MainID,
                    CreatorID = this.CreatorID,
                    Type = (int)this.Type,
                    CreateDate = DateTime.Now,
                    Status = this.Status,
                    IsCurrent = true
                });
            }
        }
    }
}
