using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
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
        public Models.DecImportResponse DecImportResponse { get; private set; }

        public DecResponseFailedEventArgs(Models.DecImportResponse decImportResponse)
        {
            this.DecImportResponse = decImportResponse;
        }

        public DecResponseFailedEventArgs() { }
    }
}
