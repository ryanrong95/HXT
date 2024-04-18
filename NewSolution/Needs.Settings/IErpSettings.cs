using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings
{
    public interface IErpSettings : ISettings
    {
        /// <summary>
        /// 支付宝线上交易最大额度
        /// </summary>
        decimal Llaot { get; }

        /// <summary>
        /// 购物车最大数量
        /// </summary>
        int Tnsc { get; }
    }
}
