using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    /// <summary>
    /// 录入队列
    /// </summary>
    public class EnterQueue
    {
        ConcurrentQueue<MyParams> queue;

        /// <summary>
        /// 构造器
        /// </summary>
        protected EnterQueue()
        {
            this.queue = new ConcurrentQueue<MyParams>();

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    if (this.IsEmpty)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    MyParams mp;
                    while (this.queue.TryDequeue(out mp))
                    {
                        if (mp.Params is object[])
                        {
                            mp.Method.Invoke(mp.Sender, mp.Params as object[]);
                        }
                        else
                        {
                            mp.Method.Invoke(mp.Sender, new object[] { mp.Params });
                        }

                        if (this.IsEmpty)
                        {
                            current = null;
                            Thread.CurrentThread.Abort();
                        }
                    }
                }
            });

            thread.Start();
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.queue.IsEmpty;
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arry">参数</param>
        public bool Enqueue(object sender, params object[] arry)
        {
            //获取执行方法
            var frames = new StackTrace().GetFrames();
            var method = frames[1].GetMethod();

            if (method.DeclaringType == this.GetType())
            {
                return false;
            }

            if (frames[2].GetMethod().DeclaringType == typeof(System.RuntimeMethodHandle))
            {
                return false;
            }

            this.queue.Enqueue(new MyParams
            {
                Method = method,
                Params = arry,
                Sender = sender,
            });

            return true;
        }

        static EnterQueue current;
        static object locker = new object();

        /// <summary>
        /// 获取指定类型实例
        /// </summary>
        static public EnterQueue Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new EnterQueue();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 私有参数
        /// </summary>
        class MyParams
        {
            public MethodBase Method { get; set; }
            public object[] Params { get; set; }
            public object Sender { get; set; }
        }
    }

}
