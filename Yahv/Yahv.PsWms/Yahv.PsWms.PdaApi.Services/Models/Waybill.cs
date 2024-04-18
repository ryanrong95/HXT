using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    /// <summary>
    /// 运单
    /// </summary>
    public class Waybill : IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 运费币种
        /// </summary>
        public Underly.Currency Currency { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }
    }
}
