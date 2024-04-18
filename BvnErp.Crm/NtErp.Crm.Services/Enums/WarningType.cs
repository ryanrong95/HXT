using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum WarningType
    {
        [Description("销售机会跟踪记录点评")]
        ProjectComment = 20,

        [Description("客户跟踪记录点评")]
        CommentTrace = 30,

        [Description("工作周报点评")]
        CommentWorksWeekly = 40,

        [Description("工作计划点评")]
        CommentWorksOther = 50,

        [Description("客户跟踪记录提醒")]
        ClientReportWarning = 60,

        [Description("销售机会跟踪记录提醒")]
        ProjectReportWarning = 70,

        [Description("即将进入公海的客户提醒")]
        ClientToPublic = 80,

        [Description("客户跟踪记录指定阅读人提醒")]
        ClientReportReadWarning = 90,

        [Description("客户跟踪记录点评指定阅读人提醒")]
        CommentTraceReadWarning = 100,
    }
}
