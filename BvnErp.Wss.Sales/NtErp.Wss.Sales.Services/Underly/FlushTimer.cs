using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public class FlushTimerEventArgs : EventArgs
    {

    }

    public delegate void FlushTimerHandle(object sender, FlushTimerEventArgs e);


    public class FlushTimer<T> where T : class
    {
        public event FlushTimerHandle Init;
        public int Interval { get; private set; }

        Thread thread;
        T target;
        object[] parameters;

        public FlushTimer(params object[] arry) : this(1500, arry)
        {

        }

        public FlushTimer(int interval, params object[] arry)
        {
            this.Interval = interval;
            this.parameters = arry;
            this.Initialize();
        }

        void Initialize()
        {
            if (parameters == null || parameters.Length == 0)
            {
                this.target = Activator.CreateInstance<T>();
            }
            else
            {
                this.target = Activator.CreateInstance(typeof(T), this.parameters) as T;
            }

            if (this != null && this.Init != null)
            {
                this.Init(this, new FlushTimerEventArgs());
            }

        }

        public T Current
        {
            get
            {
                if (this.thread == null)
                {
                    lock (this)
                    {
                        if (this.thread == null)
                        {
                            (this.thread = new Thread(delegate ()
                            {
                                try
                                {
                                    this.Initialize();
                                    Thread.Sleep(this.Interval);
                                    this.thread = null;
                                    Thread.CurrentThread.Abort();
                                }
                                catch
                                {
                                    return;
                                }
                            })).Start();
                        }
                    }
                }

                return this.target;
            }
        }
    }
}
