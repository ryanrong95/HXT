using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产品封箱完成
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">封箱事件参数</param>
    public delegate void SealedHanlder(object sender, SealedEventArgs e);

    /// <summary>
    /// 事件参数
    /// </summary>
    public class SealedEventArgs : EventArgs
    {
        public string OrderID { get; set; }

        public Admin Admin { get; set; }
        public string SourceOperation { get; set; }

        public SealedEventArgs(string orderID,string sourceOperation="HK")
        {
            this.OrderID = orderID;
            this.SourceOperation = sourceOperation;
        }
    }
}