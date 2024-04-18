using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Flow.Event
{
    public class HandlerInfo
    {
        public int SerialNo { get; set; }

        public string Namespace { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }
    }
}
