using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Needs.Wl.CustomsTool.WinForm.Models;

namespace Needs.Wl.CustomsTool
{
    public class ManiApplyQueue
    {
        private Queue<ManifestConsignment> queue;

        public Queue<ManifestConsignment> Queue
        {
            get
            {
                if (this.queue == null)
                {
                    this.queue = new Queue<ManifestConsignment>();

                }
                return queue;
            }
        }

        public ManiApplyQueue()
        {
            this.threadStart();
        }


        private void threadStart()
        {
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
            while (true)
            {
                if (Queue.Count > 0)
                {
                    try
                    {
                        var dec = Queue.Dequeue();
                        dec.Apply();
                        
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
