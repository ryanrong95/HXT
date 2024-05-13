using Kdn.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdn.Library
{

    public delegate void RequestingEventHandler(object sender, RequestingEventArgs e);

    /// <summary>
    /// 快递鸟请求
    /// </summary>
    public class RequestingEventArgs : EventArgs
    {
        public KdnRequest Request { get; internal set; }
    }

    public delegate void ResponsedEventHandler(object sender, ResponsedEventArgs e);

    /// <summary>
    /// 快递鸟相应结果
    /// </summary>
    public class ResponsedEventArgs : EventArgs
    {
        public KdnRequest Request { get; internal set; }

        public KdnResult Result { get; internal set; }
    }
}
