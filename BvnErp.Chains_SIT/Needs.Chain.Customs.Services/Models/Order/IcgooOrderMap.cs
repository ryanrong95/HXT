using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooOrderMap : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IcgooOrder { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.CompanyTypeEnums CompanyType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
    }
}
