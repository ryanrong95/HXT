using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DutiablePricePostBack : IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public bool PostStatus { get; set; }
        public string Msg { get; set; }
        public int Type { get; set; }
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(this.ToLinq());
            }
        }
    }
}
