using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ProductSupplierMap : IUnique, IPersist
    {
        public string ID { get; set; }
       
        public string SupplierID { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ProductSupplierMap
                {
                    ID = this.ID,
                    SupplierID = this.SupplierID
                });
            }
        }
    }
}
