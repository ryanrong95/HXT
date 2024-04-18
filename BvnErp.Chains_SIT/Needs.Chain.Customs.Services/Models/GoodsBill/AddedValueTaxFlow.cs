using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AddedValueTaxFlow
    {
        public string ID { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime PayData { get; set; }
        /// <summary>
        /// 增值税税费单号
        /// </summary>
        public string AddedValueTaxTaxNumber { get; set; }
        /// <summary>
        /// 增值税税额
        /// </summary>
        public decimal AddedValueTaxAmount { get; set; }
        /// <summary>
        /// 增值税有效税额
        /// </summary>
        public decimal? VaildAddedValueTaxAmount { get; set; }
        /// <summary>
        /// 所属月份
        /// </summary>
        public DateTime BelongMonth { get; set; }
    }
}
