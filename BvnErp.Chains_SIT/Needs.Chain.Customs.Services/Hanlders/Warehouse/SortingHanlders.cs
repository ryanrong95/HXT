using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产品分拣完成
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void EntryNoticeItemSortedEventHanlder(object sender, EntryNoticeItemSortedEventArgs e);

    /// <summary>
    /// 事件参数
    /// </summary>
    public class EntryNoticeItemSortedEventArgs : EventArgs
    {
        public Models.EntryNoticeItem EntryNoticeItem { get; private set; }

        public EntryNoticeItemSortedEventArgs(Models.EntryNoticeItem item)
        {
            this.EntryNoticeItem = item;
        }

        public EntryNoticeItemSortedEventArgs() { }
    }
}