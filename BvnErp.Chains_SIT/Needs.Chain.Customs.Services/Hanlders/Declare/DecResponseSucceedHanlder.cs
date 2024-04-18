using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关导入响应成功
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DecResponseSucceedHanlder(object sender, DecResponseSucceedEventArgs e);

    /// <summary>
    /// 报关导入响应成功事件参数
    /// </summary>
    public class DecResponseSucceedEventArgs : EventArgs
    {
        public Models.DecImportResponse DecImportResponse { get; private set; }

        public DecResponseSucceedEventArgs(Models.DecImportResponse decImportResponse)
        {
            this.DecImportResponse = decImportResponse;
        }

        public DecResponseSucceedEventArgs() { }
    }
}
