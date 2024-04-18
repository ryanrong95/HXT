using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 报关员角色
    /// </summary>
    public enum DeclarantRole
    {
        [Description("初级报关员")]
        JuniorDeclarant = 1,

        [Description("高级报关员")]
        SeniorDeclarant = 2
    }
}
