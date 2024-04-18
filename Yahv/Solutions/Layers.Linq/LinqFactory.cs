using System;
using System.Data.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Layers.Linq
{
    ///// <summary>
    ///// 工厂支持者
    ///// </summary>
    //class LinqFactoryReponsitory : LinqReponsitory
    //{
    //    internal LinqReponsitory Reponsitory { get; private set; }
    //    internal LinqFactoryReponsitory(bool isAutoSumit, LinqReponsitory reponsitory) : base(isAutoSumit, reponsitory)
    //    {
    //        this.Reponsitory = reponsitory;
    //    }

    //    internal protected override DataContext InitDataContext()
    //    {
    //        return this.Reponsitory.InitDataContext();
    //    }

    //    public override void Dispose()
    //    {
    //        if (this is LinqFactoryReponsitory)
    //        {
    //            var mtid = Thread.CurrentThread.ManagedThreadId;
    //            var frames = new StackTrace().GetFrames();
    //            var code = frames[1].GetMethod().GetHashCode();

    //            var rest = Thread.GetNamedDataSlot($"lrReponsitory_{this.GetType().FullName}_{Thread.CurrentThread.ManagedThreadId}");

    //            //if (!LinqFactory.TryDispose(this))
    //            //{
    //            //    return;
    //            //}

    //            return;
    //        }

    //        base.Dispose();
    //    }

    //}

    /// <summary>
    /// Linq连接工厂对象
    /// </summary>
    public class LinqFactory
    {
        ///// <summary>
        ///// 测试是否可以释放
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>陈翰临时开发后期需要开发为内部判断</remarks>
        //static internal bool TryDispose(object active)
        //{
        //    var frames = new StackTrace().GetFrames();
        //    var code = frames[2].GetMethod().GetHashCode();

        //    var name = Thread.GetNamedDataSlot("lrCreater" + active.GetType().FullName);

        //    object box = Thread.GetData(name);

        //    if (box == null)
        //    {
        //        return true;
        //    }

        //    var unbox = (int)box;

        //    if (code == unbox)
        //    {
        //        var rest = Thread.GetNamedDataSlot("lrReponsitory" + active.GetType().FullName);
        //        Thread.SetData(rest, null);
        //        Thread.SetData(name, null);
        //        return true;
        //    }

        //    return false;
        //}
        /// <summary>
        /// 测试是否可以释放
        /// </summary>
        /// <returns></returns>
        /// <remarks>陈翰临时开发后期需要开发为内部判断</remarks>
        static internal bool TryDispose(object active)
        {
            var mtid = Thread.CurrentThread.ManagedThreadId;
            var frames = new StackTrace().GetFrames();
            //var code = frames[2].GetMethod().GetHashCode();
            var type = active.GetType();
            var rest = Thread.GetNamedDataSlot($"lrReponsitory_{type.FullName}_{mtid}");
            var numt = Thread.GetNamedDataSlot($"lrReponsitory_num_{type.FullName}_{mtid}");
            var reponsitory = Thread.GetData(rest) as LinqReponsitory;
            var onum = Thread.GetData(numt);

            if (onum == null)
            {
                return false;
            }

            var unbox = (int)Thread.GetData(numt);

            if (0 == unbox)
            {
                Thread.SetData(rest, null);
                Thread.SetData(numt, null);
                return true;
            }

            Thread.SetData(numt, unbox - 1);
            return false;
        }
    }

    /// <summary>
    /// Linq连接工厂对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>只支持单线程调用</remarks>
    public class LinqFactory<T> : LinqFactory where T : LinqReponsitory
    {
        //static public T Create()
        //{
        //    T reponsitory;
        //    var name = Thread.GetNamedDataSlot("lrCreater" + typeof(T).FullName);
        //    var rest = Thread.GetNamedDataSlot("lrReponsitory" + typeof(T).FullName);
        //    var box = Thread.GetData(name);

        //    var frames = new StackTrace().GetFrames();
        //    var code = frames[1].GetMethod().GetHashCode();

        //    if (box == null)
        //    {
        //        //System.Console.WriteLine($"factory.create:{frames[1].GetMethod().DeclaringType}");

        //        Thread.SetData(name, code);
        //        reponsitory = Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null,
        //            new object[] { true, true }, Thread.CurrentThread.CurrentCulture) as T;
        //        Thread.SetData(rest, reponsitory);
        //    }
        //    else
        //    {
        //        reponsitory = Thread.GetData(rest) as T;
        //    }

        //    return reponsitory;
        //}

        /// <summary>
        /// 创建长会话实例 的连接对象
        /// </summary>
        /// <param name="isAutoSumit">是否自动提交</param>
        /// <returns>连接对象</returns>
        static public T Create(bool isAutoSumit = true)
        {
            var mtid = Thread.CurrentThread.ManagedThreadId;
            var frames = new StackTrace().GetFrames();
            //var code = frames[1].GetMethod().GetHashCode();
            var type = typeof(T);
            var rest = Thread.GetNamedDataSlot($"lrReponsitory_{type.FullName}_{mtid}");
            var numt = Thread.GetNamedDataSlot($"lrReponsitory_num_{type.FullName}_{mtid}");
            var reponsitory = Thread.GetData(rest) as T;

            if (reponsitory == null)
            {
                reponsitory = Activator.CreateInstance(type, isAutoSumit, true) as T;
                Thread.SetData(rest, reponsitory);
                Thread.SetData(numt, 0);
            }
            else
            {
                int num = (int)Thread.GetData(numt);
                Thread.SetData(numt, num + 1);
            }

            return reponsitory;
        }
    }
}
