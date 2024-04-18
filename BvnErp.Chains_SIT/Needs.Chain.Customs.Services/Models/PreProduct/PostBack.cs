using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PostBack : IUnique, IPersist
    {
        public string  ID { get; set; }
        public bool status { get; set; }
        public string msg { get; set; }
        public string id { get; set; }
        public Status RecordStatus { get; set; }
        public DateTime CreateDate { get; set; }
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
