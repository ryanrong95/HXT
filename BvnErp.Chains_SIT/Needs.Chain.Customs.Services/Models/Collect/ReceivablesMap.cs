using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ReceivablesMap : IUnique
    {
        public string ID { get; set; }
        public string ReceivableID { get; set; }
        public string OrderID { get; set; }
        public decimal GoodsFee { get; set; }
        public decimal TaxFee { get; set; }
        public decimal AgencyFee { get; set; }
        public decimal IncidentalFee { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public ReceivablesMap()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceivablesMap>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ReceivablesMap
                    {
                        ID = this.ID,
                        ReceivableID = this.ReceivableID,
                        OrderID = this.OrderID,
                        GoodsFee = this.GoodsFee,
                        TaxFee = this.TaxFee,
                        AgencyFee = this.AgencyFee,
                        IncidentalFee = this.IncidentalFee,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
            }
        }
    }
}
