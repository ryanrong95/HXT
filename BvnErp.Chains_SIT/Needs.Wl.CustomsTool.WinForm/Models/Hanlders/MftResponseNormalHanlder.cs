using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Hanlders
{
    /// <summary>
    /// 舱单正常回执委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MftResponseNormalHanlder(object sender, MftResponseNormalEventArgs e);

    /// <summary>
    /// 舱单正常回执事件参数
    /// </summary>
    public class MftResponseNormalEventArgs : EventArgs
    {
        public Messages.Manifest Manifest { get; private set; }

        public MftResponseNormalEventArgs(Messages.Manifest entity)
        {
            this.Manifest = entity;
        }

        public MftResponseNormalEventArgs() { }
    }

    /// <summary>
    /// 舱单拒绝回执委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MftResponseRefusedHanlder(object sender, MftResponseRefusedEventArgs e);

    /// <summary>
    /// 舱单拒绝回执事件参数
    /// </summary>
    public class MftResponseRefusedEventArgs : EventArgs
    {
        public Messages.Manifest Manifest { get; private set; }

        public MftResponseRefusedEventArgs(Messages.Manifest entity)
        {
            this.Manifest = entity;
        }

        public MftResponseRefusedEventArgs() { }
    }

    /// <summary>
    /// 舱单成功回执委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MftResponseSucceedHanlder(object sender, MftResponseSucceedEventArgs e);

    /// <summary>
    /// 舱单成功回执事件参数
    /// </summary>
    public class MftResponseSucceedEventArgs : EventArgs
    {
        public Messages.Manifest Manifest { get; private set; }

        public MftResponseSucceedEventArgs(Messages.Manifest entity)
        {
            this.Manifest = entity;
        }

        public MftResponseSucceedEventArgs() { }

    }

    /// <summary>
    /// 舱单传输委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MftResponseTransHanlder(object sender, MftResponseTransEventArgs e);

    /// <summary>
    /// 舱单传输事件参数
    /// </summary>
    public class MftResponseTransEventArgs : EventArgs
    {
        public Messages.Manifest Manifest { get; private set; }

        public MftResponseTransEventArgs(Messages.Manifest entity)
        {
            this.Manifest = entity;
        }

        public MftResponseTransEventArgs() { }

    }

    /// <summary>
    /// 舱单报文错误委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MftResponseErrorHanlder(object sender, MftResponseErrorEventArgs e);

    /// <summary>
    /// 舱单报文错误事件参数
    /// </summary>
    public class MftResponseErrorEventArgs : EventArgs
    {
        public Messages.Manifest Manifest { get; private set; }

        public MftResponseErrorEventArgs(Messages.Manifest entity)
        {
            this.Manifest = entity;
        }

        public MftResponseErrorEventArgs() { }

    }
}
