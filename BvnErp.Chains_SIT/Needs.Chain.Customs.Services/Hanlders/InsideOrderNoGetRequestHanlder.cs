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
    public delegate void InsideOrderNoGetRequestHanlder(object sender, InsideGetRequestEventArgs e);

    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InsideOrderNoSendSMSHanlder(object sender, InsideGetRequestEventArgs e);

    /// <summary>
    /// Icgoo请求后的参数
    /// </summary>
    public class InsideGetRequestEventArgs : EventArgs
    {

        public string DateNow { get; set; }

        public string Url { get; private set; }

        public string Info { get; private set; }

        public bool IsSuccess { get; private set; }

        public InsideGetRequestEventArgs(string dateNow, string Url,string Info,bool IsSuccess)
        {       
            this.DateNow = dateNow;           
            this.Url = Url;
            this.Info = Info;
            this.IsSuccess = IsSuccess;
        }

        public InsideGetRequestEventArgs() { }
    }
}
