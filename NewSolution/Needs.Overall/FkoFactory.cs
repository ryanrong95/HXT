using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall
{
    [Obsolete("已经转移到Needs.Underly中")]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed public class FactoryViewAttribute : Attribute
    {
        Type type;

        public FactoryViewAttribute(Type type)
        {
            this.type = type;
        }

        public IFkoView<T> GetView<T>()
        {
            return Activator.CreateInstance(this.type) as IFkoView<T>;
        }
    }


    public interface IFkoView<T> : IQueryable<T>, IDisposable
    {
        T this[string index]
        {
            get;
        }
    }
    public sealed class FkoFactory<T> where T : class
    {
        FkoFactory()
        {

        }

        static FactoryViewAttribute attributer;
        static object locker = new object();
        static public T Create(string id)
        {
            if (attributer == null)
            {
                lock (locker)
                {
                    if (attributer == null)
                    {
                        attributer = typeof(T).GetCustomAttribute<FactoryViewAttribute>();
                    }
                }
            }

            using (var view = attributer.GetView<T>())
            {
                return view[id] as T ?? default(T);
            }
        }
    }
}
