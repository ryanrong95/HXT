using System;

namespace Yahv.Usually
{
    /// <summary>
    /// 成功录入事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">成功事件参数</param>
    public delegate void SuccessHanlder(object sender, SuccessEventArgs e);

    /// <summary>
    /// 成功录入事件参数
    /// </summary>
    public class SuccessEventArgs : EventArgs
    {
        /// <summary>
        /// 返回对象
        /// </summary>
        public dynamic Object { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="entity">返回对象</param>
        public SuccessEventArgs(object entity)
        {
            this.Object = entity;
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public SuccessEventArgs() { }
    }

    /// <summary>
    /// 成功录入事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">成功事件参数</param>
    public delegate void AbandonHanlder(object sender, AbandonedEventArgs e);

    /// <summary>
    /// 成功录入事件参数
    /// </summary>
    public class AbandonedEventArgs : EventArgs
    {
        /// <summary>
        /// 返回对象
        /// </summary>
        public dynamic Object { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public AbandonedEventArgs()
        {
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="entity">返回对象</param>
        public AbandonedEventArgs(object entity)
        {
            this.Object = entity;
        }
    }

}
