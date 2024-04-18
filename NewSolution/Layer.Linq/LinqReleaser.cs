using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Layer.Linq
{
    /// <summary>
    /// linq 辅助释放器
    /// </summary>
    sealed class LinqReleaser : IDisposable
    {
        const int capacity = 5000;
        const int threadCount = capacity + 2;

        ConcurrentDictionary<Thread, ConcurrentQueue<IDisposable>> concurrent;

        Thread mainThread;
        bool runing = true;


        LinqReleaser()
        {
            this.concurrent = new ConcurrentDictionary<Thread, ConcurrentQueue<IDisposable>>(threadCount, capacity);
            (this.mainThread = new Thread(delegate ()
            {
                var dic = this.concurrent;
                while (this.runing)
                {
                    if (dic.IsEmpty)
                    {
                        Thread.Sleep(2);
                        continue;
                    }
                    var threads = dic.Keys.Take(threadCount).Where(item => item.IsStop());
                    foreach (var thread in threads)
                    {
                        ConcurrentQueue<IDisposable> queue;
                        if (dic.TryRemove(thread, out queue))
                        {
                            queue.Dispose();
                        }
                    }

                    //jzf 20181016 测试是否可以降低cpu占用
                    Thread.Sleep(1);
                }
            })).Start();
        }

        public ConcurrentQueue<IDisposable> Queue
        {
            get
            {
                ConcurrentQueue<IDisposable> queue;
                if (this.concurrent.TryGetValue(Thread.CurrentThread, out queue))
                {
                    return queue;
                }

                return null;
            }
        }

        /// <summary>
        /// 入队侦听
        /// </summary>
        /// <param name="thread">侦听的线程</param>
        /// <param name="liqns">要被释放的实例</param>
        public T Enqueue<T>(Thread thread, T watched) where T : IDisposable
        {
            var dic = this.concurrent;
            ConcurrentQueue<IDisposable> queue;
            if (!dic.TryGetValue(thread, out queue))
            {
                queue = dic.GetOrAdd(thread, new ConcurrentQueue<IDisposable>());
            }
            queue.Enqueue(watched);

            //queue.Enqueue(new Releaser(thread, watched));

            return watched;
        }

        public T Enqueue<T>(T watched) where T : IDisposable
        {
            return this.Enqueue(Thread.CurrentThread, watched);
        }

        public void Dispose()
        {
            this.runing = false;
            this.mainThread.Abort();
        }

        static LinqReleaser current;
        static object lockhelper = new object();
        /// <summary>
        /// 当前侦听者
        /// </summary>
        static public LinqReleaser Current
        {
            get
            {
                if (current == null)
                {
                    lock (lockhelper)
                    {
                        if (current == null)
                        {
                            current = new LinqReleaser();
                        }
                    }
                }
                return current;
            }
        }


    }
}
