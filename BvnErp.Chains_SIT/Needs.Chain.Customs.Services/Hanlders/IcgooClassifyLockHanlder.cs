using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// Icgoo请求完成时发生
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooClassifyLockHanlder(object sender, IcgooClassifyLockEventArgs e);


    /// <summary>
    /// Icgoo请求后的参数
    /// </summary>
    public class IcgooClassifyLockEventArgs : EventArgs
    {
        public ClassifyResult classifyResult { get; private set; }
        public string Operation { get; set; }

        public IcgooClassifyLockEventArgs(ClassifyResult classifyResult,string operation)
        {
            this.classifyResult = classifyResult;
            this.Operation = operation;
        }

        public IcgooClassifyLockEventArgs() { }
    }
}
