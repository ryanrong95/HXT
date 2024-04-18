using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class XDTStaff : IUnique
    {
        public string ID { get; set; }

        public string AdminID { get; set; }

        public string OriginID { get; set; }

        public string StaffID { get; set; }

        public string DepartmentCode { get; set; }
    }
}
