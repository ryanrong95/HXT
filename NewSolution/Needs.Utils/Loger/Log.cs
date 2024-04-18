using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Utils
{
    public class Log
    {
        Stopwatch watch;
        public Log()
        {
            this.watch = new Stopwatch();
            this.watch.Start();
        }

        public void LogText(string text)
        {
            Loger.Instance().Add($"{text}");
        }

        public void LogTimer(string text)
        {
            this.watch.Stop();
            Loger.Instance().Add($"{text},执行时间:{ this.watch.ElapsedMilliseconds * 0.001 }秒");
            this.watch.Restart();
        }

        public void LogTimer(string text, long count)
        {
            this.watch.Stop();
            Loger.Instance().Add($"{text},执行时间:{ this.watch.ElapsedMilliseconds * 0.001 }秒,平均每次用时:{this.watch.ElapsedMilliseconds * 0.1m / count }毫秒");
            this.watch.Restart();
        }
    }
}
