using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Services.Enums
{
    public enum FormStatus
    {
        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Frozen = 100,
        /// <summary>
        /// 真实的
        /// </summary>
        [Description("真实的")]
        Facted = 200,
    }
}
