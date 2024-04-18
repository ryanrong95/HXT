using System;
using System.Diagnostics;
using System.Reflection;

namespace Needs.Linq
{
    /// <summary>
    /// linq数据上下文[相当于连接]
    /// </summary>
    sealed public class LinqContext : IDisposable
    {
        /// <summary>
        /// 方便实例化 构造器
        /// </summary>
        public LinqContext()
        {

        }

        /// <summary>
        /// 释放接口
        /// </summary>
        public void Dispose()
        {
            Layer.Linq.LinqRuntime.Current.Dispose();
        }

        /// <summary>
        /// 方便性 linq数据上下文
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
