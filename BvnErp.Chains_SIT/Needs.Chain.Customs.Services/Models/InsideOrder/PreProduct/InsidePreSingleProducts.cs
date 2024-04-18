using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InsidePreSingleProducts
    {
        public string Status { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public bool isPage { get; set; }
        public InsidePrePageInfo pageInfo { get; set; }
        public List<InsidePreSingleList> list { get; set; }
    }
}
