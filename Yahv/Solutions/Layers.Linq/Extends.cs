using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Layers.Linq
{
    static class Extends
    {
        static public bool IsStop(this Thread thread)
        {
            var status = thread.ThreadState;
            return status == ThreadState.Stopped
                || status == ThreadState.StopRequested
                || status == ThreadState.Aborted
                || status == ThreadState.AbortRequested
                || status.HasFlag(ThreadState.Stopped)
                || status.HasFlag(ThreadState.StopRequested)
                || status.HasFlag(ThreadState.Aborted)
                || status.HasFlag(ThreadState.AbortRequested);
        }

        static public void Dispose(this ConcurrentQueue<IDisposable> queue)
        {
            //再次展示 扩展方法的优势
            if (queue == null)
            {
                return;
            }

            IDisposable puter;
            while (queue.TryDequeue(out puter))
            {
                puter.Dispose();
            }
        }
    }
}
