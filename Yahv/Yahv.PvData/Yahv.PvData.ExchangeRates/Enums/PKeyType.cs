using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.ExchangeRates
{
    /// <summary>
    /// 用于生成主键
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 中国银行外汇牌价
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("Feroboc", PKeySigner.Mode.Date, 8)]
        Feroboc = 30001,

        /// <summary>
        /// 抓取的中国银行外汇牌价页面记录
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("FerobocLog", PKeySigner.Mode.Date, 8)]
        FerobocLog = 30002,

        /// <summary>
        /// 创新恒远汇率日志
        /// </summary>
        [Repository(typeof(ScCustomReponsitory))]
        [PKey("ERLog", PKeySigner.Mode.Date, 6)]
        ExchangeRateLog_HY = 30003,

        /// <summary>
        /// 芯达通汇率日志
        /// </summary>
        [Repository(typeof(foricScCustomsReponsitory))]
        [PKey("ERLog", PKeySigner.Mode.Date, 6)]
        ExchangeRateLog_XDT = 30004,
    }
}
