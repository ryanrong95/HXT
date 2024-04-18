using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InsidePreProducts
    {
        public string Status { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public bool isPage { get; set; }
        public InsidePrePageInfo pageInfo { get; set; }
        public List<InsidePreList> list { get; set; }
    }
}
