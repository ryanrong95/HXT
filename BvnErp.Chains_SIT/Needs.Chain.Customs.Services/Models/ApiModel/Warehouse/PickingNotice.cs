using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PickingNotice : Notice
    {
        public CenterProduct Product { get; set; }

        public Picking Picking { get; set; }

        public Output Output { get; set; }
    }
}
