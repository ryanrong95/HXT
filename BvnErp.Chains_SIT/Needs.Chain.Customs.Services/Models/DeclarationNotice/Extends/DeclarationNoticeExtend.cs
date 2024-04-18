using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 用于拆分申报
    /// </summary>
    public static class DeclarationNoticeExtend
    {
        public static void Trace(this DeclarationNotice decNotice, string message)
        {
            DeclarationNoticeLog trace = new DeclarationNoticeLog();
            trace.DeclarationNoticeID = decNotice.ID;
            trace.Admin = decNotice.Admin;
            trace.CreateDate = DateTime.Now;
            trace.Summary = message;
            trace.Enter();
        }
    }
}
