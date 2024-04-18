using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class HandOver : IUnique
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string AdminLeave { get; set; }
        public string AdminWork { get; set; }
        public string ApplyID { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public HandOver()
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.Status = Status.Normal;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.HandOverRecord>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.HandOverRecord
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        AdminLeave = this.AdminLeave,
                        AdminWork = this.AdminWork,
                        ApplyID = this.ApplyID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }

                this.OnEnter();
            }
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.HandOverRecord>(new
                {
                    Status = (int)Needs.Ccs.Services.Enums.Status.Delete,
                    UpdateDate = this.UpdateDate
                }, item => item.ID == this.ID);
            }
        }
    }
}
