using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    public enum IDType
    {
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 0,

        /// <summary>
        /// 身份证
        /// </summary>
        [Description("身份证")]
        IDCard = 1,
    }
}
