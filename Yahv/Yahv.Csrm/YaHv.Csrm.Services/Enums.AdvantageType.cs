using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services
{
    public enum AdvantageType
    {
        /// <summary>
        /// 销售
        /// </summary>
        [Description("销售")]
        Sales = 10,
        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        Purchases = 20,
    }
}
