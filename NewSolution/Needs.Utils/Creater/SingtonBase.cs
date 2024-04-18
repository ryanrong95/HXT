using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Creater
{
    public abstract class SingtonBase<T> where T : SingtonBase<T>
    {
        static T instance;
        static object locker = new object();
        public static T Instance(params object[] param)
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        var type = typeof(T);
                        if (type.IsAssignableFrom(typeof(SingtonBase<>)))
                        {

                        }
                        if (!type.GetConstructors().All(item => item.IsPrivate))
                        {
                            throw new Exception($"类型[{type.FullName}]的构造函数必须为私有");
                        }
                        instance = (param == null || param.Length == 0 ? Activator.CreateInstance(type, true) : Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, param, null)) as T;
                    }
                }
            }
            return instance;
        }

        public static T Current
        {
            get { return Instance(); }
        }
    }
}
