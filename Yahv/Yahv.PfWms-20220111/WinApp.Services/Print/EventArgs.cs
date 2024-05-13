using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services.Print
{

    public delegate void SuccessEventHandler(object sender, SuccessEventArgs e);

    /// <summary>
    /// 新版顺丰/跨越相应结果
    /// </summary>
    public class SuccessEventArgs : EventArgs
    {

        public List<Result> Result { get; internal set; }
        //public Result Result { get; internal set; }
    }
}
