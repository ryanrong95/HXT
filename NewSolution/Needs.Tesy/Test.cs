using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Test
{
    public class Test
    {
        public void CheckUsingPath()
        {
            var frames = new StackTrace().GetFrames();
            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                var type = method.ReflectedType;
                var methodinfo = type.GetMethod(method.Name, BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methodinfo == null)
                {
                    continue;
                }
                var space = method.ReflectedType.Namespace;
                var returnType = methodinfo.ReturnType;
                var index = returnType.Name.IndexOf("IQueryable");
                //if(returnType == typeof(IQueryable) && space != "CnslApp")
                if (returnType.Name.IndexOf("IQueryable") >= 0 && space != "CnslApp")
                {
                    throw new Exception("非法的调用,调用路径是：" + method.ReflectedType.FullName + method.Name);
                }
            }
        }

        public void CheckUsingThread()
        {
            Queue<Thread> threads = new Queue<Thread>();
            Timer timer1 = new Timer(delegate (object obj) {
                var currentThread = Thread.CurrentThread;
                threads.Enqueue(currentThread);
            },null,10,20);
        }
    }
}
