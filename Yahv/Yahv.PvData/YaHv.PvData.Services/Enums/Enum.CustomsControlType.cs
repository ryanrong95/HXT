using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 海关卡控类型
    /// </summary>
    public enum CustomsControlType 
    {
        [Description("卡控型号")]
        Partnumber = 100,

        [Description("卡控海关编码")]
        HSCode = 200
    }
}
