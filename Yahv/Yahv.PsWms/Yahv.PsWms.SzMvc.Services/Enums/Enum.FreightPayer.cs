using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    public enum FreightPayer
    {
        /// <summary>
        /// 发件人, 寄付
        /// </summary>
        [Description("寄付")]
        Sender = 1,

        /// <summary>
        /// 收件人, 到付
        /// </summary>
        [Description("到付")]
        Recipient = 2,

        /// <summary>
        /// 月结, 芯达通月结
        /// </summary>
        [Description("月结")]
        Mothly = 4,

        /// <summary>
        /// 第三方月结, 需要另外的月结账户
        /// </summary>
        [Description("第三方月结")]
        ThirdParty = 3,
    }
}
