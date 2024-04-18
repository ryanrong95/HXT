using Needs.Wl.CustomsTool.WinForm.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Hanlders
{
    /// <summary>
    /// 报关导入响应失败
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DecResponseFailedHanlder(object sender, DecResponseFailedEventArgs e);

    /// <summary>
    /// 报关导入响应失败事件参数
    /// </summary>
    public class DecResponseFailedEventArgs : EventArgs
    {
        public DecImportResponse DecImportResponse { get; private set; }

        public DecResponseFailedEventArgs(DecImportResponse decImportResponse)
        {
            this.DecImportResponse = decImportResponse;
        }

        public DecResponseFailedEventArgs() { }
    }
}
