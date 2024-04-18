using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单轨迹/回执信息
    /// </summary>
    public class DutiablePriceJson
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public string count { get; set; }

        public List<DutiablePriceJsonItem> data { get; set; }
    }
}
