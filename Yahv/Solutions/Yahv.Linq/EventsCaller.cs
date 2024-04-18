using System;
using System.Threading;

namespace Yahv.Linq
{
    /// <summary>
    /// 事件调用器
    /// 
    /// </summary>
    /// <remarks>这个只是我临时开发的一个，如果大规模使用必须要重构</remarks>

    [Obsolete("按照新的要求进行开发")]
    public class EventsCaller
    {
        /// <summary>
        /// 获取编码
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>编号</returns>
        static string GetCode<T>()
        {
            var type = typeof(T);
            return $"{Thread.CurrentThread.GetHashCode()}-{type}";
        }

        /// <summary>
        /// 添加线程事件
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="dg">委托对象</param>
        /// <param name="name">自定义命名</param>
        static public void Add<T>(T dg, string name = "")
        {
            var type = typeof(T);
            var code = $"{Thread.CurrentThread.GetHashCode()}-{type}-{name}";
            Thread.SetData(Thread.GetNamedDataSlot(code), dg);
        }

        /// <summary>
        /// 获取线程事件
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <param name="name">自定义命名</param>
        static public T Get<T>(string name = "")
        {
            var type = typeof(T);
            var code = $"{Thread.CurrentThread.GetHashCode()}-{type}-{name}";
            return (T)Thread.GetData(Thread.GetNamedDataSlot(code));
        }

        /// <summary>
        /// 移除线程事件 [理论上用不到]
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        static public void Remove<T>(string name = "")
        {
            var type = typeof(T);
            var code = $"{Thread.CurrentThread.GetHashCode()}-{type}-{name}";
            Thread.SetData(Thread.GetNamedDataSlot(code), null);
        }
    }
}
