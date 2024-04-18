using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class Waybill
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 运单Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 运费币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }


        /// <summary>
        /// 重量,默认单位Kg
        /// </summary>
        public decimal Weight { get; set; }
    }
}
