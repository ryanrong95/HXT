using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services
{
    /// <summary>
    /// 记账科目
    /// </summary>
    public enum Subjects
    {
        [Description("停车费")]
        ParkingFee = 100,
        [Description("登记费")]
        RegistrationFee = 101,
        [Description("特殊手续费")]
        SpecialHandlingFee = 102,
        [Description("分拣费")]
        SortingFee = 103,
        [Description("贴签费")]
        TicketingFee = 104,
        [Description("包装费")]
        PackingFee = 105,
        [Description("代付货款")]
        AgentPayFee = 106,
        [Description("货款")]
        GoodsFee = 107,
        [Description("材料费")]
        Material = 108,
        [Description("纸箱费")]
        Box = 109
    }

    /// <summary>
    /// 业务类型
    /// </summary>
    public enum BusinessTypes
    {
        [Description("代仓储")]
        AgentStorages = 100,
        [Description("代报关")]
        AgentCustoms = 101,
        [Description("贸易")]
        Trade = 102,
        [Description("代检测")]
        AgentTesting = 103,
        [Description("代收货")]
        AgentReceiving = 104

    }
}
