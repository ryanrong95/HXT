using NtErp.Wss.Sales.Services.Underly.InRuntimes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 这里值考虑的web 的情况
    /// </summary>
    /// <example>
    /// 设计目标：
    /// 目前的我的想法是尽量避免线程调用因此会有当前（current）
    /// </example>
    public class InRuntime<T> where T : BaseBuilder
    {
        IVisitor Falsifies(IVisitor visitor)
        {
            var nds = Thread.GetData(Thread.GetNamedDataSlot(nameof(InRuntime<T>)));
            if (nds == null)
            {
                this.Falsify(Thread.CurrentThread.CurrentCulture.DateTimeFormat);
                this.Falsify(Thread.CurrentThread.CurrentUICulture.DateTimeFormat);
                Thread.SetData(Thread.GetNamedDataSlot(nameof(InRuntime<T>)), visitor);
            }
            return visitor;
        }

        protected InRuntime()
        {


        }

        void Falsify(DateTimeFormatInfo info)
        {
            FieldInfo field = info.GetType().GetField("m_isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                throw new ArgumentException("请重新找寻修改只读的参数", "m_isReadOnly");
            }
            field.SetValue(info, false);

            info.DateSeparator = "-";
            info.LongTimePattern = "HH:mm:ss";
            info.ShortDatePattern = "yyyy-MM-dd";
        }


        static InRuntime<T> runtime;
        static object lockcurrent = new object();

        /// <summary>
        /// 当前引用
        /// </summary>
        /// <example>这是偷梁换柱</example>
        public static IVisitor Current
        {
            get
            {
                if (runtime == null)
                {
                    lock (lockcurrent)
                    {
                        if (runtime == null)
                        {
                            runtime = new InRuntime<T>();
                        }
                    }
                }

                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    var visitor = HttpContext.Current.Session[nameof(InRuntime<T>)] as IVisitor;

                    if (visitor == null)
                    {
                        HttpContext.Current.Session[nameof(InRuntime<T>)]
                            = visitor
                            = Activator.CreateInstance(typeof(T), HttpContext.Current.Session.SessionID) as IVisitor;
                    }

                    return runtime.Falsifies(visitor);
                }

                //if (false)
                //{
                //}
                return null;
                throw new NotSupportedException("Do not support the current call!");
            }
        }
    }
}
