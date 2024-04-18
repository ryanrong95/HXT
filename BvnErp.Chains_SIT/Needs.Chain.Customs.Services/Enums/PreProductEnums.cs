using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 预归类产品类型
    /// </summary>
    public enum PreProductUserType
    {
        [Description("预归类")]
        Pre = 1,

        [Description("咨询")]
        Consult = 2
    }

    /// <summary>
    /// 预归类产品管控审批状态
    /// </summary>
    public enum PreProductControlStatus
    {
        [Description("待审批")]
        Waiting = 100,

        [Description("通过")]
        Approved = 200,

        [Description("否决")]
        Vetoed = 300,
    }
}
