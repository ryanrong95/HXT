using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Models
{
    class ErpSettings : ISettings, IErpSettings
    {
        /// <summary>
        /// 支付宝线上交易最大额度
        /// </summary>
        [Label("The largest line of Alipay online trading")]
        public decimal Llaot { get; set; }

        /// <summary>
        /// 购物车最大数量
        /// </summary>
        [Label("Total number of shopping carts")]
        public int Tnsc { get; set; }
    }
}
