using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum  FollowUpMethod
    {
        [Description("直接登门")]
        DirectVisit = 10,
        [Description("邀约登门")]
        InvitationVisit = 20,
        [Description("电话联系")]
        Phone = 30,
        [Description("网络聊天交流")]
        WeChat = 40,
        [Description("邮件")]
        Mail = 50,
        [Description("手机短信")]
        PhoneSMS = 60,
        [Description("其他方式")]
        Other = 70

    }
}
