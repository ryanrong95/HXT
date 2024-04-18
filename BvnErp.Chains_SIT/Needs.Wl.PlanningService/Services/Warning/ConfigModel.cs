using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class ConfigModel
    {
        /// <summary>
        /// Enums.SendNoticeType 的值
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 发送频率 min为单位
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// 发送邮件还是发送短信
        /// 1、发邮件
        /// 2、发信息
        /// </summary>
        public int WarningMethod { get; set; }
    }
}
