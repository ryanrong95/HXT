using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 提货通知状态
    /// </summary>
    public enum DeliveryNoticeStatus
    {

        [Description("待提货")]
        UnDelivery = 1,

        [Description("提货完成")]
        Delivered = 2,

        [Description("提货中")]
        Taking = 3,
    }
}

