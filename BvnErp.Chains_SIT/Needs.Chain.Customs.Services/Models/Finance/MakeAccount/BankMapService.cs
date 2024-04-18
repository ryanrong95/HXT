using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BankMapService
    {
        static object locker = new object();
        static BankMapService current;

        private BankMap bankMap;

        public BankMap BankMap
        {
            get
            {
                if (this.bankMap == null)
                {
                    this.bankMap = new BankMap();
                }
                return this.bankMap;
            }
            set { this.bankMap = value; }
        }

        private BankMapService()
        {
            this.bankMap = new BankMap();
        }

        public static BankMapService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new BankMapService();
                        }
                    }
                }
                return current;
            }
        }
    }
}
