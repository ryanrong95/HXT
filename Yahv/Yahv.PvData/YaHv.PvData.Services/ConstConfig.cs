using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 常量配置
    /// </summary>
    public class ConstConfig
    {
        /// <summary>
        /// 强制增值(0.2%)
        /// </summary>
        public const decimal FVARate = 0.002M;

        /// <summary>
        /// 芯达通自动归类默认创建人
        /// </summary>
        public const string XdtNpc = "XDTAdmin";
    }
}
