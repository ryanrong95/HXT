using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 增加审批检查未审批事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AttachApprovalCheckUnApprovedHanlder(object sender, AttachApprovalCheckUnApprovedEventArgs e);

    /// <summary>
    /// 增加审批检查未审批事件参数
    /// </summary>
    public class AttachApprovalCheckUnApprovedEventArgs : EventArgs
    {
        public AttachApproval AttachApproval { get; set; }

        public AttachApprovalCheckUnApprovedEventArgs(AttachApproval attachApproval)
        {
            this.AttachApproval = attachApproval;
        }
    }
}
