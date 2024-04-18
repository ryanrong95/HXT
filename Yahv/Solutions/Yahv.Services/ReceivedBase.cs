using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Events;
using Yahv.Underly;

namespace Yahv.Services
{
    public class ReceivedBase
    {
        private static object lockerWhs = new object();
        static private event ConfirmedHandler<WhsPayConfirmedEventArgs> whsConfirmed;

        static public event ConfirmedHandler<WhsPayConfirmedEventArgs> WhsConfirmed
        {
            add
            {
                if (whsConfirmed == null)
                {
                    lock (lockerWhs)
                    {
                        if (whsConfirmed == null)
                        {
                            whsConfirmed = value;
                        }
                    }
                }
                else
                {
                    throw new Exception("事件已经绑定!");
                }
            }
            remove { }
        }

        private static object lockerLs = new object();
        static private event ConfirmedHandler<LsPayConfirmedEventArgs> lsConfirmed;

        static public event ConfirmedHandler<LsPayConfirmedEventArgs> LsConfirmed
        {
            add
            {
                if (lsConfirmed == null)
                {
                    lock (lockerLs)
                    {
                        if (lsConfirmed == null)
                        {
                            lsConfirmed = value;
                        }
                    }
                }
                else
                {
                    throw new Exception("事件已经绑定!");
                }
            }
            remove { }
        }

        private static object lokcerOrder = new object();
        static private event OrderHandler recordingHandler;

        static public event OrderHandler Recording
        {
            add
            {
                if (recordingHandler == null)
                {
                    lock (lokcerOrder)
                    {
                        if (recordingHandler == null)
                        {
                            recordingHandler = value;
                        }
                    }
                }
            }
            remove { }
        }

        public void Fire(object sender, EventArgs e)
        {
            if (e is ConfirmedEventArgs<WhsPayConfirmedEventArgs>)
            {
                if (this != null && whsConfirmed != null)
                {
                    whsConfirmed(sender, e as ConfirmedEventArgs<WhsPayConfirmedEventArgs>);
                }
            }

            if (e is ConfirmedEventArgs<LsPayConfirmedEventArgs>)
            {
                if (this != null && lsConfirmed != null)
                {
                    lsConfirmed(sender, e as ConfirmedEventArgs<LsPayConfirmedEventArgs>);
                }
            }
            if (e is OrderEventArgs)
            {
                if (this != null && recordingHandler != null)
                {
                    recordingHandler(sender, e as OrderEventArgs);
                }
            }
        }
    }
}
