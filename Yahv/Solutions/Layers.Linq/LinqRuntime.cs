using System;
using System.Collections.Generic;
using System.Collections;

namespace Layers.Linq
{
    /// <summary>
    /// linq数据上下文[相当于连接]
    /// </summary>

    sealed public class LinqRuntime : IDisposable, IEnumerable<IDisposable>
    {
        /// <summary>
        /// 方便实例化 构造器
        /// </summary>
        public LinqRuntime()
        {

        }

        /// <summary>
        /// 实现 释放函数
        /// </summary>
        public void Dispose()
        {
            LinqReleaser.Current.Queue.Dispose();
        }

        /// <summary>
        /// 重写 目标对象的可查询集获取方法
        /// </summary>
        /// <returns>目标对象的可查询集</returns>
        public IEnumerator<IDisposable> GetEnumerator()
        {
            return LinqReleaser.Current.Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 全局实例
        /// </summary>
        static public LinqRuntime Current
        {
            get
            {
                return new LinqRuntime();
            }
        }
    }
}
