using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BaseAppCertCode : IUnique
    {
        public string ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
