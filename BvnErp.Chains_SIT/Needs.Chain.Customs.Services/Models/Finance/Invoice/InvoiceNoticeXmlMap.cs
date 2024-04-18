using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceNoticeXmlMap : IUnique
    {
        public string ID { get; set; }
        public string InvoiceXmlID { get; set; }
        public string DecListID { get; set; }      
        public decimal OutQty { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }

        public InvoiceNoticeXmlMap() 
        {
            this.Status = Status.Normal;
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlMap>(new Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlMap
                    {
                        ID = this.ID,
                        InvoiceXmlID = this.InvoiceXmlID,
                        DecListID = this.DecListID,                      
                        OutQty = this.OutQty,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate
                    });
            }
        }
    }
}
