using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BalanceUpdateLock
    {
        static private  BalanceUpdateLock current;
        static object locker = new object();
        public ReaderWriterLockSlim BalanceLock;

        private BalanceUpdateLock()
        {
            this.BalanceLock = new ReaderWriterLockSlim();
        }

        static public BalanceUpdateLock Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new BalanceUpdateLock();
                        }
                    }
                }

                return current;
            }
        }
    }
}
   
