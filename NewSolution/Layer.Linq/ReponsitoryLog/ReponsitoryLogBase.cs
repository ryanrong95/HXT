using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public class ReponsitoryLogBase
    {
        public string ID { get; set; } = string.Empty;

        public Status Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Summary { get; set; }
    }
}
