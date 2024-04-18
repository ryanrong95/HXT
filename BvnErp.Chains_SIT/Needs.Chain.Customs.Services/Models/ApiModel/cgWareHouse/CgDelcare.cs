using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CgDelcare
    {
        public CgWaybill HkExitWaybill { get; set; }

        public List<CgNotice> Notices { get; set; }
    }

    public class CgWaybill : Waybill
    {
        /// <summary>
        /// 运单操作人
        /// </summary>
        public string OperatorID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户企业ID
        /// </summary>
        public string ClientID { get; set; }
    }
}
