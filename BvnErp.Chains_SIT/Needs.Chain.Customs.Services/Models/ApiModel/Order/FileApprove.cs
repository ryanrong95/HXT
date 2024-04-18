using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FileApprove
    {
        public string OrderID { get; set; }

        public Enums.FileType Type { get; set; }
    }
}
