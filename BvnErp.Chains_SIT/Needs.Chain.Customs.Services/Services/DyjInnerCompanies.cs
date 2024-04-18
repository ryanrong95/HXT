using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public  class DyjInnerCompanies
    {
        public string Companies { get; set; }
        static object locker = new object();
        private DyjInnerCompanies()
        {
            Companies = System.Configuration.ConfigurationManager.AppSettings["DyjInnerCompanies"];
            this.Companies = Companies;
        }

        private static DyjInnerCompanies current;

        public static DyjInnerCompanies Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new DyjInnerCompanies();
                        }
                    }
                }
                return current;
            }
        }
    }
}
