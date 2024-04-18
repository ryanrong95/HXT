using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceXmlWaybillRequestVo
    {
        public string 开票完成状态 { get; set; }
        public string 发票标识 { get; set; }

        public InvoiceXmlWaybillRequestVo()
        {
            this.开票完成状态 = "开票完成";
            //this.开票完成状态 = "全部";
        }
    }
}
