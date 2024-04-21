using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.VcCsrm.Service.Models
{
    /// <summary>
    /// 合同
    /// </summary>
    public class Contact : Yahv.Linq.IUnique
    {
        public string ID { set; get; }
    }
}
