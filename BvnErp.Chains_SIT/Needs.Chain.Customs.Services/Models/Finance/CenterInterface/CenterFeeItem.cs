using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterFeeItem
    {
        /// <summary>
        /// 费用类型
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 费用描述
        /// </summary>
        public string FeeDesc { get; set; }
    }
}
