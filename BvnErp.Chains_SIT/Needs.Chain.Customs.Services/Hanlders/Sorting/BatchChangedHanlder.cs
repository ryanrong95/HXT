using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 批次变更
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void BatchChangedHanlder(object sender, BatchChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class BatchChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public string Batch { get; private set; }

        public BatchChangedEventArgs(object entity, string batch)
        {
            this.Object = entity;
            this.Batch = batch;
        }

        public BatchChangedEventArgs() { }
    }
}
