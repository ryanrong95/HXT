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
    public delegate void IcgooClassifyHanlder(object sender, IcgooClassifyEventArgs e);

    /// <summary>
    /// 向Icgoo提交数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PostIcgooHandler(object sender, IcgooClassifyEventArgs e);

    /// <summary>
    /// Icgoo请求后的参数
    /// </summary>
    public class IcgooClassifyEventArgs : EventArgs
    {
        public ClassifyResult classifyResult { get; private set; }


        public IcgooClassifyEventArgs(ClassifyResult classifyResult)
        {
            this.classifyResult = classifyResult;
        }

        public IcgooClassifyEventArgs() { }
    }
}
