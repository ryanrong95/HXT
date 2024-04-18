using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Configuration.Model
{
    class TesterConfig : ConfigurationBase, ITesterConfig
    {
        public int Supremum { get; set; }
    }
}
