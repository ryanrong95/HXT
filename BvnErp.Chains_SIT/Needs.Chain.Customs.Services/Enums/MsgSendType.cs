using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum MsgSendType
    {
        /// <summary>
        /// 短信
        /// </summary>
        [Description("短信")]
        SMS = 1,

        /// <summary>
        /// 邮件
        /// </summary>
        [Description("邮件")]
        Email = 2,

        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        WeChat = 4,
    }



    public enum SpotName
    {
        /// <summary>
        /// 下单
        /// </summary>
        [Description("下单")]
        DOrdered = 1,

        /// <summary>
        /// 归类报价后
        /// </summary>
        [Description("归类")]
        Classified = 2,

        /// <summary>
        /// 货到香港
        /// </summary>
        [Description("货到香港")]
        ArriveHK = 3,

        /// <summary>
        /// 报关过关
        /// </summary>
        [Description("报关过关")]
        Declare = 4,

        /// <summary>
        /// 深圳出库后
        /// </summary>
        [Description("深圳出库后")]
        DSZSend = 5,

        /// <summary>
        /// 开票后
        /// </summary>
        [Description("开票后")]
        Invoiced = 6,

        /// <summary>
        /// 受理后
        /// </summary>
        [Description("受理后")]
        Taked = 7,

        /// <summary>
        /// 收货后
        /// </summary>
        [Description("收货后")]
        GoodsArrived = 8,

        /// <summary>
        /// 深圳出库后
        /// </summary>
        [Description("深圳出库后")]
        SSZSend = 9,
    }
}
