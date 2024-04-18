using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class jumuReport : IUnique
    {
        public string ID { get; set; }
        public string ReportName { get; set; }
        public string ReportUrl { get; set; }
        public string RoleID { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        public string AdminID { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public jumuReport()
        {
            this.Status = Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }



        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.JimuReport
                {
                    ID = this.ID,
                    ReportName = this.ReportName,
                    ReportUrl = this.ReportUrl,
                    RoleID = this.RoleID,
                    Status = (int)Status.Normal,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
