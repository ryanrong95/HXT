using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 换汇通知完成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SwapNoticeCompletedHanlder(object sender, SwapNoticeCompletedEventArgs e);


    /// <summary>
    /// 换汇通知完成事件
    /// </summary>
    public class SwapNoticeCompletedEventArgs : EventArgs
    {
        public SwapNotice SwapNotice { get; private set; }

        public SwapNoticeCompletedEventArgs(SwapNotice notice)
        {
            this.SwapNotice = notice;
        }

        public SwapNoticeCompletedEventArgs() { }
    }
}
