using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 证件类型
    /// </summary>
    public enum IDType
    {
        [Description("身份证")]
        IDCard = 1,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 2,
    }
}
