using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
   public  enum LimitType
    {
        /// <summary>
        /// 授信附件
        /// </summary>
        [Description("不限制")]
        NoLimit = 1,
        /// <summary>
        /// 不可用
        /// </summary>
        [Description("不可用")]
        Disabled = 2,
        /// </summary>
        [Description("可用")]
        Usable = 3,


    }
}
