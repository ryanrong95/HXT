using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Services.Models.Rolls
{
    public class AdminWareHouse
    {
        public string AdminID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentID { get; set; }
        public string ParentName { get; set; }
        public string ParentCode { get; set; }
    }
}
