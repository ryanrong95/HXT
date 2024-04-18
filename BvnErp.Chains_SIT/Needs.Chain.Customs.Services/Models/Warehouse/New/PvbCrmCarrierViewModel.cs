using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public class PvbCrmCarrierViewModel:IUnique
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Place { get; set; }
        public PvbCarrierType Type { get; set; }
        public bool IsInternational { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public enum PvbCarrierType
    {

        /// <summary>
        /// 物流
        /// </summary>
        [Description("物流")]
        Logistics = 1,
        /// <summary>
        /// 快递
        /// </summary>
        [Description("快递")]
        Express = 2,

        /// <summary>
        /// 物流快递
        /// </summary>
        [Description("物流快递")]
        Both = 3
    }
}
