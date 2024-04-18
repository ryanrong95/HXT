using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 产品管控类型
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        Ccc = 100,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Embargo = 200
    }
}
