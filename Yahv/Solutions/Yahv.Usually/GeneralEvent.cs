using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Usually
{
    public class GeneralEventArgs : EventArgs
    {
        public object[] Values { get; private set; }
        public GeneralEventArgs(params object[] values)
        {
            this.Values = values;
        }
    }
    
    
}
