using System;

namespace Yahv.Linq
{
    /// <summary>
    /// Linq数据上下文
    /// </summary>
    sealed public class LinqContext : IDisposable
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        LinqContext()
        {

        }

        /// <summary>
        /// 实现 释放接口
        /// </summary>
        public void Dispose()
        {
            Layers.Linq.LinqRuntime.Current.Dispose();
        }

        /// <summary>
        /// 全局实例
        /// </summary>
        static public LinqContext Current
        {
            get
            {
                return new LinqContext();
            }
        }
    }
}
