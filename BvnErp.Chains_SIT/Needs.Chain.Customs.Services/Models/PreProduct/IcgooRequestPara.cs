using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooRequestPara : IUnique
    {
        public string ID { get; set; }
        public string Supplier { get; set; }
        public int days { get; set; }
        public string RequestUrl { get; set; }
        public int PageSize { get; set; }
        public bool IsUse { get; set; }

        public string ClientID { get; set; }

    }
}
