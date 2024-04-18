using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.SpiderService.Handlers
{
    /// <summary>
    /// 数据抓取后调用
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void CrawledHandler(object sender, CrawledEventArgs e);

    /// <summary>
    /// 数据抓取事件参数
    /// </summary>
    public class CrawledEventArgs : EventArgs
    {
        public string Html { get; private set; }

        public CrawledEventArgs()
        {
        }

        public CrawledEventArgs(string html)
        {
            this.Html = html;
        }
    }
}
