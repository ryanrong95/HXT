using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExcludeOriginTariffs : IUnique
    {
        public string ID { get; set; }
        public string HSCode { get; set; }
        public string Name { get; set; }
        public string ExclusionPeriod { get; set; }
        public string Origin { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public ExcludeOriginTariffs()
        {
            this.Origin = "USA";
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExcludeOriginTariffs>().Where(t => t.HSCode == this.HSCode && t.ExclusionPeriod == this.ExclusionPeriod).Count();
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExcludeOriginTariffs
                    {
                        ID = this.ID,
                        HSCode = this.HSCode,
                        Name = this.Name,
                        ExclusionPeriod = this.ExclusionPeriod,
                        Origin = this.Origin,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExcludeOriginTariffs>(new { Name = this.Name, UpdateDate = DateTime.Now },
                        t => t.HSCode == this.HSCode && t.ExclusionPeriod == this.ExclusionPeriod);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

    }
}
