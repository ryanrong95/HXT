using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    public enum FollowWay
    {
        /// <summary>
        /// 接待
        /// </summary>
        [Description("接待")]
        Receive = 1,

        /// <summary>
        /// 拜访
        /// </summary>
        [Description("拜访")]
        Visited = 2,

        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        Tel = 3,

        /// <summary>
        /// 邮件
        /// </summary>
        [Description("邮件")]
        Email = 4

    }
}
