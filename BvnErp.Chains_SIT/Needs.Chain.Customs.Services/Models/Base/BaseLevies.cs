using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BaseLevies : IUnique
    {
        public string ID { get; set; }

        public string Code { get; set; }

        public string BriefName { get; set; }

        public string FullName { get; set; }
    }
}
