using Layer.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace Layer.Linq
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
        /// 释放接口
        /// </summary>
        public void Dispose()
        {
            LinqReleaser.Current.Queue.Dispose();
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return LinqReleaser.Current.Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 方便性 linq数据上下文
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
