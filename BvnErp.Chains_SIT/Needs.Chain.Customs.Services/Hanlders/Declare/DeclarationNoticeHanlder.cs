using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关通知 委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclarationNoticeHanlder(object sender, DeclarationNoticeEventArgs e);

    /// <summary>
    /// 报关通知 事件参数
    /// </summary>
    public class DeclarationNoticeEventArgs : EventArgs
    {
        public Models.DeclarationNotice DecNotice { get; private set; }

        public DeclarationNoticeEventArgs(Models.DeclarationNotice entity)
        {
            this.DecNotice = entity;
        }

        public DeclarationNoticeEventArgs() { }
    }
}
