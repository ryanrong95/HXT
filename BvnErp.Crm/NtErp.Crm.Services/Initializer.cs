using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services
{
    class Initializer : Needs.Overall.InitialBase
    {
        override protected string ProjcetName { get { return "Crm Two phase"; } }
    }
}
