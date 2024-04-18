using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    public class AdminTop : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string RealName { get; set; }
        public string UserName { get; set; }
       
     

        public bool IsSa
        {
            get
            {
                return this.UserName.Equals("sa", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
