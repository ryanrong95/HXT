using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TaxManageMap : IUnique, IPersistence
    {
        public string ID { get; set; }

        public string TaxManageID { get; set; }

        public string InvoiceNoticeID { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManageMap>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManageMap>(new Layer.Data.Sqls.ScCustoms.TaxManageMap
                    {
                        ID = this.ID,
                        TaxManageID = this.TaxManageID,
                        InvoiceNoticeID = this.InvoiceNoticeID,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManageMap>(new
                    {
                        TaxManageID = this.TaxManageID,
                        InvoiceNoticeID = this.InvoiceNoticeID,
                    }, item => item.ID == this.ID);
                }
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.TaxManageMap>(item => item.ID == this.ID);
            }
        }
    }
}
