using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public class Releaser : IDisposable
    {
        public MethodBase Method { get; private set; }
        public Thread Thread { get; private set; }

        IDisposable watched;

        internal Releaser(Thread thread, IDisposable watched)
        {
            this.Thread = thread;
            this.watched = watched;

            StackTrace trace = new StackTrace(true);
            StackFrame sframe = trace.GetFrame(2);
            this.Method = sframe.GetMethod();
        }

        public void Dispose()
        {
            this.watched.Dispose();
        }
    }
}
