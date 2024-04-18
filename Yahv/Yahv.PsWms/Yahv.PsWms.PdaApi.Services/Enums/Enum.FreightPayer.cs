using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    public enum FreightPayer
    {
        /// <summary>
        /// 寄付
        /// </summary>
        [Description("发件人")]
        Sender = 1,

        /// <summary>
        /// 到付
        /// </summary>
        [Description("收件人")]
        Recipient = 2,

        /// <summary>
        /// 需要另外的月结账户
        /// </summary>
        [Description("第三方月结")]
        ThirdParty = 3,

        /// <summary>
        /// 芯达通月结
        /// </summary>
        [Description("月结")]
        Mothly = 4,
    }
}
