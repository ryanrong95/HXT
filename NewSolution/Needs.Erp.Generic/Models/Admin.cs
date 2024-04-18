using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Erp.Generic.Models
{
    public class Admin : Needs.Linq.IUnique, IGenericAdmin
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? LoginDate { get; set; }
        public Status Status { get; set; }
        public string Summary { get; set; }

        public string ErmAdminID { get; set; }

        public bool IsSa
        {
            get
            {
                return this.UserName.Equals("sa", StringComparison.OrdinalIgnoreCase);
            }
        }

        internal Admin() { }
    }
}
